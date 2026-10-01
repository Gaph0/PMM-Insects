using ProjectMamono;
using RimWorld;
using Verse;

namespace PMM_Insects
{
    /// <summary>
    /// The greenworm's maturation into a papillon (CASTES-PLAN.md §7).
    ///
    /// The lore was already in the mod before this file was: a greenworm who takes in enough mana
    /// pupates into a papillon. What was missing was the machinery, and the shape of it is the four
    /// decisions the user locked on 2026-09-27:
    ///
    ///   1. One way. A papillon never goes back to a greenworm, so this comp only ever fires in one
    ///      direction and the papillon's own race does not carry it.
    ///   2. Mana is the fuel, and spent mana becomes charges. The comp watches the mana bar and
    ///      counts every FALL as mana spent; refills never subtract. One rule covers both spenders -
    ///      her passive drain and giving essence away - and it is why a nest keeps her a grub:
    ///      `CompHiveNourishment` tops her up at 1.5 bars a day, so charges do not accrue while she
    ///      sits by the hive. That is deliberate, not a bug.
    ///   3. Colony greenworms only. Off-map village pawns never tick, so they are already safe, and
    ///      `requireColonist` keeps a spawned raider from pupating mid-raid.
    ///   4. A cocoon, 15 days, reusing the abaddon egg sac's own art.
    ///
    /// A hediff cannot render on its own, so the cocoon is a building on her tile
    /// (`CompPapillonCocoon`, `Defs/ThingDefs/Things_Cocoon.xml`) with an immobility hediff holding
    /// her still for the duration. Destroy that cocoon before its time and she dies with it - the
    /// user's ruling, 2026-10-02.
    ///
    /// The swap itself is core's, not ours: `MamonoTransformation.ConvertXenotype` is the
    /// mamono-to-mamono path, written for exactly this case and never called until now. It strips
    /// the old xenotype's signature endogenes, adds the new set, swaps the B&amp;S race and dirties
    /// the graphics, and it notifies the player on its own - which is why nothing here posts a
    /// message.
    /// </summary>
    public class CompProperties_PapillonMaturation : CompProperties
    {
        /// <summary>
        /// Mana that counts as one charge. The mana bar is a need, so its levels run 0 to 1: 0.05
        /// means one charge per twentieth of a bar spent, and `Need_Mana` drains a whole bar in
        /// about two days on its own.
        /// </summary>
        public float manaPerCharge = 0.05f;

        /// <summary>
        /// Charges she needs before the cocoon starts. At the defaults that is three full bars of
        /// spent mana - roughly six days away from a hive at her own drain rate, sooner if she is
        /// also giving essence away.
        /// </summary>
        public float chargesNeeded = 60f;

        /// <summary>How long the cocoon takes, in days.</summary>
        public int cocoonDays = 15;

        /// <summary>
        /// Only a colonist pupates. A raider or a village pawn never ticks anyway, and this is the
        /// belt to that pair of braces.
        /// </summary>
        public bool requireColonist = true;

        /// <summary>
        /// What she becomes, read as a defName from XML and looked up defensively: the comp has to
        /// ship dormant and harmless if the papillon is ever renamed, and a `[DefOf]` field pointing
        /// at a missing def is a load error.
        /// </summary>
        public string targetXenotype = "PMM_InsectPapillon";

        /// <summary>The cocoon spawned on her tile when her time comes.</summary>
        public string cocoonDef = "PMM_Cocoon";

        public CompProperties_PapillonMaturation()
        {
            compClass = typeof(CompPapillonMaturation);
        }
    }

    /// <summary>
    /// The greenworm's half of it: count the mana she spends, and start the cocoon when she has
    /// spent enough.
    ///
    /// Ticks rarely (every 250 ticks), like the hive feeder beside it on the race, and it reads one
    /// need and writes two floats, so a colony of grubs costs nothing.
    /// </summary>
    public class CompPapillonMaturation : ThingComp
    {
        /// <summary>Charges banked so far. Scribed, so a reload cannot lose her progress.</summary>
        private float charges;

        /// <summary>
        /// The mana level the last tick saw, so a fall can be measured. -1 means "not seen yet": the
        /// first tick only records the level, because charging her for a bar she started with would
        /// make a fresh grub half-matured.
        /// </summary>
        private float lastMana = -1f;

