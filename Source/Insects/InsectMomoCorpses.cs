using ProjectMomo;
using Verse;

namespace PMM_Insects
{
    /// <summary>
    /// Puts the six insect momo corpses under the family's shared "momo corpses" line.
    ///
    /// The category def and all of the moving live in the core mod now
    /// (ProjectMomo.MomoCorpses, Defs/ThingCategoryDefs/ThingCategories_MomoCorpses.xml), so
    /// the insects, the slimes and the elementals share one line instead of one line each.
    /// See MomoCorpses for why a corpse's category cannot be set in XML.
    ///
    /// The three Odyssey races are skipped when the DLC is absent: MomoCorpses looks every
    /// race up by name and a missing one is harmless.
    /// </summary>
    [StaticConstructorOnStartup]
    public static class InsectMomoCorpses
    {
        static InsectMomoCorpses()
        {
            MomoCorpses.Register(
                "PMM_Race_DevilBug",
                "PMM_Race_GiantAnt",
                "PMM_Race_SoldierBeetle",
                "PMM_Race_Greenworm",
                "PMM_Race_VampMosquito",
                "PMM_Race_Abaddon");
        }
    }
}
