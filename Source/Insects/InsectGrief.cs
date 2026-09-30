using HarmonyLib;
using RimWorld;
using Verse;

namespace PMM_Insects
{
    /// <summary>
    /// Marks a ThoughtDef as one this mod disables for insect mamono: the heavy half of grief, laid
    /// on twenty-two relation thoughts by Patches/InsectGrief.xml.
    ///
    /// It is a marker rather than vanilla's `ThoughtDef.nullifyingGenes` because that field is
    /// printed. `GeneDef.GetDescriptionFull` walks every thought naming a gene and lists the whole
    /// set on the gene's own tooltip as a `Mood:` block - "Removes: My daughter died: -23", once per
    /// relation, twenty-two of them on the insectoid gene (user's call 2026-09-29: "hide all the
    /// lines about relationship dying"). A mod extension is read by nothing the game ships, so the
    /// gene's tooltip has nothing to show.
    /// </summary>
    public class InsectGriefThought : DefModExtension
    {
    }

    /// <summary>
    /// Answers vanilla's nullifying-gene question for the marked thoughts, so the insect gene
    /// disables them without being written into their `nullifyingGenes` list.
    ///
    /// `ThoughtUtility.NullifyingGene` is the one place the game asks a gene whether it nullifies a
    /// thought: `Thought_Memory.MoodOffset` arrives there through `ThoughtUtility.ThoughtNullified`,
    /// and `ThoughtUtility.ThoughtNullifiedMessage` builds the memory's own "(Disabled by gene:
    /// insectoid)" line from it. A postfix here therefore reproduces the vanilla field exactly - the
    /// memory is still gained and still worth nothing - while leaving the gene's tooltip nothing to
    /// print. Checked in the 1.6 assembly: those two are the only readers of `nullifyingGenes`.
    /// </summary>
    [HarmonyPatch(typeof(ThoughtUtility), nameof(ThoughtUtility.NullifyingGene))]
    public static class InsectGriefNullifyingPatch
    {
        /// <summary>
        /// Marked thoughts only, and the gene is looked up last, so every other thought on every
        /// other pawn pays one null mod-extension check and nothing more.
        /// </summary>
        public static void Postfix(ThoughtDef def, Pawn pawn, ref Gene __result)
        {
            if (__result != null || def == null || !def.HasModExtension<InsectGriefThought>())
            {
                return;
            }
            if (pawn?.genes == null)
            {
                return;
            }
            __result = pawn.genes.GetGene(InsectDefOf.PMM_Gene_Insect);
        }
    }
}
