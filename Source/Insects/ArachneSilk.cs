using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace PMM_Insects
{
    [DefOf]
    public static class InsectDefOf
    {
        /// <summary>The job that gathers the silk, from her own hands or another's.</summary>
        public static JobDef PMM_GatherArachneSilk;
    }

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

    public class CompArachneSilk : ThingComp
    {
        private float fullness;

        private CompProperties_ArachneSilk Props => (CompProperties_ArachneSilk)props;

        /// <summary>True when there is silk to take.</summary>
        public bool CanGather => fullness >= Props.minFullness;

        /// <summary>What she spins. Named by the race def; the MO patch changes which def that is.</summary>
        public ThingDef SilkDef => Props.silkDef;

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
        /// every caller checks <see cref="CanGather"/> first.
        /// </summary>
        public int GatherNow()
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
    /// Puts "Gather silk" on the right-click menu of an arachne for any of the player's pawns -
    /// including the arachne herself, who is perfectly able to do it with her own hands. Mirrors
    /// core's CorruptionFloatMenuPatch: same hook, same decoration, and greyed with a reason rather
    /// than hidden when there is nothing to take yet.
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

            FloatMenuOption gather = BuildOption(__instance, selPawn);
            if (gather != null)
            {
                yield return gather;
            }
        }

        private static FloatMenuOption BuildOption(Pawn target, Pawn worker)
        {
            if (target == null || worker == null || !worker.Spawned || !target.Spawned || worker.Map != target.Map)
            {
                return null;
            }
            // Yours at both ends: your own hand, and an arachne of yours. A nest arachne has no
            // reason to stand still for it.
            if (worker.Faction != Faction.OfPlayer || !worker.IsColonistPlayerControlled)
            {
                return null;
            }
            if (target.Faction != Faction.OfPlayer)
            {
                return null;
            }
            if (!(target.GetComp<CompArachneSilk>() is CompArachneSilk silk))
            {
                return null;
            }

            string label = "PMM_SilkGatherLabel".Translate().Resolve();
            if (!silk.CanGather)
            {
                return new FloatMenuOption(label + " (" + "PMM_SilkNoneYet".Translate().Resolve() + ")", null);
            }

            return FloatMenuUtility.DecoratePrioritizedTask(
                new FloatMenuOption(label, delegate
                {
                    // Re-checked on click: she may have been gathered from since the menu was built.
                    if (silk.CanGather)
                    {
                        worker.jobs.TryTakeOrderedJob(JobMaker.MakeJob(InsectDefOf.PMM_GatherArachneSilk, target), JobTag.Misc);
                    }
                }),
                worker,
                target);
        }
    }

    /// <summary>
    /// The gather itself: a short wait with a progress bar, then the silk lands on the ground beside
    /// her. One driver for both paths - a handler taking it, and an arachne working her own.
    /// </summary>
    public class JobDriver_GatherArachneSilk : JobDriver
    {
        private const int GatherTicks = 400;

        private Pawn Target => TargetA.Thing as Pawn;

        private CompArachneSilk Silk => Target?.GetComp<CompArachneSilk>();

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return true;
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOn(() => Silk == null || !Silk.CanGather);

            // Nothing to walk to when she is doing it herself.
            if (pawn != Target)
            {
                yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
            }

            Toil wait = Toils_General.Wait(GatherTicks, TargetIndex.A);
            wait.WithProgressBarToilDelay(TargetIndex.A);
            yield return wait;

            yield return Toils_General.Do(delegate
            {
                CompArachneSilk silk = Silk;
                Pawn target = Target;
                if (silk == null || target == null || !silk.CanGather)
                {
                    return;
                }

                Thing bundle = ThingMaker.MakeThing(silk.SilkDef);
                bundle.stackCount = silk.GatherNow();
                GenPlace.TryPlaceThing(bundle, target.Position, target.Map, ThingPlaceMode.Near);
            });
        }
    }

    /// <summary>
    /// She gathers her own silk (user's call: "she can also do it herself"), on the model of
    /// vanilla's SelfTend: a work giver whose only possible target is the pawn herself.
    /// </summary>
    public class WorkGiver_ArachneSilk : WorkGiver_Scanner
    {
        public override ThingRequest PotentialWorkThingRequest => ThingRequest.ForGroup(ThingRequestGroup.Pawn);

        public override bool ShouldSkip(Pawn pawn, bool forced = false)
        {
            return !(pawn?.GetComp<CompArachneSilk>()?.CanGather ?? false);
        }

        public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            return t == pawn && (pawn?.GetComp<CompArachneSilk>()?.CanGather ?? false);
        }

        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            return JobMaker.MakeJob(InsectDefOf.PMM_GatherArachneSilk, pawn);
        }
    }
}
