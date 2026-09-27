using System.Collections.Generic;
using RimWorld;
using Verse;

namespace PMM_Insects
{
    /// <summary>
    /// Insectoids are fed by the hive, so are insect Mamonos. While she stands near a nest
    /// (a Hive building), her hunger and mana creep back up - the same way the
    /// hive "sustains" its insects. The refill is gradual, not instant: the rates
    /// below are per day, and XML can override them per race. Attached to each
    /// insect-mamono race def in Races_InsectMamono_BS.xml.
    ///
    /// Ticks rarely (every 600 ticks / 10 game-seconds) and cheaply: it only
    /// queries the map's hive list when the pawn is alive, spawned, and not
    /// already full on both needs. At the default rates a mamono parked by the nest
    /// refills an empty bar in about a day.
    /// </summary>
    public class CompProperties_HiveNourishment : CompProperties
    {
        /// <summary>Radius (in cells) around a Hive within which a mamono is nourished.</summary>
        public float radius = 12f;

        /// <summary>Interval between top-ups, in ticks.</summary>
        public int intervalTicks = 600;

        /// <summary>
        /// Hunger saturation fed per full day spent near a hive. 1 is one whole bar.
        /// Raise it in XML if the hive should feed her faster.
        /// </summary>
        public float foodPerDay = 1f;

        /// <summary>
        /// Mana fed per full day spent near a hive. Need_Mana drains a whole bar in about
        /// two days, so anything above 0.5 is a net gain. 1.5 lifts her from empty to full
        /// in about a day and a half.
        /// </summary>
        public float manaPerDay = 1.5f;

        public CompProperties_HiveNourishment()
        {
            compClass = typeof(CompHiveNourishment);
        }
    }

    public class CompHiveNourishment : ThingComp
    {
        private CompProperties_HiveNourishment Props => (CompProperties_HiveNourishment)props;

        private static List<ThingDef> hiveDefs;

        /// <summary>
        /// Every hive-like def we know of, resolved once. VFE Insectoids 2 adds four
        /// natural hives (Kemian, Chelis, Xanides, Nuchadus) and all four set thingClass
        /// to Hive, so the class test finds them exactly the way VFEI2's own
        /// Utils.allHiveDefs does. Its artificial hives are a different family
        /// (VFEInsectoids.ArtificialHive is a ThingWithComps, not a Hive), so those are
        /// matched by class name - which keeps VFEI2 a load-order dependency instead of
        /// a compile-time reference we would take just to read a static list.
        /// </summary>
        private static List<ThingDef> HiveDefs()
        {
            if (hiveDefs != null)
            {
                return hiveDefs;
            }
            hiveDefs = new List<ThingDef>();
            foreach (ThingDef def in DefDatabase<ThingDef>.AllDefs)
            {
                if (def.thingClass == null)
                {
                    continue;
                }
                if (typeof(Hive).IsAssignableFrom(def.thingClass)
                    || def.thingClass.Name.Contains("ArtificialHive"))
                {
                    hiveDefs.Add(def);
                }
            }
            return hiveDefs;
        }

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
            // Both bars already full? Nothing to do, and no reason to walk the map's
            // hive list either.
            Need_Food food = pawn.needs?.food;
            // Mana is the core mamono need; only present on Mamono-carriers.
            Need mana = pawn.needs?.TryGetNeed(ProjectMamono.ProjectMamono_DefOf.ProjectMamono_Mana);
            bool foodFull = food == null || food.CurLevel >= food.MaxLevel;
            bool manaFull = mana == null || mana.CurLevel >= mana.MaxLevel;
            if (foodFull && manaFull)
            {
                return;
            }

            if (!NearHive(pawn))
            {
                return;
            }

            // Both needs creep up instead of snapping to full, so lingering by the nest
            // is what pays. The props are rates per day, so scale one interval's worth.
            float intervalDays = Props.intervalTicks / 60000f;
            Refill(food, Props.foodPerDay * intervalDays);
            Refill(mana, Props.manaPerDay * intervalDays);
        }

        /// <summary>Adds <paramref name="amount"/> to a need, never past its maximum.</summary>
        private static void Refill(Need need, float amount)
        {
            if (need == null || amount <= 0f)
            {
                return;
            }
            float target = need.CurLevel + amount;
            need.CurLevel = target > need.MaxLevel ? need.MaxLevel : target;
        }

        private bool NearHive(Pawn pawn)
        {
            // One entry per hive def, so a mamono is fed by a VFEI2 nest just as well as
            // by vanilla's Hive. ThingsOfDef is a dictionary lookup, so an absent def
            // ("the player has no Kemian hive") costs nothing.
            foreach (ThingDef def in HiveDefs())
            {
                foreach (Thing hive in pawn.Map.listerThings.ThingsOfDef(def))
                {
                    if (hive.Position.InHorDistOf(pawn.Position, Props.radius))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
