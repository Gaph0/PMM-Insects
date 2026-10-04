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
    /// Every race is looked up by name, so a name that is not loaded would simply be skipped.
    /// Nothing here is conditional any more: the three species that were once gated on a DLC are
    /// registered exactly like the base-game ones, and the abaddon folk never had a gate at all.
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
                "PMM_Race_AbaddonFolk",
                // The four species built after that list was written were never added to it, so
                // their corpses sat under vanilla's humanlike line while every older sister's
                // moved. Straightened out 2026-09-27, in the pass that added the honey bee.
                "PMM_Race_Arachne",
                "PMM_Race_Beelzebub",
                "PMM_Race_Girtablilu",
                "PMM_Race_AntArachne",
                "PMM_Race_HoneyBee",
                // The two built 2026-10-01 (archive/CASTES-PLAN.md phase 2), for the same reason as the
                // five above: a caste added without a line here files her corpse with the humans.
                "PMM_Race_Mothman",
                "PMM_Race_Papillon",
                // The hornet, built with them 2026-10-01 (archive/CASTES-PLAN.md phase 3).
                "PMM_Race_Hornet",
                // The mantis, 2026-10-01 (archive/CASTES-PLAN.md phase 4): a swarm caste, filed with her
                // sisters all the same - the list is every race this mod ships, not every friendly
                // one.
                "PMM_Race_Mantis");
        }
    }
}
