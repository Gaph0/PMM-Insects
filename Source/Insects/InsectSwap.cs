using HarmonyLib;
using RimWorld;
using Verse;

namespace PMM_Insects
{
    /// <summary>
    /// The generation swap. A prefix on PawnGenerator.GeneratePawn that replaces
    /// the six vanilla insect pawnkinds with the matching insect-momo pawnkind —
    /// a TOTAL replacement, no vanilla bugs spawn anywhere. Every spawn source
    /// funnels through PawnGenerator — infestations and hives (CompSpawnerPawn),
    /// Odyssey insect lairs and egg sacs, insectoid-mod spawners, wild biome
    /// spawns, ancient dangers, deep drilling, quests and trader livestock — so
    /// one patch covers all of them. The swap is contextual:
    ///
    ///   request.Faction == null  ->  PMM_Wild&lt;Species&gt;     (wild woman, tameable
    ///                                 via the IsWildMan patches)
    ///   any faction              ->  PMM_Insect&lt;Species&gt;   (hostile hive/raid momo;
    ///                                 joins her spawner's lord whether the faction is
    ///                                 the vanilla Insect faction or an insectoid mod's)
    ///
    /// Vanilla CompSpawnerPawn fixes the pawn's biological age to the bug's adult
    /// minAge (0.2-0.4y); the swap clears that and forces Adult so the humanlike
    /// pawn generates as a grown woman instead of a baby who can't stand. Odyssey
    /// kinds (Larva/Locust/HiveQueen) may not exist when Odyssey is absent — the
    /// GetNamedSilentFail guard makes the swap a no-op then.
    /// </summary>
    [HarmonyPatch(typeof(PawnGenerator), nameof(PawnGenerator.GeneratePawn),
        new System.Type[] { typeof(PawnGenerationRequest) })]
    public static class Patch_InsectGenerationSwap
    {
        private static string SwapKindName(string defName, Faction faction)
        {
            string species;
            switch (defName)
            {
                case "Megascarab": species = "DevilBug"; break;
                case "Spelopede": species = "GiantAnt"; break;
                case "Megaspider": species = "SoldierBeetle"; break;
                case "Larva": species = "Greenworm"; break;
                case "Locust": species = "VampMosquito"; break;
                case "HiveQueen": species = "Abaddon"; break;
                default: return null;
            }
            // Factionless (wild biome spawns, some trader livestock) -> tameable wild
            // woman. ANY faction (vanilla Insect hives AND insectoid-mod hives whose
            // faction defName isn't literally "Insect") -> the hostile hive/raid momo,
            // who joins her spawner's lord regardless of which faction it is.
            return (faction == null ? "PMM_Wild" : "PMM_Insect") + species;
        }

        public static void Prefix(ref PawnGenerationRequest request)
        {
            PawnKindDef kind = request.KindDef;
            if (kind?.defName == null)
            {
                return;
            }
            // Total replacement: EVERY vanilla insect pawnkind becomes the matching
            // insect momo, wherever it spawns from — hives, infestations, insectoid
            // mods' spawners, wild spawns, ancient dangers, traders. No coexisting
            // vanilla bugs.
            //
            // Vanilla CompSpawnerPawn sets fixedBiologicalAge to the bug's adult
            // minAge (0.2-0.4 years). On a humanlike race that rolls a baby who can't
            // stand -> "Generated downed pawn" x120 -> the spawner NREs. Clear the
            // fixed age and force Adult so the swap always generates a grown woman.
            Faction faction = request.Faction;
            string swapName = SwapKindName(kind.defName, faction);
            if (swapName == null)
            {
                return;
            }
            PawnKindDef swap = DefDatabase<PawnKindDef>.GetNamedSilentFail(swapName);
            if (swap == null)
            {
                return; // Odyssey species without the DLC: leave the vanilla kind alone
            }
            request = new PawnGenerationRequest(
                swap, request.Faction, request.Context, request.Tile,
                forceGenerateNewPawn: request.ForceGenerateNewPawn,
                allowDead: request.AllowDead,
                allowDowned: request.AllowDowned,
                canGeneratePawnRelations: request.CanGeneratePawnRelations,
                mustBeCapableOfViolence: request.MustBeCapableOfViolence,
                colonistRelationChanceFactor: request.ColonistRelationChanceFactor,
                forceAddFreeWarmLayerIfNeeded: request.ForceAddFreeWarmLayerIfNeeded,
                allowGay: request.AllowGay,
                allowPregnant: request.AllowPregnant,
                allowFood: request.AllowFood,
                allowAddictions: request.AllowAddictions,
                inhabitant: request.Inhabitant,
                certainlyBeenInCryptosleep: request.CertainlyBeenInCryptosleep,
                forceRedressWorldPawnIfFormerColonist: request.ForceRedressWorldPawnIfFormerColonist,
                worldPawnFactionDoesntMatter: request.WorldPawnFactionDoesntMatter,
                biocodeWeaponChance: request.BiocodeWeaponChance,
                biocodeApparelChance: request.BiocodeApparelChance,
                extraPawnForExtraRelationChance: request.ExtraPawnForExtraRelationChance,
                relationWithExtraPawnChanceFactor: request.RelationWithExtraPawnChanceFactor,
                validatorPreGear: request.ValidatorPreGear,
                validatorPostGear: request.ValidatorPostGear,
                forcedTraits: request.ForcedTraits,
                prohibitedTraits: request.ProhibitedTraits,
                minChanceToRedressWorldPawn: request.MinChanceToRedressWorldPawn,
                // Clear the bug's fixed adult minAge (0.2-0.4y): on a humanlike race
                // that is a baby. Let her generate at a normal adult age instead.
                fixedBiologicalAge: null,
                fixedChronologicalAge: request.FixedChronologicalAge,
                fixedGender: request.FixedGender,
                fixedLastName: request.FixedLastName,
                fixedBirthName: request.FixedBirthName,
                fixedTitle: request.FixedTitle,
                fixedIdeo: request.FixedIdeo,
                forceNoIdeo: request.ForceNoIdeo,
                forceNoBackstory: request.ForceNoBackstory,
                forbidAnyTitle: request.ForbidAnyTitle,
                forceDead: request.ForceDead,
                forcedXenogenes: request.ForcedXenogenes,
                forcedEndogenes: request.ForcedEndogenes,
                forcedXenotype: request.ForcedXenotype,
                forcedCustomXenotype: request.ForcedCustomXenotype,
                allowedXenotypes: request.AllowedXenotypes,
                forceBaselinerChance: request.ForceBaselinerChance,
                developmentalStages: DevelopmentalStage.Adult,
                forceNoGear: request.ForceNoGear);
        }
    }
}
