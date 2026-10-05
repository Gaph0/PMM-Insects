using System.Collections.Generic;
using ProjectMamono;
using RimWorld;
using Verse;

namespace PMM_Insects
{
    /// <summary>
    /// Carries the queen's brood order across the throw.
    ///
    /// It spawns the sac itself rather than leaving that to `Projectile_SpawnsThing`, for one reason:
    /// vanilla's version drops the thing it just made, so the order would have to be found again on
    /// the map afterwards - by landing cell, which is where two sacs in one cell can swap orders (a
    /// sac is a building, and vanilla's `tryAdjacentFreeSpaces` walk steps around the cell it hit, not
    /// around a sac already sitting beside it). Making the sac here keeps the handle, so the order
    /// goes to that very object and to nothing else.
    ///
    /// `Projectile.Impact`'s own three lines - the impact clamour, the landed effect and the
    /// projectile's destruction - are copied below because `base` is the thing that spawns. Re-diff
    /// them against `Verse.Projectile.Impact` when a 1.6.x patch ships.
    /// </summary>
    public class Projectile_EggSac : Projectile_SpawnsThing
    {
        protected override void Impact(Thing hitThing, bool blockedByShield = false)
        {
            Map map = Map;
            if (map == null)
            {
                return;
            }
            IntVec3 loc = Position;
            // Vanilla's own landing rule, kept because the spawn below has to land where vanilla's
            // would have: the impact cell, or the first free standable neighbour when the cell is
            // taken by a building and tryAdjacentFreeSpaces is on (our projectile sets it).
            if (def.projectile.tryAdjacentFreeSpaces && loc.GetFirstBuilding(map) != null)
            {
                foreach (IntVec3 cell in GenAdjFast.AdjacentCells8Way(loc))
                {
                    if (cell.GetFirstBuilding(map) == null && cell.Standable(map))
                    {
                        loc = cell;
                        break;
                    }
                }
            }

            // Verse.Projectile.Impact, verbatim.
            GenClamor.DoClamor(this, 12f, ClamorDefOf.Impact);
            if (!blockedByShield && def.projectile.landedEffecter != null)
            {
                def.projectile.landedEffecter.Spawn(ExactPosition.ToIntVec3(), map).Cleanup();
            }
            Destroy();

            // Projectile_SpawnsThing.Impact's own spawn, in vanilla's order: spawn, then take the
            // launcher's faction. Vanilla reads `Launcher.Faction` unguarded, which throws for a
            // projectile nobody launched (a dev spawn); this one leaves the sac unfactioned instead.
            Thing spawned = GenSpawn.Spawn(ThingMaker.MakeThing(def.projectile.spawnsThingDef), loc, map);
            if (spawned.def.CanHaveFaction && Launcher != null)
            {
                spawned.SetFaction(Launcher.Faction);
            }

            // No launcher, or a launcher that is not a queen (a dev spawn, a stray projectile):
            // the sac simply keeps the def's own default brood.
            Pawn queen = Launcher as Pawn;
            CompBroodOrder order = queen?.GetComp<CompBroodOrder>();
            if (order != null && spawned is ThingWithComps sac)
            {
                sac.GetComp<CompEggSacBrood>()?.SetBrood(order.ResolvedBrood, queen);
            }
        }
    }

