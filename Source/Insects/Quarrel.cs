using RimWorld;
using Verse;

namespace PMM_Insects
{
    /// <summary>
    /// The dial of the quarrel gene: whose presence she cannot stand. It is named on the gene def
    /// (Defs/GeneDefs/Genes_Quarrel.xml), so a new grudge is one more def and no new code - the
    /// soldier beetle points it at her own gene, the hornet at the honey gene.
    /// </summary>
    public class QuarrelGeneExtension : DefModExtension
    {
        /// <summary>The gene whose carriers she dislikes. Null on a half-written def.</summary>
        public GeneDef dislikedGene;
    }

    /// <summary>
    /// Names the quarrel gene a thought def speaks for.
    ///
    /// Both quarrel thoughts share one worker, and until 2026-10-03 that worker answered with
    /// whichever quarrel gene it found first on her. One dial therefore drove both thoughts: a
    /// woman with the honey grudge fired the kin thought as well, and a woman with the kin grudge
    /// fired the honey one, so two -50s landed where one was intended. A hornet read -100 toward a
    /// honey bee with fifty of it labelled "quarrelsome kin", and soldier beetles read -100 toward
    /// each other instead of -50. Each def below binds its thought to one gene, which keeps the two
    /// apart and keeps the rule that a new grudge is one more def and no new code.
    /// </summary>
    public class QuarrelThoughtExtension : DefModExtension
    {
        /// <summary>The gene this thought answers for. Null on a half-written def.</summary>
        public GeneDef gene;
    }

    /// <summary>
    /// The quarrel gene: a marker with a dial. The opinion itself is the thought's
    /// `baseOpinionOffset`, and the fights follow from it for free - `Pawn_InteractionsTracker`
    /// scales the social-fight chance by opinion (x2.5 at -50, x4 at -100) - so nothing in this file
    /// touches the fight roll, and nothing needs a Harmony patch.
    /// </summary>
    public class Gene_Quarrel : Gene
    {
        public GeneDef DislikedGene => def.GetModExtension<QuarrelGeneExtension>()?.dislikedGene;

        public bool Dislikes(Pawn other)
        {
            if (other == null || other == pawn || other.genes == null)
            {
                return false;
            }
            GeneDef disliked = DislikedGene;
            return disliked != null && other.genes.HasActiveGene(disliked);
        }

        /// <summary>
        /// The named quarrel gene on this pawn, or null - named rather than merely "a quarrel
        /// gene", because one thought def answers for one gene and one dial. The honey gene uses
        /// the same shape, so nothing reaches a caste by name.
        /// </summary>
        public static Gene_Quarrel Get(Pawn pawn, GeneDef geneDef)
        {
            if (pawn?.genes == null || geneDef == null)
            {
                return null;
            }
            foreach (Gene gene in pawn.genes.GenesListForReading)
            {
                if (gene is Gene_Quarrel quarrel && gene.def == geneDef && gene.Active)
                {
                    return quarrel;
                }
            }
            return null;
        }
    }

    /// <summary>
    /// Answers vanilla's social-thought question for the one quarrel gene its own thought def names
    /// (`QuarrelThoughtExtension`).
    /// `Thought_SituationalSocial.CurrentStateInternal` calls `def.Worker.CurrentSocialState(pawn,
    /// otherPawn)` once per other pawn, and only a def whose `thoughtClass` is that class is
    /// collected per pawn at all - which is why both quarrel thought defs name it.
    /// </summary>
    public class ThoughtWorker_Quarrel : ThoughtWorker
    {
        protected override ThoughtState CurrentSocialStateInternal(Pawn p, Pawn otherPawn)
        {
            GeneDef geneDef = def.GetModExtension<QuarrelThoughtExtension>()?.gene;
            Gene_Quarrel quarrel = Gene_Quarrel.Get(p, geneDef);
            if (quarrel == null || !quarrel.Dislikes(otherPawn))
            {
                return ThoughtState.Inactive;
            }
            return ThoughtState.ActiveAtStage(0);
        }
    }
}
