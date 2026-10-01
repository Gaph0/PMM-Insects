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

        /// <summary>
        /// Honey from one milking, at full fullness. Halved from 25 at the user's call
        /// (2026-09-29); 25 does not halve into whole jars, so this is the rounded-down 12.
        /// </summary>
        public int honeyPerMilking = 12;

        /// <summary>
        /// How much must build up before there is anything worth taking. Three quarters, raised
        /// from a third at the user's call (2026-09-29): she carries her honey further before
        /// emptying, so one milking takes more at once. This same dial gates her own milking work
        /// (WorkGiver_MilkHoney), so she holds her honey rather than taking it early.
        /// </summary>
        public float minFullness = 0.75f;

        /// <summary>
        /// What one tracked work action is worth. Fullness is clamped at 1, so this number is also
        /// the build rate: 0.024 puts a full bar at about 42 sown plants, harvested plants or
        /// tamed animals, where 0.03 put it at 33. Lowered from 0.03 at the user's call
        /// (2026-09-29), which is the same load costing a quarter more work. Farming is what she
        /// does anyway; the honey is the by-product of it.
        /// </summary>
        public float fullnessPerAction = 0.024f;
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
    ///
    /// It implements `IPawnHarvest`, the shape both harvest chains share, so the right-click order,
    /// the job driver and the self-only work giver live once in Source/Insects/PawnHarvest.cs.
    /// </summary>
    public class Gene_Honey : Gene, IPawnHarvest
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
        public ThingDef YieldDef => Dials != null ? Dials.honeyDef : null;

        public float MinFullness => Dials != null ? Dials.minFullness : 0.75f;

        public int HoneyPerMilking => Dials != null ? Dials.honeyPerMilking : 12;

        private float FullnessPerAction => Dials != null ? Dials.fullnessPerAction : 0.024f;

        /// <summary>
        /// True when there is honey to take. The def check is not decoration: `honeyDef` is what the
        /// Medieval Overhaul patch rewrites, and the extension itself can be missing.
        /// </summary>
        public bool CanHarvest => fullness >= MinFullness && YieldDef != null;

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
        /// every caller checks <see cref="CanHarvest"/> first.
        /// </summary>
        public int HarvestNow()
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
                Disabled = !CanHarvest,
                disabledReason = "PMM_HoneyNoneYet".Translate().Resolve(),
            };
        }

        /// <summary>
        /// The honey item's own art, so the order reads as the thing it produces. Read off the def
        /// actually in play, so a load order with Medieval Overhaul gets MO's honey picture on the
        /// button along with MO's honey in her hands.
        /// </summary>
        private Texture2D HoneyGizmoIcon => YieldDef?.uiIcon;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref fullness, "honeyFullness", 0f);
            Scribe_Values.Look(ref lastWorkTotal, "honeyLastWorkTotal", -1);
        }
    }

    /// <summary>
    /// Puts "Milk honey" on the right-click menu of a honey-making mamono - for any of the player's
    /// pawns, including the bee herself, who is perfectly able to do it with her own hands. The
    /// option itself is built once for both harvest chains, in `PawnHarvestOrder`.
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

            FloatMenuOption milk = PawnHarvestOrder.BuildOption(__instance, selPawn,
                InsectDefOf.PMM_MilkHoney, "PMM_HoneyMilkLabel", "PMM_HoneyNoneYet",
                target => Gene_Honey.Get(target));
            if (milk != null)
            {
                yield return milk;
            }
        }
    }

    /// <summary>
    /// The milking itself. The walk, the wait and the drop are shared with silk
    /// (`JobDriver_HarvestFromPawn`); this class says only where the honey lives.
    /// </summary>
    public class JobDriver_MilkHoney : JobDriver_HarvestFromPawn
    {
        protected override IPawnHarvest StateOn(Pawn pawn)
        {
            return Gene_Honey.Get(pawn);
        }
    }

    /// <summary>
    /// She takes her own honey. The scan, the skip and the job start are shared with silk
    /// (`WorkGiver_HarvestFromSelf`); this class says only where the honey lives and which job to
    /// start. The player's right-click order covers every other case, including a handler doing it
    /// for her.
    /// </summary>
    public class WorkGiver_MilkHoney : WorkGiver_HarvestFromSelf
    {
        protected override IPawnHarvest StateOn(Pawn pawn)
        {
            return Gene_Honey.Get(pawn);
        }

        protected override JobDef HarvestJob => InsectDefOf.PMM_MilkHoney;
    }
}
