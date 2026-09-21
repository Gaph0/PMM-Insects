using HarmonyLib;
using ProjectMomo;
using RimWorld;
using Verse;

namespace PMM_Insects
{
    /// <summary>
    /// The end of the line for anyone a hive momo carries off the map: he never
    /// reaches the hive. Vanilla files any pawn carried off the map with the
    /// carrier's faction as a hostage (Pawn.ExitMap -> KidnappedPawnsTracker.Kidnap),
    /// and for the insect geneline that hostage is unreachable: no settlement to
    /// raid, no rescue quest — a hidden faction can hold no sites — only a random
    /// ransom demand, and a silent absorption into the faction after about a month.
    /// The hive keeps no prisoners, so a hive kidnapping ends here instead.
    ///
    /// The vanilla method is skipped rather than undone, so the hostage entry, the
    /// tracker and the game's own "he is not lost forever" letter never happen at
    /// all. Everything else that body did is done by hand below, so the rest of the
    /// game still sees a kidnapping: the storyteller event, the tale, the relatives'
    /// grief, the released bills, the quest signal and the game over check.
    ///
    /// The hook is the end of the chase, so the player keeps every chance to
    /// intercept her and get him back — he is only gone once she clears the map.
    /// And if the willpower knockout wears off while she carries him, the kidnap job
    /// fails and she drops him: that escape still works.
    /// </summary>
    [HarmonyPatch(typeof(KidnappedPawnsTracker), nameof(KidnappedPawnsTracker.Kidnap))]
    public static class Patch_HiveKidnapVanish
    {
        /// <summary>
        /// Skip this patch if the game ever renames or removes the method. Without
        /// the guard a missing target makes PatchAll throw on load, which would take
        /// every other patch in this assembly down with it.
        /// </summary>
        [HarmonyPrepare]
        public static bool Prepare()
        {
            return AccessTools.Method(typeof(KidnappedPawnsTracker), nameof(KidnappedPawnsTracker.Kidnap),
                new System.Type[] { typeof(Pawn), typeof(Pawn) }) != null;
        }

        /// <summary>
        /// Runs instead of the game's hostage bookkeeping for a hive momo's victim,
        /// and returns false so the hostage entry, the tracker, the ransom candidate
        /// and the game's "not lost forever" letter never happen.
        /// </summary>
        public static bool Prefix(Pawn pawn, Pawn kidnapper)
        {
            if (pawn == null || pawn.Dead || pawn.Destroyed || !HiveInsectMomo.IsHiveInsectMomo(kidnapper))
            {
                return true; // not ours: the game's own kidnap, untouched
            }

            // Vanilla's own loss handler, taken from the body being replaced:
            // storyteller adaptation, the kidnapped-colonist tale, ownership
            // unclaimed, guest status cleared, the relatives' kidnap notification
            // and the victim's mind cleared. None of it is about the hostage entry,
            // so all of it still belongs here.
            pawn.PreKidnapped(kidnapper);

            // Sever the tsugai bond while his relations still exist, through the
            // core's own "partner died" path: every bonded Momo (not only the
            // kidnapper) loses the relation and the bond hediff and takes the grief.
            // Without this she would stay bonded to a pawn that no longer exists —
            // unkillable, unreachable and blocking her next bond.
            TsugaiFormation.HandleBondPartnerDied(pawn);

            // The rest of what the vanilla body did. Still right for a man who is
            // gone: his relatives and lovers mourn him, bills stop waiting on him,
            // any quest watching him hears the signal, and a colony that just lost
            // its last colonist still ends.
            PawnDiedOrDownedThoughtsUtility.TryGiveThoughts(pawn, null, PawnDiedOrDownedThoughtsKind.Lost);
            BillUtility.Notify_ColonistUnavailable(pawn);
            QuestUtility.SendQuestTargetSignals(pawn.questTags, "Kidnapped", pawn.Named("SUBJECT"), kidnapper.Named("KIDNAPPER"));
            Find.GameEnder.CheckOrUpdateGameOver();

            // Erase him with the game's own eraser for a pawn nobody should
            // reference any more: out of the world pawns, relations notified, all
            // worn and carried gear destroyed, and no corpse left behind (Vanish
            // never produces leavings).
            pawn.Destroy(DestroyMode.Vanish);
            Find.WorldPawns.RemoveAndDiscardPawnViaGC(pawn);

            return false; // the game's hostage bookkeeping never runs
        }
    }
}
