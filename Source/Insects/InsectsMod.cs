using System;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using Verse;

namespace PMM_Insects
{
    /// <summary>
    /// Entry point. Applies Harmony patches when the mod loads.
    ///
    /// One patch class at a time rather than `Harmony.PatchAll`, which throws at the first class it
    /// cannot build and never reaches the ones after it. On 2026-10-01 one method that would not bind
    /// (see the postfix in `InsectPheromones.cs`) stopped every patch class declared after it, and the
    /// throw surfaced as a `TypeInitializationException` naming this constructor rather than the patch.
    /// Here a class that fails is named in the log and the others still apply.
    /// </summary>
    [StaticConstructorOnStartup]
    public static class InsectsMod
    {
        static InsectsMod()
        {
            var harmony = new Harmony("PMM.Insects");
            Type[] patchClasses = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(type => type.GetCustomAttributes(typeof(HarmonyPatch), true).Length > 0)
                .ToArray();
            foreach (Type patchClass in patchClasses)
            {
                try
                {
                    harmony.CreateClassProcessor(patchClass).Patch();
                }
                catch (Exception exception)
                {
                    // Harmony has already logged the method it could not build; this line says which
                    // of our classes it was, and that the rest of them are on.
                    Log.Error($"[PMM Insects] {patchClass.Name} did not apply; the other patch "
                        + $"classes did. {exception}");
                }
            }
        }
    }
}
