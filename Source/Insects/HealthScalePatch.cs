using HarmonyLib;
using Verse;

namespace PMM_Insects
{
    /// <summary>
    /// Multiplies Pawn.HealthScale for PMM insect races by a per-species factor.
    ///
    /// Why this exists: vanilla HealthScale (= ageStage.healthScaleFactor *
    /// RaceProps.baseHealthScale) drives every body part's max HP, limbs AND
    /// organs. Insect mamonos clone their bugs' tiny baseHealthScale (devil bug
    /// 0.4), which left limbs paper-thin - a 7-damage punch on an 8-HP arm.
    /// Big & Small offers no part-HP hook, and no hediff can raise max HP, so
    /// this postfix is the one knob that scales limbs and organs together.
    ///
    /// Race-level (not gene-level): can never stack with itself, and it composes
    /// cleanly with the tracker's armor stage + internalDamageDivisor - each
    /// layer covers a different failure mode (armor = roll to halve, divisor =
    /// organ survival, this = raw part HP).
    ///
    /// Note: the race tracker stays VISIBLE in the Health tab, and it has to. B&S draws a
    /// tracker's render art through the hediff's own nodes and only installs them while the
    /// hediff is Visible, so hiding the row hides the pawn's wings with it - tried and
    /// reverted 2026-09-26 (a core patch, RaceTrackerRowHidden.cs, deleted again). One
    /// readable row is the cheaper half of that trade.
    /// </summary>
    [HarmonyPatch(typeof(Pawn), nameof(Pawn.HealthScale), MethodType.Getter)]
    public static class HealthScalePatch
    {
        public static void Postfix(ref float __result, Pawn __instance)
        {
            if (__instance?.def == null) return;

            float factor = __instance.def.defName switch
            {
                // Devil Bug: 0.4 base x 2.5 = 1.0 effective. Her 8-HP arm becomes
                // ~20 HP, so punches bruise instead of amputating.
                "PMM_Race_DevilBug" => 2.5f,
                // Arachne: 0.4 base x 2.5 = 1.0 effective, the same silk-over-chitin
                // frame as the devil bug's. She was missing from this switch while
                // her own race def claimed the factor was applied, so she sat at 0.4
                // and one punch could take one of her eight legs off - the failure
                // this patch exists to prevent.
                "PMM_Race_Arachne" => 2.5f,
                // Greenworm: 0.25 base x 2.5 = 0.625 effective. The larva is the
                // frailest bug; same treatment keeps her limbs on her body.
                "PMM_Race_Greenworm" => 2.5f,
                // Giant Ant (1.7), Soldier Beetle (2.5), Abaddon (9.8) and Abaddon
                // Folk (1.0) need no help: B&S scales big races' health itself, and
                // those bases already meet or exceed human norms.
                // Vamp Mosquito (0.7) and Beelzebub (0.7) are left alone on purpose. Both are meant
                // to be fragile - the swarm's fliers, not its brawlers (user's word, 2026-10-01:
                // "both are meant to be fragile"). Named rather than merely absent, so the default
                // arm below cannot read them as an omission.
                "PMM_Race_VampMosquito" => 1f,
                "PMM_Race_Beelzebub" => 1f,
                // Girtablilu: 0.44 base x 2.5 = 1.1 effective. A plated scorpion
                // body sits a tenth above a human's where the arachne sits on one.
                "PMM_Race_Girtablilu" => 2.5f,
                // Ant Arachne: 0.36 base x 2.5 = 0.9 effective. She is the small
                // worker of the family, and a tenth under a human is the point.
                "PMM_Race_AntArachne" => 2.5f,
                // Honey Bee: 0.4 base x 2.5 = 1.0 effective, the devil bug's frame. A
                // farmer's body and not a fighter's, but it keeps her limbs on her.
                "PMM_Race_HoneyBee" => 2.5f,
                // Mothman: 0.4 base x 2.0 = 0.8 effective. The hive's gentle one - softer
                // than a human and tougher than the papillon, and never in a fight anyway.
                "PMM_Race_Mothman" => 2.0f,
                // Papillon: 0.4 base x 1.25 = 0.5 effective, the frailest caste in the mod
                // (CASTES-PLAN.md 0 decision 10). Her limbs are meant to be a liability.
                "PMM_Race_Papillon" => 1.25f,
                // Hornet: 0.4 base x 2.0 = 0.8 effective, the mothman's frame. She is a
                // fighter, but her venom is what makes her dangerous, not her shell.
                "PMM_Race_Hornet" => 2.0f,
                // Mantis: 0.4 base x 1.75 = 0.7 effective (CASTES-PLAN.md §0 decision 10).
                // The thinnest shell of any fighter here, and VRE_WeakenedChitin takes x1.5
                // of whatever lands on it - she is meant to be the glass half of the pair.
                "PMM_Race_Mantis" => 1.75f,
                _ => WarnIfUnscaled(__instance.def),
            };

            if (factor != 1f)
            {
                __result *= factor;
            }
        }

        /// <summary>
        /// The default arm, and the only place this patch looks at anything but a name: a race of ours
        /// with a base scale under a human's is either handled above or is the next omission - exactly
        /// the failure this patch was written for (the arachne sat at 0.4 while her own def claimed
        /// otherwise). Once per race, so a shipped mod says it in the log rather than never.
        /// </summary>
        private static float WarnIfUnscaled(ThingDef def)
        {
            if (def.defName.StartsWith("PMM_Race_") && def.race != null && def.race.baseHealthScale < 1f)
            {
                Log.WarningOnce($"[PMM Insects] {def.defName} has baseHealthScale "
                    + $"{def.race.baseHealthScale} and no line in HealthScalePatch; her body parts "
                    + "stay that thin.", def.shortHash);
            }
            return 1f;
        }
    }
}

