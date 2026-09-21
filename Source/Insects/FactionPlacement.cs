using System;
using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace PMM_Insects
{
    /// <summary>
    /// The two insector tribes only settle polluted ground (ruling 2026-09-20).
    /// Vanilla has no XML hook for a per-faction settlement tile filter, so this
    /// postfix re-rolls the result of TileFinder.RandomSettlementTileFor for our
    /// two factions until the tile is polluted, chaining the caller's own
    /// extraValidator so every other vanilla rule still applies.
    ///
    /// Pollution lives on the world tile as <c>RimWorld.Planet.Tile.pollution</c>,
    /// clamped 0..1 (Biotech). It is generated at world creation and grows during
    /// play whenever a map pollutes its tile (waste packs, pollution pumps,
    /// toxifiers) - WorldPollutionUtility.PolluteWorldAtTile spreads it to
    /// neighbours out to radius 4. So in practice these villages cluster on the
    /// polluted regions of the world, and a colony that pollutes its own
    /// neighbourhood is asking for neighbours.
    ///
    /// Follows the Reptiles precedent (its mountain-only placement), including the
    /// graceful fallback: 300 re-rolls x TileFinder's own 500 candidates is plenty
    /// to find polluted ground in any world that has some. If a world has none at
    /// all - no Biotech pollution anywhere - the vanilla tile is kept and a dev
    /// warning is logged, because a tribe with no village still raids and still
    /// exists, while silently placing it on clean ground would be a worse lie.
    /// Make it strict by returning PlanetTile.Invalid instead of keeping result.
    ///
    /// Both overloads are patched, since which one a caller uses is unspecified.
    /// The reentrancy flag stops the postfix's own re-roll calls from recursing.
    /// </summary>
    public static class PollutedSettlement
    {
        /// <summary>
        /// Any tile carrying at least this much pollution counts as polluted. The
        /// polluted region's fringe sits well above this, so the tribes still get
        /// a spread of tiles rather than one shared centre.
        /// </summary>
        public const float MinPollution = 0.1f;

        private static bool rerolling;

        /// <summary>
        /// Compared by defName rather than through a DefOf class, so a load-order
        /// or rename problem cannot throw during static constructor resolution.
        /// </summary>
        public static bool IsInsectTribe(Faction faction)
        {
            string name = faction?.def?.defName;
            return name == "PMM_InsectorHive" || name == "PMM_InsectorSwarm";
        }

        private static bool IsPolluted(PlanetTile tile)
        {
            if (tile == PlanetTile.Invalid)
            {
                return false;
            }
            Tile worldTile = Find.WorldGrid[tile];
            return worldTile != null && worldTile.pollution >= MinPollution;
        }

        /// <summary>
        /// Re-roll through <paramref name="invoke"/> (the same overload the caller
        /// used) with a pollution-only validator chained onto the caller's
        /// validator. Returns PlanetTile.Invalid when no polluted tile was found.
        /// </summary>
        public static PlanetTile Reroll(Func<Predicate<PlanetTile>, PlanetTile> invoke,
            Predicate<PlanetTile> extraValidator)
        {
            rerolling = true;
            try
            {
                for (int i = 0; i < 300; i++)
                {
                    PlanetTile candidate = invoke(
                        t => IsPolluted(t) && (extraValidator == null || extraValidator(t)));
                    if (candidate != PlanetTile.Invalid)
                    {
                        return candidate;
                    }
                }
                return PlanetTile.Invalid;
            }
            finally
            {
                rerolling = false;
            }
        }

        public static void PostfixImpl(Faction faction, Func<Predicate<PlanetTile>, PlanetTile> invoke,
            Predicate<PlanetTile> extraValidator, ref PlanetTile result)
        {
            if (rerolling || !IsInsectTribe(faction))
            {
                return;
            }
            if (result != PlanetTile.Invalid && IsPolluted(result))
            {
                return; // already on polluted ground
            }
            PlanetTile polluted = Reroll(invoke, extraValidator);
            if (polluted != PlanetTile.Invalid)
            {
                result = polluted;
                if (Prefs.DevMode)
                {
                    Log.Message($"[PMM Insects] Moved {faction.def.defName} settlement to polluted tile {polluted} "
                        + $"(pollution {PollutionOf(polluted):F2})");
                }
            }
            else if (Prefs.DevMode)
            {
                Log.Warning($"[PMM Insects] No polluted tile found for {faction.def.defName}; "
                    + $"keeping the vanilla tile {result}. A world with no pollution anywhere has none.");
            }
        }

        private static float PollutionOf(PlanetTile tile)
        {
            Tile worldTile = Find.WorldGrid[tile];
            return worldTile?.pollution ?? 0f;
        }
    }

    [HarmonyPatch(typeof(TileFinder), nameof(TileFinder.RandomSettlementTileFor),
        new[] { typeof(PlanetLayer), typeof(Faction), typeof(bool), typeof(Predicate<PlanetTile>) })]
    public static class Patch_RandomSettlementTileFor_Layer
    {
        public static void Postfix(PlanetLayer layer, Faction faction, bool mustBeAutoChoosable,
            Predicate<PlanetTile> extraValidator, ref PlanetTile __result)
        {
            PollutedSettlement.PostfixImpl(faction,
                v => TileFinder.RandomSettlementTileFor(layer, faction, mustBeAutoChoosable, v),
                extraValidator, ref __result);
        }
    }

    [HarmonyPatch(typeof(TileFinder), nameof(TileFinder.RandomSettlementTileFor),
        new[] { typeof(Faction), typeof(bool), typeof(Predicate<PlanetTile>) })]
    public static class Patch_RandomSettlementTileFor_Faction
    {
        public static void Postfix(Faction faction, bool mustBeAutoChoosable,
            Predicate<PlanetTile> extraValidator, ref PlanetTile __result)
        {
            PollutedSettlement.PostfixImpl(faction,
                v => TileFinder.RandomSettlementTileFor(faction, mustBeAutoChoosable, v),
                extraValidator, ref __result);
        }
    }
}
