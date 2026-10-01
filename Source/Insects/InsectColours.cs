using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace PMM_Insects
{
    /// <summary>
    /// The castes' colours, and the rule that a mamono wears one of them.
    ///
    /// Three colours per caste, listed on the caste's own xenotype (<see cref="InsectHairColours"/>),
    /// and one of them rolled for her as her HAIR colour - which is also her wings', and her lower
    /// body's on the three eight-legged castes. Big & Small paints every wing colour setting from
    /// `hairColor` (`BS_WingClr_A`/`_B` and their dark and saturated variants all say so), and our
    /// own copy of its spider sets takes the same source (Defs/ThingDefs/SpiderBody_BS.xml). Her
    /// skin is vanilla: the insect gene carries no `skinColorBase` (2026-10-01), so nothing of ours
    /// competes with the melanin gene she rolls like any other pawn.
    ///
    /// Colours rather than genes, and this used to be a set of skin-colour genes. Two reasons, both
    /// read out of the 1.6 assembly:
    ///   * a hair-colour gene joins the pool `PawnHairColors.HairColorGenes` builds from every
    ///     GeneDef carrying a `hairColorOverride`, and `PawnGenerator` hands one of those to *any*
    ///     pawn it generates - so a palette of genes would have coloured strangers' hair;
    ///   * a skin-colour gene competed for `Pawn.story.SkinColorBase` with vanilla's own melanin
    ///     gene: `Pawn_GeneTracker.Notify_GenesChanged` gathers every gene carrying a `skinColorBase`
    ///     and picks one with `SelectGene`, which ends in `tmpGenes.TryRandomElement` over the pawn's
    ///     genes - so which tone she wore came down to luck, and a caste with no palette of her own
    ///     had the family's chitin tone about half the time.
    ///
    /// Every hook below was checked against the assembly:
    ///   * `Pawn_GeneTracker.AddGene` ends in `gene.PostAdd()`, so every path that hands a pawn a
    ///     gene runs <see cref="Gene_InsectColours"/>.
    ///   * `Pawn_GeneTracker.SetXenotype` sets `xenotype` and only then adds that xenotype's genes
    ///     through `AddGene`, so the roll already knows which caste she is.
    ///   * `PawnGenerator.GeneratePawn` rolls once more at the end of generation, so a generated
    ///     mamono - a daughter included - wears her own colour rather than her mother's.
    ///   * Nothing re-asserts the colour on load: `Pawn.story.HairColor` is saved with her, so a
    ///     colour the player picks in the styler is left alone, and a save is never recoloured.
    /// </summary>
    internal static class InsectColours
    {
        /// <summary>
        /// The colours her caste rolls between, or null when her caste names none - a caste still
        /// to be built, or a woman carrying the insect gene without one of the caste xenotypes.
        /// </summary>
        public static List<Color> Palette(Pawn pawn)
        {
            return pawn?.genes?.Xenotype?.GetModExtension<InsectHairColours>()?.colours;
        }

        /// <summary>True for a mamono who should be wearing a colour: she has the shared insect gene.</summary>
        public static bool IsInsectMamono(Pawn pawn)
        {
            return pawn?.genes != null && pawn.genes.HasActiveGene(InsectDefOf.PMM_Gene_Insect);
        }

        /// <summary>
        /// Gives her a freshly rolled colour from her caste, replacing whatever she was wearing.
        /// A caste with no palette is left alone rather than being handed somebody else's colour.
        /// </summary>
        public static void Roll(Pawn pawn)
        {
            if (!IsInsectMamono(pawn) || pawn?.story == null)
            {
                return;
            }

            List<Color> palette = Palette(pawn);
            if (palette == null || palette.Count == 0)
            {
                return;
            }

            pawn.story.HairColor = palette.RandomElement();
        }
    }

    /// <summary>
    /// The shared insect gene's own class. It is the marker that says "this woman is an insect
    /// caste", so it is the place that decides she wears a colour: a wild one rolled at
    /// generation, and a woman transformed into a caste rolled when her genes land.
    /// </summary>
    public class Gene_InsectColours : Gene
    {
        public override void PostAdd()
        {
            base.PostAdd();
            InsectColours.Roll(pawn);
        }
    }

    /// <summary>
    /// A mamono who is generated - a wild one, a raider, or a daughter born into the hive - rolls
    /// her colour here, last word on the matter. A daughter needs it: her mother's genes arrive
    /// with her, and this roll is what makes the colour her own.
    /// </summary>
    [HarmonyPatch(typeof(PawnGenerator), nameof(PawnGenerator.GeneratePawn), new[] { typeof(PawnGenerationRequest) })]
    public static class InsectColoursSpawnPatch
    {
        public static void Postfix(Pawn __result)
        {
            InsectColours.Roll(__result);
        }
    }

    /// <summary>
    /// A caste's hair and wing colours, declared on her own XenotypeDef.
    ///
    /// It lives there rather than here for the same reason her genes do: a caste's palette belongs
    /// with the caste. Adding a caste is therefore one list and no defs of its own.
    /// </summary>
    public class InsectHairColours : DefModExtension
    {
        public List<Color> colours;
    }
}
