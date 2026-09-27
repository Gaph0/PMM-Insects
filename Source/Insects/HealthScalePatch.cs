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
                // Greenworm: 0.25 base x 2.5 = 0.625 effective. The larva is the
                // frailest bug; same treatment keeps her limbs on her body.
                "PMM_Race_Greenworm" => 2.5f,
                // Giant Ant (1.7), Soldier Beetle (2.5), Vamp Mosquito (0.7), Abaddon
                // (9.8) and Abaddon Folk (1.0) need no help - their base scales already
                // meet or exceed human norms, and B&S scales big races' health itself.
                _ => 1f,
            };

            if (factor != 1f)
            {
                __result *= factor;
            }
        }
    }
}

