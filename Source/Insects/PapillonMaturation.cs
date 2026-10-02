using System.Collections.Generic;
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
    /// She is held the way a cryptosleep casket holds a sleeper (user's call, 2026-10-02: "make sure
    /// the mamono is treated as if she were in a cryptosleep casket while the pupation is
    /// happening"). The sac is a building on her tile (`CompPapillonCocoon`,
    /// `Defs/ThingDefs/Things_Cocoon.xml`) and its comp is her holder: she is despawned into the
    /// comp's own `ThingOwner`, and the comp reports its contents suspended, which is the engine's
    /// own way of saying "this pawn is in cryptosleep, do not tick her". Destroy the sac before its
    /// time and she dies with it - the user's ruling, 2026-10-02.
    ///
    /// Three earlier builds are worth knowing about, because each one left a trap behind:
    ///
    ///   1. An immobility hediff (`Moving` 0) made her a *downed* colonist lying on the map, and a
    ///      downed colonist is a rescue target: her own colony fetched her into a bed within minutes
    ///      and left the sac empty (user's report: "she exists outside the cocoon building when she
    ///      reaches her required mana").
    ///   2. Gating the rescue (`CocoonedNotRescuedPatch` on `HealthAIUtility.CanRescueNow`) closed
    ///      the work givers but not the alert, which asks `HealthAIUtility.WantsToBeRescued`
    ///      instead - so the player still got "Colonist needs rescue" and a marker over the sac.
    ///      Hiding her (`CocoonedInvisiblePatch`) fixed what was drawn and nothing else.
    ///   3. Both gates were treating a symptom. A pawn who is not on the map cannot be rescued,
    ///      alerted, targeted or drawn, so both patches are gone and she is simply held instead.
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
        /// Charges she needs before the cocoon starts. At the defaults that is thirty full bars of
        /// spent mana - around sixty days away from a hive at her own drain rate of about half a
        /// bar a day, sooner if she is also giving essence away (user's call, 2026-10-02: 600).
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
    ///
    /// She can see how far along she is. The counter was hidden at first (user's call, 2026-10-02:
    /// "the maturation should surprise a player rather than fill a bar") and restored the next day,
    /// once it was clear what the absence costs: at the shipping numbers this is a sixty-day wait, and
    /// a player watching a grub cannot tell a slow pupation from one that will never happen. So the
    /// line is permanent, and the cocoon's own countdown follows it once she is inside.
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

        /// <summary>
        /// How much mana she has spent towards pupation, printed on her own description. It is the only
        /// place the mod says how far along she is before the cocoon appears, and the difference it
        /// makes is between a grub who is getting there and one who never will.
        /// </summary>
        public override string CompInspectStringExtra()
        {
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

        /// <summary>
        /// True while she is inside a cocoon, or once she has stopped being a greenworm. Being held
        /// by the sac is what answers the first half - not a hediff and not a cell - so it is her own
        /// `ParentHolder` that is asked, and a grub the sac never hatched (a target xenotype that no
        /// longer exists, say) is a grub again with her count reset.
        /// </summary>
        private bool Puppating
        {
            get
            {
                if (!(parent is Pawn pawn))
                {
                    return false;
                }
                if (pawn.genes?.Xenotype != null
                    && pawn.genes.Xenotype.defName != "PMM_InsectGreenworm")
                {
                    return true;
                }
                return pawn.ParentHolder is CompPapillonCocoon;
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

            // The sac first, then her: taking her in is what despawns her, and a despawned pawn has
            // no map left to be taken off. Nothing needs freezing by hand once she is inside - a
            // suspended pawn does not tick, so her hunger, her rest and her mana all stop where
            // they were.
            Thing cocoon = ThingMaker.MakeThing(cocoonDef);
            cocoon.SetFaction(pawn.Faction);
            GenSpawn.Spawn(cocoon, pawn.Position, pawn.Map);

            // GetComp lives on ThingWithComps, and a building is one - the same cast the egg sac's
            // projectile does when it hands the brood order over.
            if (cocoon is ThingWithComps withComps)
            {
                withComps.GetComp<CompPapillonCocoon>()?.TakeIn(pawn, Find.TickManager.TicksGame
                    + (int)(Props.cocoonDays * GenDate.TicksPerDay), target);
            }
            charges = 0f;
            PMMLog.Message($"[PMM Insects] {pawn.LabelShort} has spent enough mana and is cocooning.");
        }
    }

    public class CompProperties_PapillonCocoon : CompProperties
    {
        /// <summary>
        /// No settings of its own. The sac is told everything it needs - who is inside, when she comes
        /// out and what she becomes - by the greenworm's comp, so this class is here to declare the
        /// comp, which is what an XML comp list asks for by name.
        /// </summary>
        public CompProperties_PapillonCocoon()
        {
            compClass = typeof(CompPapillonCocoon);
        }
    }

    /// <summary>
    /// The cocoon itself: a sac that holds one grub, keeps her off the map while it holds her,
    /// hatches her when its time is up, and takes her with it if it is broken first (user's ruling,
    /// 2026-10-02).
    ///
    /// It holds her the way `CompBiosculpterPod` holds the pawn being biosculpted, and that is the
    /// whole trick rather than a detail: the `ThingOwner<Pawn>` inside this comp is the single source
    /// of truth for who is inside, so a save, a reload, a raid and a fifteen-day wait all keep hold of
    /// the same woman without a lookup by cell and without a hediff standing in for a container.
    ///
    /// `IsContentsSuspended` is what makes it a *cryptosleep* hold rather than a shelf. `Thing.Tick`
    /// asks `ThingOwnerUtility.ContentsSuspended` before it ticks whatever a thing holds, and vanilla
    /// checks `Building_CryptosleepCasket` on exactly the same line - so answering true means she is
    /// not ticked at all: her needs, her hediffs and her mana stop where they were, and nothing here
    /// has to freeze them by name.
    ///
    /// Nothing else needs special-casing either. A despawned pawn is not in `PawnsFinder`, not in the
    /// colonist bar's `mapPawns.FreeColonists`, not in any cell the renderer walks, and not a candidate
    /// for the rescue alert - which is why the two Harmony patches this file used to carry are gone.
    /// </summary>
    public class CompPapillonCocoon : ThingComp, IThingHolder, ISuspendableThingHolder
    {
        /// <summary>
        /// Whoever is inside, held by owner rather than by cell. Deep-scribed by this comp, so the pawn
        /// is written into the sac in the save - the shape a pod or a casket uses.
        /// </summary>
        private ThingOwner<Pawn> held;

        /// <summary>Tick she hatches at. -1 means `TakeIn` never ran, so the sac is empty.</summary>
        private int hatchTick = -1;

        /// <summary>What she becomes, handed over by the greenworm's comp.</summary>
        private XenotypeDef targetXenotype;

        public CompPapillonCocoon()
        {
            held = new ThingOwner<Pawn>(this, oneStackOnly: true);
        }

        /// <summary>She is asleep in there, not stored: nothing inside her ticks.</summary>
        public bool IsContentsSuspended => true;

        public ThingOwner GetDirectlyHeldThings()
        {
            return held;
        }

        public void GetChildHolders(List<IThingHolder> outChildren)
        {
            ThingOwnerUtility.AppendThingHoldersFromThings(outChildren, held);
        }

        /// <summary>Whoever is inside, or null if the sac is empty.</summary>
        private Pawn Grub => held != null && held.Count > 0 ? held[0] : null;

        /// <summary>
        /// Takes her in, which is the moment she leaves the map. `DeSpawnOrDeselect` also drops any
        /// selection of her, so the player is not left holding a bracket around an empty tile.
        /// </summary>
        public void TakeIn(Pawn pawn, int hatchAtTick, XenotypeDef target)
        {
            hatchTick = hatchAtTick;
            targetXenotype = target;
            pawn.DeSpawnOrDeselect();
            held.TryAddOrTransfer(pawn, 1);
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Deep.Look(ref held, "held", this);
            Scribe_Values.Look(ref hatchTick, "hatchTick", -1);
            Scribe_Defs.Look(ref targetXenotype, "targetXenotype");
        }

        public override void CompTickRare()
        {
            base.CompTickRare();

            if (Grub == null)
            {
                // Nobody inside: an empty sac is just a sac on the ground.
                parent.Destroy(DestroyMode.Vanish);
                return;
            }

            if (hatchTick >= 0 && Find.TickManager.TicksGame >= hatchTick)
            {
                Hatch();
            }
        }

        private void Hatch()
        {
            // She comes out first, because the swap after it dirties her graphics and tells the player
            // about a pawn who is back on the map. A sac is `PassThroughOnly`, so the cell under it is
            // hers the moment it opens.
            Pawn pawn = LetOut(parent.Map);

            // The swap is core's: ConvertXenotype changes her genes, her race and her graphics, and it
            // notifies the player itself, which is why nothing here posts a message.
            if (pawn != null && MamonoTransformation.ConvertXenotype(pawn, targetXenotype))
            {
                PMMLog.Message($"[PMM Insects] {pawn.LabelShort} has emerged from her cocoon as a papillon.");
            }

            // Destroyed with Vanish, which is exactly the mode the hatch is meant to be: she is out,
            // and the sac that held her does not kill her on the way out.
            parent.Destroy(DestroyMode.Vanish);
        }

        /// <summary>
        /// Puts whoever is inside back on the map beside the sac and returns her, or null if there was
        /// nobody to let out. The map is passed in because the destruction path has to hand over the one
        /// the sac used to stand on.
        /// </summary>
        private Pawn LetOut(Map map)
        {
            Pawn pawn = Grub;
            if (pawn == null || map == null)
            {
                return null;
            }
            held.TryDropAll(parent.Position, map, ThingPlaceMode.Near);
            return pawn;
        }

        /// <summary>
        /// How long she has left in there: whole days until fewer than one remain, then whole hours.
        ///
        /// A countdown that reads "Emerges in 0 days" for a full day is not a countdown at all, it is a
        /// shrug (user's report, 2026-10-03, when the test value was a single day), so the unit drops to
        /// hours as soon as the days run out. Both units are vanilla's own period keys, which is where the
        /// singular and the plural come from, and rounding hours up means it reads "1 hour" until the
        /// hatch rather than "0 hours".
        /// </summary>
        public override string CompInspectStringExtra()
        {
            if (hatchTick < 0)
            {
                return null;
            }

            int ticksLeft = hatchTick - Find.TickManager.TicksGame;
            if (ticksLeft >= GenDate.TicksPerDay)
            {
                int days = ticksLeft / GenDate.TicksPerDay;
                return "PMM_CocoonEmergesIn".Translate(
                    days == 1 ? "Period1Day".Translate() : "PeriodDays".Translate(days));
            }

            int hours = (ticksLeft + GenDate.TicksPerHour - 1) / GenDate.TicksPerHour;
            return "PMM_CocoonEmergesIn".Translate(
                hours <= 1 ? "Period1Hour".Translate() : "PeriodHours".Translate(hours));
        }

        /// <summary>
        /// Broken before its time, it takes her with it: everything except `Vanish` - a deconstruct, a
        /// raid, fire - kills her, and `Vanish` (the hatch, a dev-tool deletion, an internal cleanup)
        /// only lets her out, because a bookkeeping removal is not a player destroying anything.
        ///
        /// Either way she leaves the container first. A corpse lands where the sac stood rather than
        /// inside a holder that is about to stop existing, which is also the difference between a pawn
        /// the game can clean up and one it holds forever.
        /// </summary>
        public override void PostDestroy(DestroyMode mode, Map previousMap)
        {
            base.PostDestroy(mode, previousMap);

            Pawn pawn = LetOut(previousMap);
            if (pawn != null && mode != DestroyMode.Vanish && !pawn.Dead)
            {
                pawn.Kill(null);
            }
        }
    }
}
