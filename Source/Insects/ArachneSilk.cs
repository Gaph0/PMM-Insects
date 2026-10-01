using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace PMM_Insects
{
    /// <summary>
    /// Arachne silk: the resource she spins and the player takes (user's call 2026-09-26, gathered
    /// by a right click, and she can gather it herself). The state lives here. `fullness` fills over
    /// time, one gather empties it, and the yield scales with what had built up - so taking it early
    /// is a shorter wait for less silk, never a loss. Attached to PMM_Race_Arachne
    /// and PMM_Race_AntArachne in Races_InsectMamono_BS.xml. Everything that finds
    /// silk does it through this comp rather than through a race name - the work
    /// giver, the right-click order and the gizmo - so a new spinning caste is one
    /// comp in its race def and nothing else here.
    /// </summary>
    public class CompProperties_ArachneSilk : CompProperties
    {
        /// <summary>
        /// What a gather actually yields. Named in the race def, and re-pointed by
        /// Patches/ArachneSilk_MedievalOverhaul.xml, so a colony running Medieval Overhaul spins
        /// MO's own silk and there is no second silk in it.
        /// </summary>
        public ThingDef silkDef;

        /// <summary>Silk from one gather, at full fullness.</summary>
        public int silkPerGather = 25;

        /// <summary>Days from empty to full. The yield per day is this number divided by that one.</summary>
        public float growthDays = 3f;

        /// <summary>How much must build up before there is anything worth taking.</summary>
        public float minFullness = 0.3f;

        public CompProperties_ArachneSilk()
        {
            compClass = typeof(CompArachneSilk);
        }
    }

    /// <summary>
    /// The silk itself, on the woman who spins it: filled on the clock, emptied by one gather, with
    /// the yield scaled to what had built up. Implements `IPawnHarvest` - the shape both harvest
    /// chains share - so the right-click order, the job driver and the self-only work giver live
    /// once, in Source/Insects/PawnHarvest.cs.
    /// </summary>
    public class CompArachneSilk : ThingComp, IPawnHarvest
    {
        private float fullness;

        private CompProperties_ArachneSilk Props => (CompProperties_ArachneSilk)props;

        /// <summary>
        /// True when there is silk to take. The def check is not decoration: `silkDef` is the field
        /// the Medieval Overhaul patch rewrites, and a name that resolved to nothing would leave the
        /// gather building a thing out of a null def. Honey guards its own `honeyDef` for the same
        /// patch and the same reason.
        /// </summary>
        public bool CanHarvest => Props.silkDef != null && fullness >= Props.minFullness;

        /// <summary>What she spins. Named by the race def; the MO patch changes which def that is.</summary>
        public ThingDef YieldDef => Props.silkDef;

        public override void CompTickInterval(int delta)
        {
            base.CompTickInterval(delta);
            if (!(parent is Pawn pawn) || !pawn.Spawned || pawn.Dead)
            {
                return;
            }
            fullness = Mathf.Clamp01(fullness + delta / (Props.growthDays * GenDate.TicksPerDay));
        }

        /// <summary>
        /// Takes everything that has built up and empties her. Returns at least one unit, because
        /// every caller checks <see cref="CanHarvest"/> first.
        /// </summary>
        public int HarvestNow()
        {
            int amount = Mathf.Max(1, Mathf.RoundToInt(Props.silkPerGather * fullness));
            fullness = 0f;
            return amount;
        }

        public override string CompInspectStringExtra()
        {
            return "PMM_SilkFullness".Translate(fullness.ToStringPercent()).Resolve();
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref fullness, "silkFullness", 0f);
        }
    }

    /// <summary>
    /// Puts "Gather silk" on the right-click menu of an arachne - for any of the player's pawns,
    /// including the arachne herself, who is perfectly able to do it with her own hands. The option
    /// itself is built once for both harvest chains, in `PawnHarvestOrder`.
    /// </summary>
    [HarmonyPatch(typeof(Pawn), nameof(Pawn.GetFloatMenuOptions))]
    public static class ArachneSilkFloatMenuPatch
    {
        public static IEnumerable<FloatMenuOption> Postfix(IEnumerable<FloatMenuOption> __result, Pawn __instance, Pawn selPawn)
        {
            foreach (FloatMenuOption option in __result)
            {
                yield return option;
            }

            FloatMenuOption gather = PawnHarvestOrder.BuildOption(__instance, selPawn,
                InsectDefOf.PMM_GatherArachneSilk, "PMM_SilkGatherLabel", "PMM_SilkNoneYet",
                target => target.GetComp<CompArachneSilk>());
            if (gather != null)
            {
                yield return gather;
            }
        }
    }

    /// <summary>
    /// The gather itself. The walk, the wait and the drop are shared with honey
    /// (`JobDriver_HarvestFromPawn`); this class says only where the silk lives.
    /// </summary>
    public class JobDriver_GatherArachneSilk : JobDriver_HarvestFromPawn
    {
        protected override IPawnHarvest StateOn(Pawn pawn)
        {
            return pawn.GetComp<CompArachneSilk>();
        }
    }

    /// <summary>
    /// She gathers her own silk (user's call: "she can also do it herself"). The scan, the skip and
    /// the job start are shared with honey (`WorkGiver_HarvestFromSelf`); this class says only where
    /// the silk lives and which job to start.
    /// </summary>
    public class WorkGiver_ArachneSilk : WorkGiver_HarvestFromSelf
    {
        protected override IPawnHarvest StateOn(Pawn pawn)
        {
            return pawn.GetComp<CompArachneSilk>();
        }

        protected override JobDef HarvestJob => InsectDefOf.PMM_GatherArachneSilk;
    }
}