    /// <summary>
    /// The sac's half of the brood: what the queen was told to lay, and the hatch itself.
    ///
    /// **The sac is a wait now, not a thing to break** (user's ruling, 2026-10-04): it opens by
    /// itself a day after it is thrown, and anyone who breaks it before that kills the brood
    /// inside. The timer is a comp tick, which is why the def carries a `<tickerType>` - a building
    /// is not ticked unless it says so, and the cocoon in `Things_Cocoon.xml` learned that the hard
    /// way (`archive/HANDOFF.md` §5.20).
    ///
    /// Derives from vanilla's `CompSpawnPawnOnDestroyed` so the def keeps its own `pawnKind` and
    /// `lordJob` fields and so the swarmling default can go straight through vanilla's hatch -
    /// age 0, the stun-flyer hop and the nest lord are all right for the swarmling and none of them are
    /// right for a mamono child. It also inherits the reason this has to be a comp at all: vanilla
    /// hardcodes `allowDowned: false` (which a newborn humanlike always fails - the 2026-09-20
    /// "Generated downed pawn" cascade) and an age no def field can express.
    /// </summary>
    public class CompProperties_EggSacBrood : CompProperties_SpawnPawnOnDestroyed
    {
        /// <summary>
        /// How old the hatched mamono is, in biological years, and 0 is the answer the mod settled
        /// on: the abaddon raises her brood from 0, so a daughter comes out of the sac a newborn
        /// (user's ruling 2026-10-01). A def field rather than a constant because it is the one
        /// number here worth trying - it read 3, a child who walks and works, for a while. Vanilla
        /// has no such field at all, which is half the reason this comp exists.
        /// </summary>
        public float biologicalAge = 0f;

        /// <summary>
        /// Hours from the throw until the sac opens by itself (user's ruling, 2026-10-04: one day).
        /// A def field for the same reason as the age above - it is a number worth trying, and it
        /// is what decides how much of a race a sac on the ground is.
        /// </summary>
        public float hatchHours = 24f;

        public CompProperties_EggSacBrood()
        {
            compClass = typeof(CompEggSacBrood);
        }
    }

    public class CompEggSacBrood : CompSpawnPawnOnDestroyed
    {
        /// <summary>
        /// The order carried from the queen. Scribed, because a sac can sit on the map - or fly -
        /// across a save, and without it she would hatch the default brood afterwards. Null means
        /// nobody told this sac anything, and the def's own `pawnKind` is the fallback.
        /// </summary>
        private PawnKindDef brood;

        /// <summary>
        /// The queen who threw the sac. Scribed as a reference for the same reason as the order:
        /// the relation has to be added when the sac is killed, which can be long after the throw,
        /// and a dead queen still counts - her daughter is still her daughter.
        /// </summary>
        private Pawn mother;

        /// <summary>
        /// Ticks left before she opens by herself, counted down on the rare tick. Scribed, so a sac
        /// waits out a save the way it waits out anything else, and **-1 means the timer has never
        /// started**: a sac spawned from XML gets its full day in `PostSpawnSetup`, and so does one
        /// out of a save written before this timer existed - rather than hatching, or dying, on its
        /// first tick after the load.
        /// </summary>
        private int ticksUntilHatch = -1;

        private CompProperties_EggSacBrood Props => (CompProperties_EggSacBrood)props;

        public void SetBrood(PawnKindDef kind, Pawn motherPawn)
        {
            brood = kind;
            mother = motherPawn;
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            if (ticksUntilHatch < 0)
            {
                ticksUntilHatch = (int)(Props.hatchHours * (float)GenDate.TicksPerHour);
            }
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Defs.Look(ref brood, "brood");
            Scribe_References.Look(ref mother, "mother");
            Scribe_Values.Look(ref ticksUntilHatch, "ticksUntilHatch", -1);
        }

        /// <summary>
        /// What she is, and how long she has left. The countdown keeps the cocoon's shape
        /// (`PapillonMaturation.cs`): whole days until fewer than one remain, then whole hours, and
        /// both units are vanilla's own period keys, which is where the singular and the plural come
        /// from. Rounding hours up means it reads "1 hour" until the hatch rather than "0 hours", and
        /// the unit drops to hours as soon as the days run out - "hatches in 0 days" for a whole day
        /// is a shrug, not a countdown (the user's words about the cocoon, 2026-10-03, §5.20).
        /// </summary>
        public override string CompInspectStringExtra()
        {
            string holds = brood == null ? null : "PMM_BroodSacHolds".Translate(brood.LabelCap);
            string left = HatchCountdown();
            if (holds == null)
            {
                return left;
            }
            return left == null ? holds : holds + "\n" + left;
        }

