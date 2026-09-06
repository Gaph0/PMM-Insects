using HarmonyLib;
using RimWorld;
using Verse;

namespace PMM_Insects
{
    /// <summary>
    /// The generation swap. A prefix on PawnGenerator.GeneratePawn that replaces
    /// the six vanilla insect pawnkinds with the matching insect-momo pawnkind.
    /// Every spawn source funnels through PawnGenerator — infestations and hives
    /// (CompSpawnerPawn), Odyssey insect lairs and egg sacs, wild biome spawns,
    /// ancient dangers, deep drilling, quests and trader livestock — so one patch
    /// covers all of them. The swap is contextual:
    ///
    ///   request.Faction == Insect  ->  PMM_Insect&lt;Species&gt;  (stays in the Insect
    ///                                  faction: hostile hive/raid NPC)
    ///   otherwise                  ->  PMM_Wild&lt;Species&gt;     (wild woman, tameable
    ///                                  via the IsWildMan patches)
    ///
    /// Everything else in the request is left untouched, so faction, position and
    /// lord assignment flow through exactly as they did for the bug. Odyssey kinds
    /// (Larva/Locust/HiveQueen) may not exist when Odyssey is absent — the
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
            bool insectFaction = faction?.def?.defName == "Insect";
            return (insectFaction ? "PMM_Insect" : "PMM_Wild") + species;
        }

        public static void Prefix(ref PawnGenerationRequest request)
        {
            PawnKindDef kind = request.KindDef;
            if (kind?.defName == null)
            {
                return;
            }
            // Only swap genuine insect spawns. The Insect faction is obvious; a null
            // faction covers wild spawns. ANY other faction (a slime-faction hive comp,
            // a trader's livestock faction, etc.) is another mod borrowing the vanilla
            // insect kind — swapping that produced a hostile insect-faction NPC with a
            // mismatched request (fixedBiologicalAge, gear, etc.) and PawnGenerator
            // failed with "Generated downed pawn" 120 times and NRE'd. Leave it alone.
            Faction faction = request.Faction;
            if (faction != null && faction.def?.defName != "Insect")
            {
                return;
            }
            // VFE Insectoids and similar insectoid mods pass the VANILLA Insect faction
            // to their hive spawns but force a tiny fixedBiologicalAge (e.g. 0.2 years)
            // suited to a bug, not a woman. Swapping those into a humanlike race makes
            // PawnGenerator roll a baby that can't stand -> "Generated downed pawn" x120
            // and the hive NREs every tick. Vanilla insect spawns never fix the age, so
            // any fixed age is a modded spawn we must leave alone.
            if (request.FixedBiologicalAge.HasValue)
            {
                return;
            }
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
                fixedBiologicalAge: request.FixedBiologicalAge,
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
                developmentalStages: request.AllowedDevelopmentalStages,
                forceNoGear: request.ForceNoGear);
        }
    }
}
