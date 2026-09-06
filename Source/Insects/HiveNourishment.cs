using RimWorld;
using Verse;

namespace PMM_Insects
{
    /// <summary>
    /// A daughter of the swarm is fed by the hive. While she stands near a nest
    /// (a Hive building), her hunger and mana are topped up — the same way the
    /// hive sustains its insects. Attached to every insect-momo race via the
    /// shared PMM_InsectMomoBase abstract ThingDef.
    ///
    /// Ticks rarely (every 600 ticks / 10 game-seconds) and cheaply: it only
    /// queries the map's hive list when the pawn is alive, spawned, and not
    /// already full on both needs.
    /// </summary>
    public class CompProperties_HiveNourishment : CompProperties
    {
        /// <summary>Radius (in cells) around a Hive within which a momo is nourished.</summary>
        public float radius = 12f;

        /// <summary>Interval between top-ups, in ticks.</summary>
        public int intervalTicks = 600;

        public CompProperties_HiveNourishment()
        {
            compClass = typeof(CompHiveNourishment);
        }
    }

    public class CompHiveNourishment : ThingComp
    {
        private CompProperties_HiveNourishment Props => (CompProperties_HiveNourishment)props;

        private static ThingDef hiveDef;

        // Countdown to the next top-up. CompTickRare fires every 250 ticks, and 600
        // is not a multiple of 250, so a "TicksGame % interval == 0" gate would almost
        // never fire. Count down in rare-tick steps instead.
        private int ticksUntilNext;

        public override void CompTickRare()
        {
            base.CompTickRare();
            ticksUntilNext -= 250; // CompTickRare interval
            if (ticksUntilNext > 0)
            {
                return;
            }
            ticksUntilNext = Props.intervalTicks;
            if (!(parent is Pawn pawn) || pawn.Dead || !pawn.Spawned || pawn.Map == null)
            {
                return;
            }
            if (!NearHive(pawn))
            {
                return;
            }

            // Hunger.
            Need_Food food = pawn.needs?.food;
            if (food != null && food.CurLevelPercentage < 1f)
            {
                food.CurLevelPercentage = 1f;
            }

            // Mana (the core momo need; only present on Momo-carriers).
            Need mana = pawn.needs?.TryGetNeed(ProjectMomo.ProjectMomo_DefOf.ProjectMomo_Mana);
            if (mana != null && mana.CurLevelPercentage < 1f)
            {
                mana.CurLevelPercentage = 1f;
            }
        }

        private bool NearHive(Pawn pawn)
        {
            if (hiveDef == null)
            {
                hiveDef = ThingDef.Named("Hive");
            }
            foreach (Thing hive in pawn.Map.listerThings.ThingsOfDef(hiveDef))
            {
                if (hive.Position.InHorDistOf(pawn.Position, Props.radius))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