        private string HatchCountdown()
        {
            if (ticksUntilHatch < 0)
            {
                return null;
            }
            if (ticksUntilHatch >= GenDate.TicksPerDay)
            {
                int days = ticksUntilHatch / GenDate.TicksPerDay;
                return "PMM_BroodSacHatchesIn".Translate(days == 1 ? "Period1Day".Translate() : "PeriodDays".Translate(days));
            }
            int hours = (ticksUntilHatch + GenDate.TicksPerHour - 1) / GenDate.TicksPerHour;
            return "PMM_BroodSacHatchesIn".Translate(hours <= 1 ? "Period1Hour".Translate() : "PeriodHours".Translate(hours));
        }

        /// <summary>
        /// Her day is up, so she crawls out on her own. This is the ordinary hatch now: the sac is
        /// a wait rather than a thing the player breaks to get a daughter out.
        ///
        /// The destroy is `Vanish`, not `KillFinalize`, and that mode is load bearing - it is what
        /// tells `PostDestroy` below that this is the sac opening rather than somebody breaking it,
        /// so the brood is spawned once and nobody is killed on the way out.
        /// </summary>
        public override void CompTickRare()
        {
            base.CompTickRare();
            if (parent.Destroyed)
            {
                return;
            }
            ticksUntilHatch -= GenTicks.TickRareInterval;
            if (ticksUntilHatch > 0)
            {
                return;
            }
            SpawnBrood(parent.Map, live: true);
            parent.Destroy(DestroyMode.Vanish);
        }

        public override void PostDestroy(DestroyMode mode, Map previousMap)
        {
            // The hatch above, or a removal that is nobody's doing: a dev-tool delete, the map
            // unloading, an internal cleanup. These take nobody with them - the same call the
            // papillon's cocoon makes for `Vanish` (`archive/HANDOFF.md` §5.20).
            if (mode == DestroyMode.Vanish)
            {
                return;
            }
            // Anyone else breaking it, however they do it: **her day up means she crawls out, and
            // her day not up means she dies in there** (user's ruling, 2026-10-04). Her day is the
            // tick count, not the clock, so a sac on an unloaded map is still waiting when the
            // player comes back to it.
            SpawnBrood(previousMap, live: ticksUntilHatch <= 0);
        }

        /// <summary>
        /// Her out of the sac, alive or dead. The one place that knows what a brood is: the queen's
        /// order when the sac carries one, the def's own `pawnKind` otherwise, and two different
        /// hatches because a mamono and a VFEI2 swarmling want opposite things (see the class comment).
        /// </summary>
        private void SpawnBrood(Map map, bool live)
        {
            if (map == null)
            {
                return;
            }
            PawnKindDef kind = brood ?? Props.pawnKind;
            if (kind == null)
            {
                return;
            }
            // Not a mamono - a swarmling, or a sac saved before the picker existed. Vanilla's
            // hatch, untouched while she lives: VFEI2's own swarmling chain wants every part of it.
            // Vanilla gates that hatch on `KillFinalize`, which the tick's `Vanish` is not, so the
            // mode is named here rather than passed through.
            if (kind.race?.race?.Humanlike != true)
            {
                if (live)
                {
                    base.PostDestroy(DestroyMode.KillFinalize, map);
                }
                else
                {
                    SpawnDead(kind, map);
                }
                return;
            }
            Pawn daughter = GenerateMamono(kind, map);
            if (!live)
            {
                // She never hatched, so she is not named and carries no relation: the corpse is the
                // whole record of her. Nothing reads her name and no family has lost a daughter
                // they ever met, so the queen's social tab stays out of it.
                daughter.Kill(null);
                return;
            }
            AddMotherRelation(daughter);
            AddFatherRelation(daughter);
            OfferName(daughter, mother);
            // Then nothing. No PawnFlyer_Stun hop and no lord: LordJob_WanderNest is written for
            // insects wandering a hive, and a newborn mamono thrown five cells would only look
            // like a bug. She simply stands up where the sac was. The wake-up call to
            // CompCanBeDormant is skipped with them - that is an insect comp she does not have.
        }

