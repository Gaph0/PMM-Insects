using ProjectMamono;
using Verse;

namespace PMM_Insects
{
    /// <summary>
    /// Puts every insect mamono corpse under the family's shared "mamono corpses" line.
    ///
    /// The category def and all of the moving live in the core mod now
    /// (ProjectMamono.MamonoCorpses, Defs/ThingCategoryDefs/ThingCategories_MamonoCorpses.xml), so
    /// the insects, the slimes and the elementals share one line instead of one line each.
    /// See MamonoCorpses for why a corpse's category cannot be set in XML.
    ///
    /// The three Odyssey races are skipped when the DLC is absent: MamonoCorpses looks every
    /// race up by name and a missing one is harmless. The abaddon folk is not gated - she
    /// has no DLC behind her, only the mod's own defs.
    /// </summary>
    [StaticConstructorOnStartup]
    public static class InsectMamonoCorpses
    {
        static InsectMamonoCorpses()
        {
            MamonoCorpses.Register(
                "PMM_Race_DevilBug",
                "PMM_Race_GiantAnt",
                "PMM_Race_SoldierBeetle",
                "PMM_Race_Greenworm",
                "PMM_Race_VampMosquito",
                "PMM_Race_Abaddon",
                "PMM_Race_AbaddonFolk");
        }
    }
}
