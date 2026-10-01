using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace PMM_Insects
{
    /// <summary>
    /// Wild insectoids leave a mamono alone unless she gives them a reason.
    ///
    /// Vanilla Races Expanded ships this behaviour as its own gene,
    /// VRE_InsectPheromones - but the gene is only a flag: the work is done by two
    /// Harmony patches in that mod's assembly, which check
    /// HasActiveGene(VRE_InsectPheromones) and then tell the insect that the pawn
    /// is not a valid target. So the mamonos used to need two genes to say one thing:
    /// PMM_Gene_Insect for their body, and VRE's gene for the insects' behaviour.
    ///
    /// This file folds that flag into PMM_Gene_Insect, so a mamono is entire on her
    /// own. The two patches below are a faithful copy of VRE's, with the gene
    /// lookup swapped from theirs to ours:
    ///
    ///   * AttackTargetFinder.BestAttackTarget - a postfix on the compiler
    ///     generated validator nested inside that method, so an insect never
    ///     picks a pheromone carrier as a target in the first place.
    ///   * Pawn.ThreatDisabledBecauseNonAggressiveRoamer - so a roaming insect
    ///     that already has a target drops it for a carrier.
    ///
    /// The cost of owning this: if VRE ever changes how their pheromones work, our
    /// copy does not follow. The check itself is short and has been stable, and
    /// VRE's own copy still runs for anyone carrying VRE's gene, so the two do not
    /// fight over the same pawn.
    ///
    /// "Unless she gives them a reason" is the NotProvoked helper: a mamono who has
    /// the insect as her current target, a mamono in a colony the insect's lord is
    /// assaulting, or any pairing where the two are already enemies of each other's
    /// faction or in the same lord, is fair game again.
    /// </summary>
    public static class InsectPheromones
    {
        /// <summary>
        /// Looked up by defName rather than through a DefOf class (so a load-order or rename problem
        /// cannot throw during static constructor resolution - the same choice PollutedSettlement
        /// makes), and looked up fresh on every call rather than kept: defs compare by reference, so a
        /// def held across a def database rebuild matches no gene on any pawn and the pheromones would
        /// quietly stop working. One dictionary lookup, on a check that is already far cheaper than the
        /// attack search around it.
        /// </summary>
        private static GeneDef Gene => DefDatabase<GeneDef>.GetNamedSilentFail("PMM_Gene_Insect");

        /// <summary>True for a pawn who wears the insectoid gene, mamono or not.</summary>
        public static bool Carries(Pawn pawn)
        {
            GeneDef gene = Gene;
            return gene != null && pawn != null && pawn.genes != null && pawn.genes.HasActiveGene(gene);
        }

        /// <summary>
        /// The equivalent of VRE's CheckHostility. Returns whether the attack is
        /// still allowed: false means the insect should not bother her. VRE threads its
        /// caller's own answer in and back out again; the answer is the return value
        /// here, so a re-diff against their copy is the one line that dropped it.
        /// </summary>
        public static bool CheckHostility(Pawn pawn1, Pawn pawn2)
        {
            if (pawn1 == null || pawn2 == null)
            {
                return true;
            }
            if (pawn1.RaceProps.Insect && Carries(pawn2) && NotProvoked(pawn2, pawn1))
            {
                return false;
            }
            if (pawn2.RaceProps.Insect && Carries(pawn1) && NotProvoked(pawn1, pawn2))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// True while the insect has no reason to dislike her: she is not hunting
        /// it, and the two are not already enemies. Copied from VRE's
        /// PreventHostility, with a name that reads the other way round.
        /// </summary>
        private static bool NotProvoked(Pawn humanlike, Pawn insect)
        {
            return !(humanlike.mindState.enemyTarget == insect
                || insect.mindState.enemyTarget == humanlike
                || (insect.GetLord()?.LordJob is LordJob_AssaultColony && humanlike.IsColonist)
                || TargetsFactionOf(humanlike.mindState.enemyTarget, insect)
                || TargetsFactionOf(insect.mindState.enemyTarget, humanlike)
                || TargetsFactionOf(humanlike.mindState.lastAttackedTarget.Thing, insect)
                || TargetsFactionOf(insect.mindState.lastAttackedTarget.Thing, humanlike)
                || SharesLordWith(humanlike.mindState.enemyTarget, insect)
                || SharesLordWith(insect.mindState.enemyTarget, humanlike)
                || SharesLordWith(humanlike.mindState.lastAttackedTarget.Thing, insect)
                || SharesLordWith(insect.mindState.lastAttackedTarget.Thing, humanlike));
        }

        private static bool TargetsFactionOf(Thing enemyTarget, Pawn otherPawn)
        {
            return enemyTarget != null && otherPawn.Faction != null && enemyTarget.Faction == otherPawn.Faction;
        }

        private static bool SharesLordWith(Thing enemyTarget, Pawn otherPawn)
        {
            return enemyTarget is Pawn other && other.GetLord() == otherPawn.GetLord();
        }
    }

    /// <summary>
    /// Every validator nested inside AttackTargetFinder.BestAttackTarget that takes one IAttackTarget
    /// - the filter the method builds and then wraps in filters of its own. The names are compiler
    /// generated, so they are found by shape, and all of them are patched rather than the first one
    /// found: they live in the same compiler-generated class beside methods of the same shape, and
    /// picking one by the order the compiler emitted it in is a bet that a game patch can silently
    /// lose - the patch would land on a wrapper, or on nothing, and the gene would stop meaning
    /// anything. The postfix only ever removes a candidate, so covering the whole family costs a
    /// repeated check on the same pair and buys independence from that order.
    ///
    /// A class without a `searcherThing` field is skipped: the postfix reads that field, and it is the
    /// pawn the validator is being asked about.
    /// </summary>
    [HarmonyPatch]
    public static class Patch_BestAttackTarget_InsectPheromones
    {
        public static IEnumerable<MethodBase> TargetMethods()
        {
            List<MethodBase> targets = new List<MethodBase>();
            foreach (Type type in typeof(AttackTargetFinder).GetNestedTypes(AccessTools.all))
            {
                if (type.GetField("searcherThing", AccessTools.all) == null)
                {
                    continue;
                }
                foreach (MethodInfo method in type.GetMethods(AccessTools.all))
                {
                    if (method.Name.Contains("<BestAttackTarget>")
                        && method.GetParameters().Length == 1
                        && method.GetParameters()[0].ParameterType == typeof(IAttackTarget))
                    {
                        targets.Add(method);
                    }
                }
            }
            if (targets.Count == 0)
            {
                // Harmony patches nothing at all when this list is empty, so say it here instead:
                // a patch that quietly did not apply is the one outcome nobody notices until the
                // gene looks broken.
                Log.Error("[PMM Insects] InsectPheromones: no AttackTargetFinder.BestAttackTarget "
                    + "validator was found; the insect gene no longer stops insects attacking her.");
                return targets;
            }
            if (Prefs.DevMode)
            {
                Log.Message("[PMM Insects] InsectPheromones patched " + targets.Count
                    + " target validators: " + string.Join(", ", targets.Select(m => m.Name)));
            }
            return targets;
        }

        /// <summary>
        /// The candidate arrives as `__0`: by position, not by name. Harmony binds a plain parameter by
        /// its *name*, and the three validators do not agree on one - the compiler named the single
        /// parameter of two of them `t` and of the third `x`. A postfix asking for `t` therefore failed
        /// to build for the third (log, 2026-10-01: `Parameter "t" not found in method ...b__3(IAttackTarget
        /// x)`), and that failure took the whole patch set with it, because `Harmony.PatchAll` throws at
        /// the first class it cannot build. `__0` is the first parameter whatever the compiler called it,
        /// and the selector above only ever hands over methods that have exactly one.
        /// </summary>
        public static void Postfix(ref bool __result, IAttackTarget __0, Thing ___searcherThing)
        {
            if (__result)
            {
                __result = InsectPheromones.CheckHostility(__0.Thing as Pawn, ___searcherThing as Pawn);
            }
        }
    }

    /// <summary>
    /// The roaming half: an insect that is already heading for her drops the target.
    /// </summary>
    [HarmonyPatch(typeof(Pawn), "ThreatDisabledBecauseNonAggressiveRoamer")]
    public static class Patch_ThreatDisabled_InsectPheromones
    {
        public static void Postfix(ref bool __result, Pawn __instance, Pawn otherPawn)
        {
            if (!__result && !InsectPheromones.CheckHostility(__instance, otherPawn))
            {
                __result = true;
            }
        }
    }
}