        /// <summary>
        /// The mamono herself. Generated rather than taken from the def for the two reasons the
        /// class comment gives: `allowDowned` has to be true for a newborn humanlike, and her age
        /// is a props field where vanilla has none.
        /// </summary>
        private Pawn GenerateMamono(PawnKindDef kind, Map map)
        {
            float? fixedBiologicalAge = Props.biologicalAge;
            Pawn pawn = PawnGenerator.GeneratePawn(new PawnGenerationRequest(kind, parent.Faction, PawnGenerationContext.NonPlayer, null, forceGenerateNewPawn: true, allowDead: false, allowDowned: true, canGeneratePawnRelations: true, mustBeCapableOfViolence: false, 1f, forceAddFreeWarmLayerIfNeeded: false, allowGay: true, allowPregnant: false, allowFood: true, allowAddictions: true, inhabitant: false, certainlyBeenInCryptosleep: false, forceRedressWorldPawnIfFormerColonist: false, worldPawnFactionDoesntMatter: false, 0f, 0f, null, 1f, null, null, null, null, null, fixedBiologicalAge));
            GenSpawn.Spawn(pawn, parent.Position, map, WipeMode.VanishOrMoveAside);
            return pawn;
        }

        /// <summary>
        /// The brood dying with the sac. She is generated and killed rather than quietly left out of
        /// the world, so breaking an egg leaves a corpse where the sac stood - the shape the
        /// papillon's cocoon uses when it is broken early (`archive/HANDOFF.md` §5.20), and the only trace a
        /// player gets that they have killed one of their own queen's daughters.
        /// </summary>
        private void SpawnDead(PawnKindDef kind, Map map)
        {
            Pawn pawn = PawnGenerator.GeneratePawn(new PawnGenerationRequest(kind, parent.Faction, PawnGenerationContext.NonPlayer, null, forceGenerateNewPawn: true, allowDead: false, allowDowned: false, canGeneratePawnRelations: false, mustBeCapableOfViolence: false, 1f, forceAddFreeWarmLayerIfNeeded: false, allowGay: true, allowPregnant: false, allowFood: true, allowAddictions: true, inhabitant: false, certainlyBeenInCryptosleep: false, forceRedressWorldPawnIfFormerColonist: false, worldPawnFactionDoesntMatter: false, 0f, 0f, null, 1f, null, null, null, null, null, Props.biologicalAge));
            GenSpawn.Spawn(pawn, parent.Position, map, WipeMode.VanishOrMoveAside);
            pawn.Kill(null);
        }
        /// <summary>
        /// She is the queen's daughter, and the game needs exactly one call for that: `Child`
        /// ("son"/"daughter") is an IMPLIED relation, and `AddDirectRelation` refuses implied
        /// defs outright (`if (def.implied) -&gt; Log.Warning and return`). Writing only the child's
        /// side is also what vanilla's own birth code does in `PregnancyUtility` - it adds
        /// `PawnRelationDefOf.Parent` for the genetic mother and the father, and the mother's
        /// side then reads "daughter" on its own.
        ///
        /// No duplicate check: the pawn was generated one statement ago and cannot already carry
        /// relations, and `AddDirectRelation` logs a warning if that ever stopped being true.
        /// </summary>
        private void AddMotherRelation(Pawn child)
        {
            if (mother == null || mother.Destroyed || child.relations == null || !child.RaceProps.IsFlesh)
            {
                return;
            }
            child.relations.AddDirectRelation(PawnRelationDefOf.Parent, mother);
        }

