using RimWorld;
using Verse;

namespace PMM_Insects
{
    /// <summary>
    /// The defs this mod's own code has to name: the two player-facing orders (the arachne's silk
    /// and the bee's honey, both reached through the feature that owns the state, so a new spinning
    /// or honey-making caste needs no entry here), the shared insect gene, and the light gene's
    /// hidden hediff.
    ///
    /// The insect gene is named by `InsectColours.cs`, which rolls one of her caste's colours onto
    /// every insect mamono as her hair colour. The colours themselves are not named here: each
    /// caste lists her own three on her xenotype (`InsectHairColours`), so a new caste adds nothing
    /// to this class. That same gene
    /// field is what the grief postfix in `InsectGrief.cs` asks a pawn for. The hediff below is
    /// named by `DisorientatingLights.cs`, which adds and removes it as the light she stands in
    /// changes.
    ///
    /// A `[DefOf]` field whose def is missing is a load error, so nothing goes in this class that
    /// is not shipped by this mod.
    /// </summary>
    [DefOf]
    public static class InsectDefOf
    {
        /// <summary>The job that gathers the silk, from her own hands or another's.</summary>
        public static JobDef PMM_GatherArachneSilk;

        /// <summary>The job that takes the honey, from her own hands or another's.</summary>
        public static JobDef PMM_MilkHoney;

        /// <summary>The shared insect gene: the marker that says a woman is an insect caste.</summary>
        public static GeneDef PMM_Gene_Insect;

        /// <summary>The hidden marker hediff of the light gene, worn while she stands in the light.</summary>
        public static HediffDef PMM_Hediff_DisorientatingLights;
    }
}
