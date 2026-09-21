using System.Collections.Generic;
using HarmonyLib;
using ProjectMomo;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace PMM_Insects
{
    /// <summary>
    /// Which Momo counts as a hive momo. Two conditions, both required:
    ///
    ///   * a PMM_Insect* pawnkind — the hive/raid kinds. Wild insect momos are
    ///     PMM_Wild* kinds, so they never match here, and neither do the
    ///     PMM_Insect* xenotypes those wild momos carry (the xenotype is not
    ///     checked at all).
    ///   * the vanilla Insect faction. Insect momos spawned for any other faction
    ///     — insectoid mods' hives, trader caravans' livestock, raiders — keep the
    ///     core's tsugai behaviour untouched.
    ///
    /// Shared by the bond-outcome patch below and by HiveKidnapVanish.
    /// </summary>
    public static class HiveInsectMomo
    {
        public static bool IsHiveInsectMomo(Pawn pawn)
        {
            if (pawn == null || pawn.kindDef == null || pawn.kindDef.defName == null
                || !pawn.kindDef.defName.StartsWith("PMM_Insect"))
            {
                return false;
            }

            FactionManager factions = Find.FactionManager;
            return factions != null && pawn.Faction != null && pawn.Faction == factions.OfInsects;
        }
    }

    /// <summary>
    /// A hive momo never joins the colony. When she wins a tsugai bond against one
    /// of your colonists the core rolls a join chance (TsugaiFormation.DecideOutcome);
    /// for a hive momo that roll is now always a loss — she goes home and takes her
    /// husband with her, through this mod's own kidnap.
    ///
    /// Hooked on TsugaiFormation.ExecuteJoin rather than on the decision itself,
    /// because that is the single funnel for every join — the forced knockout bond,
    /// the voluntary proposal and the wild-momo tame all end there — and because it
    /// runs a tick after the bond (via BondOutcomeComponent), outside the bonding
    /// job's cleanup, where starting a new job is safe.
    ///
    /// Wild insect momos are unaffected: bonding with her tamer still tames her and
    /// she still follows him home, because a wild momo is factionless and the
    /// faction half of the gate never matches.
    /// </summary>
    [HarmonyPatch(typeof(TsugaiFormation), nameof(TsugaiFormation.ExecuteJoin))]
    public static class Patch_HiveMomoNeverJoins
    {
        /// <summary>
        /// Skip this patch if the core ever renames or removes the method. Without
        /// the guard a missing target makes PatchAll throw on load, which would take
        /// every other patch in this assembly down with it.
        /// </summary>
        [HarmonyPrepare]
        public static bool Prepare()
        {
            return AccessTools.Method(typeof(TsugaiFormation), nameof(TsugaiFormation.ExecuteJoin)) != null;
        }

        public static bool Prefix(Pawn momo, Pawn man)
        {
            if (!HiveInsectMomo.IsHiveInsectMomo(momo))
            {
                return true; // every other Momo: the core's join roll, unchanged
            }

            // She cannot take a man she cannot carry. If either of them is down,
            // dead or gone, do nothing at all — skipping the join is the point, and
            // a kidnap would only refuse in the same cases.
            if (momo.Dead || momo.Downed || !momo.Spawned || man == null || man.Dead || !man.Spawned)
            {
                return false;
            }

            // No route off the map: leave her with the hive rather than joining the
            // colony she was fighting. Checked before the lord is touched, so a
            // failed kidnap never leaves her without her lord.
            if (!RCellFinder.TryFindBestExitSpot(momo, out IntVec3 exitSpot, TraverseMode.ByPawn, true))
            {
                return false;
            }

            // Her hive lord would keep issuing duties and fight the Kidnap job for
            // her, so detach her first and let a kidnap lord drive the job.
            momo.GetLord()?.RemovePawn(momo);

            StartAbductJob(momo, man, exitSpot);
            return false; // never ExecuteJoin
        }

        /// <summary>
        /// This mod's own start for a hive kidnapping: the same vanilla job and lord
        /// the core mod builds, with our own letter. The core's letter promises a
        /// rescue that can never happen for the insect geneline, and the game's own
        /// hostage letter promises the man is "not lost forever" — neither is true
        /// here, so neither is used.
        /// </summary>
        private static void StartAbductJob(Pawn momo, Pawn man, IntVec3 exitSpot)
        {
            Faction taker = momo.Faction;
            if (taker == null || momo.Map == null || momo.jobs == null)
            {
                return;
            }

            // She kidnaps for her own people, so she needs a lord whose job allows
            // the kidnapping — the same vanilla lord the core mod builds.
            if (momo.GetLord() == null)
            {
                LordMaker.MakeNewLord(taker, new LordJob_Kidnap(), momo.Map, new List<Pawn> { momo });
            }

            Job kidnap = JobMaker.MakeJob(JobDefOf.Kidnap, man, exitSpot);
            kidnap.count = 1;
            momo.jobs.StartJob(kidnap, JobCondition.InterruptForced);

            Find.LetterStack.ReceiveLetter(
                $"{momo.LabelShortCap} takes {man.LabelShort}",
                $"{momo.LabelShortCap} bonded with {man.LabelShort} and is dragging him away to the hive. The insect hive keeps no prisoners — once she reaches the map edge, {man.LabelShort} is gone for good.\n\nStop her before she escapes.",
                LetterDefOf.ThreatBig,
                new LookTargets(momo, man));
        }
    }
}
