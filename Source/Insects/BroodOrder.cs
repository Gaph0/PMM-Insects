using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace PMM_Insects
{
    /// <summary>
    /// One line of a queen's brood list, written in XML on her race comp. An entry either names
    /// a caste (`pawnKind`) or asks for a random one (`randomMamono`), and may carry its own
    /// `iconPath` for the cases where the caste has no xenotype icon to borrow.
    /// </summary>
    public class BroodOption
    {
        public PawnKindDef pawnKind;

        public bool randomMamono;

        public string iconPath;
    }

    /// <summary>
    /// The queen's brood list. This comp remembers what she has been told to lay and builds the
    /// menu that asks; the sac that hatches it is a separate comp on PMM_EggSac, and the
    /// projectile in between carries the order across. Attached to the Abaddon's race def, so it
    /// reaches a queen however she arrived - spawned from her kind, dev-spawned, or a woman
    /// transformed into one.
    ///
    /// The menu is opened by Command_Ability_EggSpew (Source/Insects/EggSpewAbility.cs), which is
    /// the egg spew button itself since 2026-09-26. The second gizmo this comp used to yield is
    /// gone: the choice is now the first step of casting, and it shows its own disabled reason.
    /// </summary>
    public class CompProperties_BroodOrder : CompProperties
    {
        /// <summary>The castes she can be told to lay, in the order the menu shows them.</summary>
        public List<BroodOption> broodOptions = new List<BroodOption>();

        /// <summary>
        /// What she lays until somebody picks something. It is deliberately *not* one of the
        /// options: the menu holds the mamono castes, and the swarmling is only the answer while
        /// nobody has given an order (user's call, 2026-10-03).
        /// </summary>
        public PawnKindDef defaultBrood;

        public CompProperties_BroodOrder()
        {
            compClass = typeof(CompBroodOrder);
        }
    }

    public class CompBroodOrder : ThingComp
    {
        private PawnKindDef chosenKind;

        private bool randomMamono;

        private CompProperties_BroodOrder Props => (CompProperties_BroodOrder)props;

        /// <summary>
        /// Looked up by name rather than through a [DefOf] field. The def always ships with this
        /// mod, but a [DefOf] field pointing at a def that is not loaded is a load error, so the
        /// lookup stays defensive: null means no def, and the comp then stays quiet instead of
        /// burning.
        /// </summary>
        private static AbilityDef EggSpewDef => DefDatabase<AbilityDef>.GetNamedSilentFail("PMM_Ability_EggSpew");

        /// <summary>
        /// The caste the next sac will hold. Random draws from the humanlike entries only, so
        /// "random mamono" can never quietly hand back the swarmling entry. Null is possible only
        /// when nothing has been picked and the def names no default; the sac has its own.
        /// </summary>
        public PawnKindDef ResolvedBrood
        {
            get
            {
                if (randomMamono)
                {
                    return RandomMamonoOption() ?? Props.defaultBrood;
                }
                return chosenKind ?? Props.defaultBrood;
            }
        }

        /// <summary>The XML entry the current order came from, or null while nothing has been picked.</summary>
        private BroodOption CurrentOption
        {
            get
            {
                if (Props.broodOptions == null)
                {
                    return null;
                }
                foreach (BroodOption option in Props.broodOptions)
                {
                    if (option == null || (option.pawnKind == null && !option.randomMamono))
                    {
                        continue;
                    }
                    if (option.randomMamono)
                    {
                        if (randomMamono)
                        {
                            return option;
                        }
                    }
                    else if (!randomMamono && option.pawnKind == chosenKind)
                    {
                        return option;
                    }
                }
                return null;
            }
        }

        /// <summary>
        /// What she is currently told to lay, as the player reads it, or null when there is nothing to
        /// name - nothing picked, and a def that names no default brood. Public because the egg spew
        /// button prints it in its tooltip now that no picker gizmo carries it; the caller leaves the
        /// line out rather than printing an empty label.
        /// </summary>
        public string CurrentLabel
        {
            get
            {
                BroodOption option = CurrentOption;
                if (option != null)
                {
                    return OptionLabel(option);
                }
                return Props.defaultBrood?.LabelCap;
            }
        }

        /// <summary>
        /// Whether this queen may be given an order at all. Somebody else's queen cannot, and the
        /// egg spew button shows disabled with a reason for her.
        /// </summary>
        public bool CanOrder => (parent as Pawn)?.Faction == Faction.OfPlayer;

        /// <summary>
        /// Grants the ability herself instead of leaving it to her pawn kind, because a woman
        /// transformed into a queen is not that kind and would otherwise have no egg spew at all.
        /// The kind's own <abilities> entry is gone for the same reason: one source that covers
        /// both paths. Runs again on every load, and the null check makes that a no-op.
        /// </summary>
        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            Pawn pawn = parent as Pawn;
            AbilityDef ability = EggSpewDef;
            if (pawn == null || ability == null || pawn.abilities == null)
            {
                return;
            }
            if (pawn.abilities.GetAbility(ability) == null)
            {
                pawn.abilities.GainAbility(ability);
            }
        }

        /// <summary>
        /// Opens the caste menu. Returns false when there is nothing to choose from - an empty
        /// list and a missing default are both XML mistakes, and a menu with no entries would be
        /// worse than none - so the caller can fall back to a plain cast.
        ///
        /// `afterPick` runs once a caste has been chosen. The egg spew button uses it to start the
        /// targeting cursor, which is why choosing and then cancelling the cursor keeps the new
        /// order and spends no cooldown.
        /// </summary>
        public bool ShowOrderMenu(Action afterPick)
        {
            List<FloatMenuOption> options = new List<FloatMenuOption>();
            // A def that names no list at all is the same mistake as an empty one, and the caller
            // treats both the same way: no menu, plain cast.
            if (Props.broodOptions == null)
            {
                return false;
            }
            foreach (BroodOption option in Props.broodOptions)
            {
                if (option == null || (option.pawnKind == null && !option.randomMamono))
                {
                    continue;
                }
                BroodOption local = option;
                string label = OptionLabel(local);
                if (local == CurrentOption)
                {
                    label += " " + "PMM_BroodCurrent".Translate();
                }
                options.Add(new FloatMenuOption(label, delegate
                {
                    Pick(local);
                    afterPick?.Invoke();
                }, OptionIcon(local), Color.white));
            }
            if (options.Count == 0)
            {
                return false;
            }
            Find.WindowStack.Add(new FloatMenu(options));
            return true;
        }

        private void Pick(BroodOption option)
        {
            if (option.randomMamono)
            {
                randomMamono = true;
                chosenKind = null;
                return;
            }
            randomMamono = false;
            chosenKind = option.pawnKind;
        }

        /// <summary>
        /// Two scribes rather than one option index: a def and a flag both survive the option
        /// list being reordered or added to later, and an index would silently switch a save's
        /// brood to whatever moved into that slot.
        /// </summary>
        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Defs.Look(ref chosenKind, "broodKind");
            Scribe_Values.Look(ref randomMamono, "broodRandom", false);
        }

        private static string OptionLabel(BroodOption option)
        {
            return option.randomMamono ? "PMM_BroodRandom".Translate() : option.pawnKind.LabelCap;
        }

        private static Texture2D OptionIcon(BroodOption option)
        {
            if (!option.iconPath.NullOrEmpty())
            {
                Texture2D own = ContentFinder<Texture2D>.Get(option.iconPath, false);
                if (own != null)
                {
                    return own;
                }
            }
            return XenotypeIcon(option.pawnKind);
        }

        /// <summary>
        /// The caste's own xenotype icon, so the menu reads like the xenotype list the player
        /// already knows. A kind with no xenotype, or art that is not there, gets the bad-texture
        /// box instead of an exception.
        /// </summary>
        private static Texture2D XenotypeIcon(PawnKindDef kind)
        {
            XenotypeSet set = kind?.xenotypeSet;
            if (set != null && set.Count > 0)
            {
                XenotypeDef xenotype = set[0].xenotype;
                if (xenotype != null && !xenotype.iconPath.NullOrEmpty())
                {
                    Texture2D tex = ContentFinder<Texture2D>.Get(xenotype.iconPath, false);
                    if (tex != null)
                    {
                        return tex;
                    }
                }
            }
            return BaseContent.BadTex;
        }

        private PawnKindDef RandomMamonoOption()
        {
            if (Props.broodOptions == null)
            {
                return null;
            }
            List<PawnKindDef> pool = new List<PawnKindDef>();
            foreach (BroodOption option in Props.broodOptions)
            {
                if (option?.pawnKind != null && option.pawnKind.race?.race?.Humanlike == true)
                {
                    pool.Add(option.pawnKind);
                }
            }
            return pool.Count > 0 ? pool.RandomElement() : null;
        }
    }
}