        private CompProperties_PapillonMaturation Props => (CompProperties_PapillonMaturation)props;

        private static Need ManaNeed(Pawn pawn)
        {
            return pawn?.needs?.TryGetNeed(ProjectMamono_DefOf.ProjectMamono_Mana);
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref charges, "charges", 0f);
            Scribe_Values.Look(ref lastMana, "lastMana", -1f);
        }

        public override string CompInspectStringExtra()
        {
            // A player who is watching her deserves the number, and it is the only place the mod
            // says how far along she is before the cocoon appears.
            if (!(parent is Pawn pawn) || !pawn.IsColonist || Puppating)
            {
                return null;
            }
            return "PMM_PapillonCharges".Translate(charges.ToString("0"), Props.chargesNeeded.ToString("0"));
        }

        public override void CompTickRare()
        {
            base.CompTickRare();

            if (!(parent is Pawn pawn) || pawn.Dead || !pawn.Spawned || pawn.Map == null)
            {
                return;
            }
            if (Props.requireColonist && !pawn.IsColonist)
            {
                return;
            }
            // Already inside one, or already a papillon: nothing left to count.
            if (Puppating)
            {
                return;
            }

            Need mana = ManaNeed(pawn);
            if (mana == null)
            {
                return;
            }

            float now = mana.CurLevel;
            if (lastMana < 0f)
            {
                lastMana = now;
                return;
            }
            if (now < lastMana && Props.manaPerCharge > 0f)
            {
                charges += (lastMana - now) / Props.manaPerCharge;
            }
            lastMana = now; // refills never subtract, by design

            if (charges >= Props.chargesNeeded)
            {
                BeginCocoon(pawn);
            }
        }

        /// <summary>True while she is inside a cocoon, or once she has stopped being a greenworm.</summary>
        private bool Puppating
        {
            get
            {
                if (!(parent is Pawn pawn) || pawn.health?.hediffSet == null)
                {
                    return false;
                }
                if (pawn.genes?.Xenotype != null
                    && pawn.genes.Xenotype.defName != "PMM_InsectGreenworm")
                {
                    return true;
                }
                HediffDef cocooned = DefDatabase<HediffDef>.GetNamedSilentFail(CompPapillonCocoon.HediffDefName);
                return cocooned != null && pawn.health.hediffSet.HasHediff(cocooned);
            }
        }

