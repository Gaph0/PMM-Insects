using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace PMM_Insects
{
    /// <summary>
    /// The harvest chain of the insect castes: the arachne's silk and the bee's honey were written
    /// one after the other and the two chains are the same shape - a right-click order, a job
    /// driver, and a self-only work giver. Everything about them is identical except where the state
    /// lives (silk on a comp in her race def, honey on a gene in her xenotype) and what comes off
    /// her, so that is all a feature supplies now.
    ///
    /// This interface is the seam: the owner of the state says whether there is anything to take,
    /// what it yields and how to empty itself, and the three pieces below are written once.
    /// </summary>
    public interface IPawnHarvest
    {
        /// <summary>True when there is something to take right now.</summary>
        bool CanHarvest { get; }

        /// <summary>
        /// What comes off her. Named by the def that owns the state, and re-pointed by a compat
        /// patch in both features (Medieval Overhaul's silk and honey), so a caller must be ready
        /// for it to be a def the feature did not ship.
        /// </summary>
        ThingDef YieldDef { get; }

        /// <summary>
        /// Takes everything that has built up and empties her. Callers check <see cref="CanHarvest"/>
        /// first, and the owner returns at least one unit for that reason.
        /// </summary>
        int HarvestNow();
    }

    /// <summary>
    /// The right-click order both features use: "Gather silk" on an arachne, "Milk honey" on a bee,
    /// offered on any of the player's pawns - including the woman herself, who is perfectly able to
    /// do it with her own hands. Mirrors core's CorruptionFloatMenuPatch: same hook, same
    /// decoration, and greyed with a reason rather than hidden when there is nothing to take yet.
    /// </summary>
    public static class PawnHarvestOrder
    {
        /// <summary>
        /// The option, or null when it does not apply. `stateLookup` is the feature's own way of
        /// finding its state on the target, and is not called until the cheaper checks have passed.
        /// </summary>
        public static FloatMenuOption BuildOption(Pawn target, Pawn worker, JobDef job, string labelKey,
            string noneKey, Func<Pawn, IPawnHarvest> stateLookup)
        {
            if (target == null || worker == null || !worker.Spawned || !target.Spawned || worker.Map != target.Map)
            {
                return null;
            }
            // Yours at both ends: your own hand, and one of your own women. A hive sister or a wild
            // one has no reason to stand still for it, and cannot be ordered anyway.
            if (worker.Faction != Faction.OfPlayer || !worker.IsColonistPlayerControlled)
            {
                return null;
            }
            if (target.Faction != Faction.OfPlayer)
            {
                return null;
            }

            IPawnHarvest state = stateLookup(target);
            if (state == null)
            {
                return null;
            }

            string label = labelKey.Translate().Resolve();
            if (!state.CanHarvest)
            {
                return new FloatMenuOption(label + " (" + noneKey.Translate().Resolve() + ")", null);
            }

            return FloatMenuUtility.DecoratePrioritizedTask(
                new FloatMenuOption(label, delegate
                {
                    // Re-checked on click: she may have been harvested from since the menu was built.
                    if (state.CanHarvest)
                    {
                        worker.jobs.TryTakeOrderedJob(JobMaker.MakeJob(job, target), JobTag.Misc);
                    }
                }),
                worker,
                target);
        }
    }

    /// <summary>
    /// The harvest itself: a short wait with a progress bar, then the yield lands on the ground
    /// beside her. One driver for both paths - a second pair of hands taking it, and her own -
    /// shared by both features, which differ only in where the state lives.
    /// </summary>
    public abstract class JobDriver_HarvestFromPawn : JobDriver
    {
        private const int HarvestTicks = 400;

        private Pawn Target => TargetA.Thing as Pawn;

        /// <summary>This feature's state on that pawn, or null when she has none.</summary>
        protected abstract IPawnHarvest StateOn(Pawn pawn);

        private IPawnHarvest State => Target == null ? null : StateOn(Target);

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return true;
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOn(() => State == null || !State.CanHarvest);

            // Nothing to walk to when she is doing it herself.
            if (pawn != Target)
            {
                yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
            }

            Toil wait = Toils_General.Wait(HarvestTicks, TargetIndex.A);
            wait.WithProgressBarToilDelay(TargetIndex.A);
            yield return wait;

            yield return Toils_General.Do(delegate
            {
                IPawnHarvest state = State;
                Pawn target = Target;
                if (state == null || target == null || !state.CanHarvest)
                {
                    return;
                }

                Thing product = ThingMaker.MakeThing(state.YieldDef);
                product.stackCount = state.HarvestNow();
                GenPlace.TryPlaceThing(product, target.Position, target.Map, ThingPlaceMode.Near);
            });
        }
    }

    /// <summary>
    /// She takes her own, on the model of vanilla's self-tend giver (`WorkGiver_TendSelf`, the class
    /// behind the `DoctorTendToSelf` def): the only possible target is the pawn herself, and she is
    /// handed to the job giver directly through `PotentialWorkThingsGlobal`. The request group is
    /// `Undefined` for the same reason vanilla sets it there - a request of her own is answered by
    /// that list, not by walking the pawns of the map.
    /// </summary>
    public abstract class WorkGiver_HarvestFromSelf : WorkGiver_Scanner
    {
        /// <summary>This feature's state on that pawn, or null when she has none.</summary>
        protected abstract IPawnHarvest StateOn(Pawn pawn);

        /// <summary>The job that does the harvest, which the feature's own def names.</summary>
        protected abstract JobDef HarvestJob { get; }

        public override ThingRequest PotentialWorkThingRequest => ThingRequest.ForGroup(ThingRequestGroup.Undefined);

        public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn)
        {
            return Gen.YieldSingle<Thing>(pawn);
        }

        public override bool ShouldSkip(Pawn pawn, bool forced = false)
        {
            return !(pawn != null && (StateOn(pawn)?.CanHarvest ?? false));
        }

        public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            return t == pawn && (StateOn(pawn)?.CanHarvest ?? false);
        }

        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            return JobMaker.MakeJob(HarvestJob, pawn);
        }
    }
}
