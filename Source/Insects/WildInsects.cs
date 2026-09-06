using HarmonyLib;
using RimWorld;
using Verse;

namespace PMM_Insects
{
    /// <summary>
    /// Wild insect momos are tamed like wild men. The slime-mod pattern: report
    /// the pawn as a wild man for taming purposes when it carries an insect wild
    /// kind. The Insect-faction kinds (PMM_Insect*) are deliberately NOT covered —
    /// raid/infestation bug girls are faction NPCs, captured and recruited like
    /// any other enemy.
    /// </summary>
    [HarmonyPatch(typeof(WildManUtility), nameof(WildManUtility.IsWildMan))]
    public static class Patch_InsectIsWildMan
    {
        public static void Postfix(Pawn p, ref bool __result)
        {
            if (!__result && p?.kindDef != null && !p.IsSubhuman
                && p.kindDef.defName.StartsWith("PMM_Wild"))
            {
                __result = true;
            }
        }
    }

    /// <summary>
    /// Stop wild insect momos from marching to the map edge and despawning the
    /// instant they spawn. Vanilla WildManShouldReachOutsideNow returns true for
    /// any wild man who hasn't "reached outside", driving the edge-walk job every
    /// think tick. Report wild insect momos as already reached outside so the
    /// walk never triggers (the slime-mod Patch_SlimeShouldNotReachOutside
    /// pattern — every wild-momo port needs this AND the IsWildMan patch AND the
    /// think-tree insert in ThinkTrees_InsectWild.xml; see PLAN.md §5).
    /// </summary>
    [HarmonyPatch(typeof(WildManUtility), nameof(WildManUtility.WildManShouldReachOutsideNow))]
    public static class Patch_InsectShouldNotReachOutside
    {
        public static void Postfix(Pawn p, ref bool __result)
        {
            if (__result && p?.kindDef != null && p.kindDef.defName.StartsWith("PMM_Wild"))
            {
                __result = false;
            }
        }
    }
}
