using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace PMM_Insects
{
    /// <summary>
    /// The castes' body colours, and the rule that a mamono wears exactly one of them.
    ///
    /// Three colours per caste, listed on the caste's own xenotype (<see cref="InsectSkinColours"
    /// below), and one of them rolled for her. Why a roll and not a list: these are mutually
    /// exclusive cosmetic genes - they all carry vanilla's `SkinColorOverride` tag - so a pawn
    /// holding several keeps one and shows the rest greyed out on her gene page. Vanilla answers
    /// the same problem the same way: its melanin genes are rolled per pawn, never listed.
    ///
    /// Every hook below was checked against the 1.6 assembly:
    ///   * `Pawn_GeneTracker.AddGene` ends in `gene.PostAdd()`, so every path that hands a pawn a
    ///     gene runs <see cref="Gene_InsectChitin"/>.
    ///   * `Pawn_GeneTracker.SetXenotype` sets `xenotype` and only then adds that xenotype's genes
    ///     through `AddGene`, so the roll already knows which caste she is.
    ///   * `PawnGenerator.GeneratePawn` rolls once more at the end of generation, so a generated
    ///     mamono - a daughter included - wears her own colour rather than her mother's.
    ///   * `Pawn_GeneTracker.ExposeData` does NOT call `PostAdd`, so a loaded pawn keeps the colour
    ///     she was saved with. The load hook below only fills a gap, which is what a save made
    ///     before the palettes existed hands us.
    /// </summary>
    internal static class InsectChitin
    {
        /// <summary>
        /// The colours her caste rolls between, or null when her caste names none - a caste still
        /// to be built, or a woman carrying the insect gene without one of the caste xenotypes.
        /// She keeps the shared chitin base tone on `PMM_Gene_Insect` in that case.
        /// </summary>
        public static List<GeneDef> Palette(Pawn pawn)
        {
            return pawn?.genes?.Xenotype?.GetModExtension<InsectSkinColours>()?.colours;
        }

        /// <summary>True for a mamono who should be wearing a colour: she has the shared insect gene.</summary>
        public static bool IsInsectMamono(Pawn pawn)
        {
            return pawn?.genes != null && pawn.genes.HasActiveGene(InsectDefOf.PMM_Gene_Insect);
        }

        /// <summary>
        /// Gives her freshly rolled colours from her caste, replacing whatever she was wearing.
        /// A caste with no palette is left alone: she keeps the shared base tone instead of
        /// taking a colour from somebody else's list.
        /// </summary>
        public static void Roll(Pawn pawn)
        {
            if (!IsInsectMamono(pawn))
            {
                return;
            }

            RollBody(pawn);
            RollHair(pawn);
        }

        /// <summary>One body colour from her caste's list.</summary>
        private static void RollBody(Pawn pawn)
        {
            List<GeneDef> palette = Palette(pawn);
            if (palette == null || palette.Count == 0)
            {
                return;
            }

            Strip(pawn);
            GeneDef colour = palette.RandomElement();
            if (colour != null)
            {
                pawn.genes.AddGene(colour, xenogene: false);
            }
        }

        /// <summary>
        /// One hair colour from her caste's list - and the wings come with it, because Big & Small
        /// paints every wing colour setting from `hairColor` (`BS_WingClr_A`/`_B` and their dark
        /// and saturated variants all say so).
        ///
        /// Colours rather than genes, and that is the one place this file does what vanilla does
        /// differently. A hair-colour gene joins the pool `PawnHairColors.HairColorGenes` builds
        /// from every GeneDef carrying a `hairColorOverride`, and `PawnGenerator` hands one of those
        /// to *any* pawn it generates - so genes of ours would have put mauve and rose hair on
        /// strangers. Setting `Pawn.story.HairColor` keeps the palette to the castes that name it,
        /// leaves the hair styler working (a player's restyle is just a different colour on the same
        /// field), and needs no row on her gene page. Nothing re-asserts it on load: it saves with
        /// her.
        /// </summary>
        private static void RollHair(Pawn pawn)
        {
            List<Color> palette = HairPalette(pawn);
            if (palette == null || palette.Count == 0 || pawn?.story == null)
            {
                return;
            }

            pawn.story.HairColor = palette.RandomElement();
        }

        /// <summary>Her caste's own hair colours, or null when it names none.</summary>
        private static List<Color> HairPalette(Pawn pawn)
        {
            return pawn?.genes?.Xenotype?.GetModExtension<InsectHairColours>()?.colours;
        }

        /// <summary>Takes every colour of ours off her, leaving the shared gene's base colour showing.</summary>
        public static void Strip(Pawn pawn)
        {
            foreach (Gene gene in Worn(pawn))
            {
                pawn.genes.RemoveGene(gene);
            }
        }

        /// <summary>
        /// Leaves her with one colour that belongs to her caste, rolling a new one only when she
        /// has none at all or hers came from a different caste's list - the pawn a save made
        /// before the palettes existed hands us, and the woman re-cast into another caste. A
        /// mamono already wearing her own colour is left exactly as she is, so loading a save
        /// never changes her body.
        /// </summary>
        public static void Ensure(Pawn pawn)
        {
            if (!IsInsectMamono(pawn))
            {
                return;
            }

            List<GeneDef> palette = Palette(pawn);
            if (palette == null || palette.Count == 0)
            {
                return;
            }

            List<Gene> worn = Worn(pawn);
            if (worn.Count == 1 && palette.Contains(worn[0].def))
            {
                return;
            }

            Roll(pawn);
        }

        /// <summary>
        /// The colour genes of ours she is carrying, active or overridden. The class is the marker,
        /// so one can be recognised - and taken off her - without consulting any caste's list.
        /// </summary>
        private static List<Gene> Worn(Pawn pawn)
        {
            List<Gene> worn = new List<Gene>();
            if (pawn?.genes == null)
            {
                return worn;
            }

            foreach (Gene gene in pawn.genes.GenesListForReading)
            {
                if (gene is Gene_InsectTone)
                {
                    worn.Add(gene);
                }
            }
            return worn;
        }
    }

    /// <summary>
    /// The shared insect gene's own class. It is the marker that says "this woman is an insect
    /// caste", so it is the place that decides she wears a colour: a wild one rolled at
    /// generation, a woman transformed into a caste rolled when her genes land, and a caste
    /// turning back into something else losing the colour with the gene.
    /// </summary>
    public class Gene_InsectChitin : Gene
    {
        public override void PostAdd()
        {
            base.PostAdd();
            InsectChitin.Roll(pawn);
        }

        public override void PostRemove()
        {
            base.PostRemove();
            InsectChitin.Strip(pawn);
        }
    }

    /// <summary>
    /// Every colour gene's own class - and it is only that, a marker. `InsectChitin` recognises
    /// the castes' colours by it, so one can be taken off a pawn without consulting any caste's
    /// list; a future colour that forgets to name the class would be invisible to the roll. The
    /// colour itself is pure `Gene` behaviour: vanilla paints the pawn from the def's
    /// `skinColorOverride`, and nothing needs to run when the gene lands.
    /// </summary>
    public class Gene_InsectTone : Gene
    {
    }

    /// <summary>
    /// A mamono who is generated - a wild one, a raider, or a daughter born into the hive - rolls
    /// her tone here, last word on the matter. A daughter needs it: her mother's tone arrives with
    /// her inherited genes, so without this she would wear her mother's colour.
    /// </summary>
    [HarmonyPatch(typeof(PawnGenerator), nameof(PawnGenerator.GeneratePawn), new[] { typeof(PawnGenerationRequest) })]
    public static class InsectChitinSpawnPatch
    {
        public static void Postfix(Pawn __result)
        {
            InsectChitin.Roll(__result);
        }
    }

    /// <summary>
    /// A caste's three body colours, declared on her own XenotypeDef.
    ///
    /// It lives there rather than here for the same reason her genes do: a caste's palette belongs
    /// with the caste. Adding a caste is therefore three gene defs and one list, and no code.
    /// </summary>
    public class InsectSkinColours : DefModExtension
    {
        public List<GeneDef> colours;
    }

    /// <summary>
    /// A caste's hair and wing colours, declared on her own XenotypeDef beside her body palette.
    /// Plain colours rather than gene defs, and `InsectChitin.RollHair` carries the reasoning for
    /// that difference.
    /// </summary>
    public class InsectHairColours : DefModExtension
    {
        public List<Color> colours;
    }

    /// <summary>
    /// Gives a colour to a mamono who has none of her caste's: the pawn a save made before the
    /// palettes existed hands us, and the woman re-cast into another caste. Runs once per pawn per
    /// load, at the point vanilla's own gene tracker has finished loading, and leaves every other
    /// pawn in the save alone after one look at her gene list.
    /// </summary>
    [HarmonyPatch(typeof(Pawn_GeneTracker), nameof(Pawn_GeneTracker.ExposeData))]
    public static class InsectChitinLoadPatch
    {
        public static void Postfix(Pawn_GeneTracker __instance)
        {
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                InsectChitin.Ensure(__instance.pawn);
            }
        }
    }

    /// <summary>
    /// The same rule when she changes caste in game rather than on load: her old colour belongs to
    /// somebody else's list, so it is replaced by one of her new caste's.
    /// </summary>
    [HarmonyPatch(typeof(Pawn_GeneTracker), nameof(Pawn_GeneTracker.SetXenotype))]
    public static class InsectChitinXenotypePatch
    {
        public static void Postfix(Pawn_GeneTracker __instance)
        {
            InsectChitin.Ensure(__instance.pawn);
        }
    }
}