        /// <summary>
        /// And her man, when she has one - vanilla's birth code adds him next to the mother in
        /// `PregnancyUtility`. A pregnancy knows its father (he is recorded at conception), but an
        /// egg laid by a queen has nobody but her, so he is read off her relations at hatch time.
        ///
        /// Order: **the core mod's tsugai bond first, then spouse, fiance, lover** - hence the
        /// `ProjectMamono_DefOf` reference. The bond is a `PawnRelationDef` of the core mod's, and it
        /// outranks a vanilla marriage by its own def: `importance` 210 against the spouse's 200,
        /// and the mod's lore is that a tsugai bond is for life. All of them male, because that is
        /// what the game means by a father (`ChildRelationUtility.ChanceOfBecomingChildOf` even
        /// logs a warning when handed a non-male one) and because `Parent` is labelled by gender,
        /// so a woman partner would be written down as a second mother. Ex-partners are
        /// deliberately not on the list.
        ///
        /// The tsugai partner must be ALIVE - that is the core mod's own rule for the bond
        /// (`TsugaiFormation.HasBondedPartner` asks for `!p.Dead`, and a partner's death severs the
        /// bond hediff and only leaves grief). The vanilla relations are not filtered that way: a
        /// dead husband stays her husband.
        /// </summary>
        private void AddFatherRelation(Pawn child)
        {
            if (mother == null || mother.Destroyed || mother.relations == null || child.relations == null)
            {
                return;
            }
            Pawn father = mother.relations.GetFirstDirectRelationPawn(ProjectMamono_DefOf.ProjectMamono_Tsugai, IsLivingFather)
                ?? mother.relations.GetFirstDirectRelationPawn(PawnRelationDefOf.Spouse, IsFather)
                ?? mother.relations.GetFirstDirectRelationPawn(PawnRelationDefOf.Fiance, IsFather)
                ?? mother.relations.GetFirstDirectRelationPawn(PawnRelationDefOf.Lover, IsFather);
            if (father == null || father == mother || father == child)
            {
                return;
            }
            child.relations.AddDirectRelation(PawnRelationDefOf.Parent, father);
        }

        private static bool IsFather(Pawn candidate)
        {
            return candidate != null && !candidate.Destroyed && candidate.gender == Gender.Male;
        }

        private static bool IsLivingFather(Pawn candidate)
        {
            return IsFather(candidate) && !candidate.Dead;
        }

        /// <summary>
        /// Offers the player a naming window for a daughter of their own colony - the same window
        /// vanilla opens for a newborn, with the first, nick and last name all editable.
        ///
        /// Vanilla's own factory (`PawnNamingUtility.NamePawnDialog`) decides what is editable by
        /// testing `Pawn.babyNamingDeadline`, so the dialog is built directly here rather than
        /// through it: same result for us, and the deadline - which vanilla's birth code owns and
        /// which other systems read - is left alone. For a humanlike the window writes its own
        /// description, and it reads "Mother: <the queen>" from the relation added just above.
        ///
        /// Only for a hatch on the map the player is looking at. A window popping for a sac
        /// somebody killed across the world would be noise, and a nest queen's brood is not the
        /// player's to name at all.
        /// </summary>
        private static void OfferName(Pawn child, Pawn mother)
        {
            if (child.Faction != Faction.OfPlayer || child.Map == null || child.Map != Find.CurrentMap)
            {
                return;
            }
            NameFilter names = NameFilter.First | NameFilter.Nick | NameFilter.Last;
            Dictionary<NameFilter, List<string>> suggested = null;
            string queenFamily = (mother?.Name as NameTriple)?.Last;
            if (!queenFamily.NullOrEmpty())
            {
                suggested = new Dictionary<NameFilter, List<string>>
                {
                    { NameFilter.Last, new List<string> { queenFamily } },
                };
            }
            Find.WindowStack.Add(new Dialog_NamePawn(child, names, names, suggested));
        }
    }
}
