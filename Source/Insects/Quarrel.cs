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
        /// The quarrel gene on this pawn, or null. One lookup for the worker, the shape the honey
        /// gene uses, so nothing reaches a caste by name.
        /// </summary>
        public static Gene_Quarrel Get(Pawn pawn)
        {
            if (pawn?.genes == null)
            {
                return null;
            }
            foreach (Gene gene in pawn.genes.GenesListForReading)
            {
                if (gene is Gene_Quarrel quarrel && gene.Active)
                {
                    return quarrel;
                }
            }
            return null;
        }
    }

    /// <summary>
    /// Answers vanilla's social-thought question for the quarrel gene.
    /// `Thought_SituationalSocial.CurrentStateInternal` calls `def.Worker.CurrentSocialState(pawn,
    /// otherPawn)` once per other pawn, and only a def whose `thoughtClass` is that class is
    /// collected per pawn at all - which is why both quarrel thought defs name it.
    /// </summary>
    public class ThoughtWorker_Quarrel : ThoughtWorker
    {
        protected override ThoughtState CurrentSocialStateInternal(Pawn p, Pawn otherPawn)
        {
            Gene_Quarrel quarrel = Gene_Quarrel.Get(p);
            if (quarrel == null || !quarrel.Dislikes(otherPawn))
            {
                return ThoughtState.Inactive;
            }
            return ThoughtState.ActiveAtStage(0);
        }
    }
}
