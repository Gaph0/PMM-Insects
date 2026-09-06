using HarmonyLib;
using RimWorld;
using Verse;

namespace PMM_Insects
{
    /// <summary>
    /// Suppress the "Humanlike pawn X was added to non-humanlike faction insect
    /// geneline" Log.Error that Faction.Notify_PawnJoined fires for every insect
    /// momo joining a hive. The vanilla Insect faction has humanlikeFaction=false,
    /// so vanilla flags our (intentionally humanlike) hive momos as errors. They
    /// are not errors — they are the whole point of the mod — so silence the log
    /// for our PMM_Insect kinds. Everything else about the join proceeds normally.
    /// Implemented as a prefix that runs the original body for non-PMM pawns and
    /// returns false (skips) for ours.
    /// </summary>
    [HarmonyPatch(typeof(Faction), nameof(Faction.Notify_PawnJoined))]
    public static class Patch_InsectFactionJoinNoError
    {
        public static bool Prefix(Faction __instance, Pawn p)
        {
            // Only skip the error for our insect momos joining the Insect faction.
            // The original method body is tiny: it notifies the ideos tracker and then
            // logs the error for humanlike pawns in non-humanlike factions. We keep the
            // ideos notification and drop only the error, so behaviour is unchanged
            // apart from the missing red line.
            if (p?.kindDef != null && p.kindDef.defName.StartsWith("PMM_Insect"))
            {
                __instance.ideos?.Notify_MemberGainedOrLost();
                return false; // skip the original (and its Log.Error)
            }
            return true; // everyone else: run vanilla as normal
        }
    }
}
