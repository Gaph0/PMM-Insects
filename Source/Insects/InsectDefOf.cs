using RimWorld;
using Verse;

namespace PMM_Insects
{
    /// <summary>
    /// The job defs this mod's own code has to name. Both orders are player-facing - the arachne's
    /// silk and the bee's honey - and both are reached through the feature that owns the state, so
    /// a new spinning or honey-making caste needs no entry here.
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
    }
}