        private void BeginCocoon(Pawn pawn)
        {
            ThingDef cocoonDef = DefDatabase<ThingDef>.GetNamedSilentFail(Props.cocoonDef);
            XenotypeDef target = DefDatabase<XenotypeDef>.GetNamedSilentFail(Props.targetXenotype);
            if (cocoonDef == null || target == null)
            {
                return;
            }

            Thing cocoon = ThingMaker.MakeThing(cocoonDef);
            cocoon.SetFaction(pawn.Faction);
            GenSpawn.Spawn(cocoon, pawn.Position, pawn.Map);

            // GetComp lives on ThingWithComps, and a building is one - the same cast the egg sac's
            // projectile does when it hands the brood order over.
            if (cocoon is ThingWithComps withComps)
            {
                withComps.GetComp<CompPapillonCocoon>()?.SetGrub(pawn, Find.TickManager.TicksGame
                    + (int)(Props.cocoonDays * GenDate.TicksPerDay), target);
            }
            charges = 0f;
            PMMLog.Message($"[PMM Insects] {pawn.LabelShort} has spent enough mana and is cocooning.");
        }
    }

    public class CompProperties_PapillonCocoon : CompProperties
    {
        /// <summary>
        /// The hediff that holds her still. Its `Moving` capacity is set to 0, so she cannot walk
        /// out of a cocoon she is inside - and it is the comp, not the greenworm, that puts it on
        /// her and takes it off again, so the two halves of this feature cannot drift apart.
        /// </summary>
        public string hediffDef = "PMM_Cocooned";

        public CompProperties_PapillonCocoon()
        {
            compClass = typeof(CompPapillonCocoon);
        }
    }

    /// <summary>
    /// The cocoon itself: it holds one grub, holds her still, hatches her when its time is up, and
    /// takes her with it if it is broken first (user's ruling, 2026-10-02).
    ///
    /// The grub is held as a scribed reference rather than found by cell, because a cocoon can sit
    /// through a save and a cell can hold more than one pawn.
    /// </summary>
    public class CompPapillonCocoon : ThingComp
    {
        /// <summary>The hediff named by the props' default, for the two places that need it by name.</summary>
        public const string HediffDefName = "PMM_Cocooned";

        private Pawn grub;

        /// <summary>Tick she hatches at. -1 means the comp was placed before `SetGrub` ran.</summary>
        private int hatchTick = -1;

        /// <summary>What she becomes, handed over by the greenworm's comp.</summary>
        private XenotypeDef targetXenotype;

        private CompProperties_PapillonCocoon Props => (CompProperties_PapillonCocoon)props;

        private HediffDef CocoonHediff => DefDatabase<HediffDef>.GetNamedSilentFail(Props.hediffDef);

        public void SetGrub(Pawn pawn, int hatchAtTick, XenotypeDef target)
        {
            grub = pawn;
            hatchTick = hatchAtTick;
            targetXenotype = target;
            HoldStill();
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_References.Look(ref grub, "grub");
            Scribe_Values.Look(ref hatchTick, "hatchTick", -1);
            Scribe_Defs.Look(ref targetXenotype, "targetXenotype");
        }

        public override void CompTickRare()
        {
            base.CompTickRare();

            if (grub == null || grub.Dead || !grub.Spawned)
            {
                // She died or left the map: an empty cocoon is just a sac on the ground.
                parent.Destroy(DestroyMode.Vanish);
                return;
            }

            HoldStill();

            if (hatchTick >= 0 && Find.TickManager.TicksGame >= hatchTick)
            {
                Hatch();
            }
        }

        /// <summary>
        /// Keeps the immobility hediff on her. It is re-asserted every tick rather than only when the
        /// cocoon is made, so a dev-tool cure or a hediff that was lost some other way cannot leave
        /// her walking around inside it.
        /// </summary>
        private void HoldStill()
        {
            HediffDef hediff = CocoonHediff;
            if (hediff == null || grub?.health?.hediffSet == null)
            {
                return;
            }
            if (!grub.health.hediffSet.HasHediff(hediff))
            {
                grub.health.AddHediff(hediff);
            }
        }

        private void Hatch()
        {
            Pawn pawn = grub;
            HediffDef hediff = CocoonHediff;

            // The swap first: ConvertXenotype changes her genes, her race and her graphics, and it
            // notifies the player itself.
            bool converted = MamonoTransformation.ConvertXenotype(pawn, targetXenotype);

            // Then the cocoon lets go - and it is destroyed with Vanish, which is exactly the mode
            // PostDestroy below ignores, so hatching never kills the woman it just released.
            if (hediff != null && pawn.health.hediffSet.HasHediff(hediff))
            {
                pawn.health.RemoveHediff(pawn.health.hediffSet.GetFirstHediffOfDef(hediff));
            }
            if (converted)
            {
                PMMLog.Message($"[PMM Insects] {pawn.LabelShort} has emerged from her cocoon as a papillon.");
            }
            parent.Destroy(DestroyMode.Vanish);
        }

        public override string CompInspectStringExtra()
        {
            if (hatchTick < 0)
            {
                return null;
            }
            int daysLeft = (hatchTick - Find.TickManager.TicksGame) / GenDate.TicksPerDay;
            return "PMM_CocoonDaysLeft".Translate(daysLeft < 0 ? 0 : daysLeft);
        }

        /// <summary>
        /// Broken before its time, it takes her with it. The one mode that does not is `Vanish` -
        /// the cocoon's own hatch, a dev-tool deletion or an internal cleanup - because a
        /// bookkeeping removal is not a player destroying anything.
        /// </summary>
        public override void PostDestroy(DestroyMode mode, Map previousMap)
        {
            base.PostDestroy(mode, previousMap);
            if (mode == DestroyMode.Vanish)
            {
                return;
            }
            if (grub != null && !grub.Dead)
            {
                grub.Kill(null);
            }
        }
    }
}
