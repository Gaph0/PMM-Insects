using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace PMM_Insects
{
    /// <summary>
    /// The dials for the honey gene (user's plan 2026-09-27). A DefModExtension because `GeneDef`
    /// has no comps field, the same reason the core mod's age-ailment marker is an extension.
    ///
    /// The state itself lives on the gene (see <see cref="Gene_Honey"/>), not on a race, so any
    /// caste that carries the gene makes honey and everything that finds her - the work giver, the
    /// right-click order, the gizmo - reaches it through the gene rather than through a race name.
    /// That is the rule the arachne's silk comp states for itself.
    /// </summary>
    public class HoneyGeneExtension : DefModExtension
    {
        /// <summary>
        /// What one milking yields. Named in the gene def, and re-pointed by
        /// Patches/Honey_MedievalOverhaul.xml, so a colony running Medieval Overhaul gets MO's own
        /// honey and there is no second honey in it.
        /// </summary>
        public ThingDef honeyDef;

        /// <summary>Honey from one milking, at full fullness.</summary>
        public int honeyPerMilking = 25;

        /// <summary>How much must build up before there is anything worth taking.</summary>
        public float minFullness = 0.3f;

        /// <summary>
        /// What one tracked work action is worth. Fullness is clamped at 1, so this number is also
        /// the build rate: 0.03 puts a full bar at about 33 sown plants, harvested plants or tamed
        /// animals. Farming is what she does anyway; the honey is the by-product of it.
        /// </summary>
        public float fullnessPerAction = 0.03f;
    }

    /// <summary>
    /// Honey: the resource she makes while she works, and the player takes.
    ///
    /// The one place this differs from the arachne's silk is where `fullness` comes from. Silk
    /// fills on the clock (`growthDays`); honey fills from what she DOES. The hook is the pawn's own
    /// work records - vanilla keeps a per-pawn tally of plants sown, plants harvested and animals
    /// tamed, among others - polled once an hour and diffed against the last reading. No Harmony
    /// patch is needed for any of it, and the records are looked up BY NAME, so a record that is
    /// absent in this load order is simply not tracked.
    ///
    /// Milking empties her, and the yield scales with what had built up, so taking it early is a
    /// shorter wait for less honey and never a loss - the same bargain the silk comp makes.
    /// </summary>
    public class Gene_Honey : Gene
    {
        /// <summary>One in-game hour. Records only change when she finishes work, so this is often enough.</summary>
        private const int PollIntervalTicks = 2500;

        /// <summary>
        /// The vanilla work records worth honey, verified in
        /// Data/Core/Defs/Misc/RecordDefs/Records_Misc.xml on 2026-09-27. There is no record for
        /// milking or training an animal - the list holds `AnimalsTamed` and `AnimalsSlaughtered`
        /// and nothing finer - so animal work counts through taming alone; the plan named milking
        /// too, and that record does not exist to name.
        /// </summary>
        private static readonly string[] WorkRecordDefNames = { "PlantsSown", "PlantsHarvested", "AnimalsTamed" };

        private static List<RecordDef> trackedRecords;

        private float fullness;

        /// <summary>
        /// The sum of every tracked record at the last poll, or -1 before the first one. Only
        /// increases, so one running total is enough to diff.
        /// </summary>
        private int lastWorkTotal = -1;

        private HoneyGeneExtension Dials => def.GetModExtension<HoneyGeneExtension>();

        /// <summary>
        /// What she makes. Falls back to the extension's own defaults when the extension is
        /// missing, so a half-written gene def cannot throw on the pawn's tick.
        /// </summary>
        public ThingDef HoneyDef => Dials != null ? Dials.honeyDef : null;

        public float Fullness => fullness;

        public float MinFullness => Dials != null ? Dials.minFullness : 0.3f;

        public int HoneyPerMilking => Dials != null ? Dials.honeyPerMilking : 25;

        private float FullnessPerAction => Dials != null ? Dials.fullnessPerAction : 0.03f;

        /// <summary>True when there is honey to take.</summary>
        public bool CanMilk => fullness >= MinFullness && HoneyDef != null;

        /// <summary>
        /// The honey gene on this pawn, or null. Every order path finds her this way, so a new
        /// honey-making caste is one gene in her xenotype and nothing else in this file.
        /// </summary>
        public static Gene_Honey Get(Pawn pawn)
        {
            if (pawn?.genes == null)
            {
                return null;
            }
            foreach (Gene gene in pawn.genes.GenesListForReading)
            {
                if (gene is Gene_Honey honey && gene.Active)
                {
                    return honey;
                }
            }
            return null;
        }

        private static List<RecordDef> TrackedRecords
        {
            get
            {
                if (trackedRecords == null)
                {
                    trackedRecords = new List<RecordDef>();
                    foreach (string recordName in WorkRecordDefNames)
                    {
                        RecordDef record = DefDatabase<RecordDef>.GetNamedSilentFail(recordName);
                        if (record != null)
                        {
                            trackedRecords.Add(record);
                        }
                    }
                }
                return trackedRecords;
            }
        }

        public override void Tick()
        {
            base.Tick();

            if (pawn == null || pawn.Dead || !pawn.Spawned)
            {
                return;
            }
            if (pawn.IsHashIntervalTick(PollIntervalTicks))
            {
                PollWork();
            }
        }

        /// <summary>
        /// Reads the tracked records and pays for what grew since the last reading. The very first
        /// reading only baselines: a pawn loaded from a save carries a lifetime of farm work on her
        /// records already, and paying for that would hand her a full bar the moment she loads.
        /// </summary>
        private void PollWork()
        {
            int total = 0;
            foreach (RecordDef record in TrackedRecords)
            {
                total += pawn.records.GetAsInt(record);
            }

            if (lastWorkTotal < 0)
            {
                lastWorkTotal = total;
                return;
            }

            int gained = total - lastWorkTotal;
            lastWorkTotal = total;

            if (gained > 0)
            {
                fullness = Mathf.Clamp01(fullness + gained * FullnessPerAction);
            }
        }

        /// <summary>
        /// Takes everything that has built up and empties her. Returns at least one unit, because
        /// every caller checks <see cref="CanMilk"/> first.
        /// </summary>
        public int MilkNow()
        {
            int amount = Mathf.Max(1, Mathf.RoundToInt(HoneyPerMilking * fullness));
            fullness = 0f;
            return amount;
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            // No `base.GetGizmos()` call anywhere in here: `Verse.Gene`'s own implementation returns
            // null rather than an empty list, so there is nothing to pass through.

            // Hers alone, and only for the player: the right-click order already covers telling
            // someone else to take it. `Verse.Gene.GetGizmos` is the engine's own hook for this, so
            // the order sits on her own bar with no patch anywhere.
            if (pawn == null || !pawn.Spawned || !pawn.IsColonistPlayerControlled)
            {
                yield break;
            }

            yield return new Command_Action
            {
                defaultLabel = "PMM_HoneyMilkLabel".Translate().Resolve(),
                defaultDesc = "PMM_HoneyFullness".Translate(fullness.ToStringPercent()).Resolve(),
                icon = HoneyGizmoIcon,
                action = delegate
                {
                    pawn.jobs.TryTakeOrderedJob(
                        JobMaker.MakeJob(InsectDefOf.PMM_MilkHoney, pawn), JobTag.Misc);
                },
                Disabled = !CanMilk,
                disabledReason = "PMM_HoneyNoneYet".Translate().Resolve(),
            };
        }

        /// <summary>
        /// The honey item's own art, so the order reads as the thing it produces. Vanilla insect
        /// jelly's texture, borrowed the same way the silk item borrows vanilla cloth's.
        /// </summary>
        private static Texture2D HoneyGizmoIcon => ContentFinder<Texture2D>.Get("Things/Item/Resource/AnimalProductRaw/InsectJelly");

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref fullness, "honeyFullness", 0f);
            Scribe_Values.Look(ref lastWorkTotal, "honeyLastWorkTotal", -1);
        }
    }

    /// <summary>
    /// Puts "Milk honey" on the right-click menu of a honey-making mamono for any of the player's
    /// pawns - including the bee herself, who is perfectly able to do it with her own hands.
    /// Mirrors the arachne's silk order: same hook, same decoration, and greyed with a reason
    /// rather than hidden when there is nothing to take yet.
    /// </summary>
    [HarmonyPatch(typeof(Pawn), nameof(Pawn.GetFloatMenuOptions))]
    public static class HoneyFloatMenuPatch
    {
        public static IEnumerable<FloatMenuOption> Postfix(IEnumerable<FloatMenuOption> __result, Pawn __instance, Pawn selPawn)
        {
            foreach (FloatMenuOption option in __result)
            {
                yield return option;
            }

            FloatMenuOption milk = BuildOption(__instance, selPawn);
            if (milk != null)
            {
                yield return milk;
            }
        }

        private static FloatMenuOption BuildOption(Pawn target, Pawn worker)
        {
            if (target == null || worker == null || !worker.Spawned || !target.Spawned || worker.Map != target.Map)
            {
                return null;
            }
            // Yours at both ends: your own hand, and a mamono of yours. A hive bee has no reason to
            // stand still for it.
            if (worker.Faction != Faction.OfPlayer || !worker.IsColonistPlayerControlled)
            {
                return null;
            }
            if (target.Faction != Faction.OfPlayer)
            {
                return null;
            }
            if (!(Gene_Honey.Get(target) is Gene_Honey honey))
            {
                return null;
            }

            string label = "PMM_HoneyMilkLabel".Translate().Resolve();
            if (!honey.CanMilk)
            {
                return new FloatMenuOption(label + " (" + "PMM_HoneyNoneYet".Translate().Resolve() + ")", null);
            }

            return FloatMenuUtility.DecoratePrioritizedTask(
                new FloatMenuOption(label, delegate
                {
                    // Re-checked on click: someone may have milked her since the menu was built.
                    if (honey.CanMilk)
                    {
                        worker.jobs.TryTakeOrderedJob(JobMaker.MakeJob(InsectDefOf.PMM_MilkHoney, target), JobTag.Misc);
                    }
                }),
                worker,
                target);
        }
    }

    /// <summary>
    /// The milking itself: a short wait with a progress bar, then the honey lands on the ground
    /// beside her. One driver for both paths - a handler taking it, and the bee doing her own.
    /// </summary>
    public class JobDriver_MilkHoney : JobDriver
    {
        private const int MilkTicks = 400;

        private Pawn Target => TargetA.Thing as Pawn;

        private Gene_Honey Honey => Gene_Honey.Get(Target);

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return true;
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOn(() => Honey == null || !Honey.CanMilk);

            // Nothing to walk to when she is doing it herself.
            if (pawn != Target)
            {
                yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
            }

            Toil wait = Toils_General.Wait(MilkTicks, TargetIndex.A);
            wait.WithProgressBarToilDelay(TargetIndex.A);
            yield return wait;

            yield return Toils_General.Do(delegate
            {
                Gene_Honey honey = Honey;
                Pawn target = Target;
                if (honey == null || target == null || !honey.CanMilk)
                {
                    return;
                }

                Thing jar = ThingMaker.MakeThing(honey.HoneyDef);
                jar.stackCount = honey.MilkNow();
                GenPlace.TryPlaceThing(jar, target.Position, target.Map, ThingPlaceMode.Near);
            });
        }
    }

    /// <summary>
    /// She takes her own honey, on the model of vanilla's SelfTend: a work giver whose only
    /// possible target is the pawn herself. The player's right-click order covers every other
    /// case, including a handler doing it for her.
    /// </summary>
    public class WorkGiver_MilkHoney : WorkGiver_Scanner
    {
        public override ThingRequest PotentialWorkThingRequest => ThingRequest.ForGroup(ThingRequestGroup.Pawn);

        public override bool ShouldSkip(Pawn pawn, bool forced = false)
        {
            return !(Gene_Honey.Get(pawn)?.CanMilk ?? false);
        }

        public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            return t == pawn && (Gene_Honey.Get(pawn)?.CanMilk ?? false);
        }

        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            return JobMaker.MakeJob(InsectDefOf.PMM_MilkHoney, pawn);
        }
    }
}
