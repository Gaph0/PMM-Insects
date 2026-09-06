using System.Reflection;
using HarmonyLib;
using Verse;

namespace PMM_Insects
{
    /// <summary>
    /// Entry point. Applies Harmony patches when the mod loads.
    /// </summary>
    [StaticConstructorOnStartup]
    public static class InsectsMod
    {
        static InsectsMod()
        {
            var harmony = new Harmony("PMM.Insects");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}
