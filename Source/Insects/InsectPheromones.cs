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
    /// Wild insectoids leave a momo alone unless she gives them a reason.
    ///
    /// Vanilla Races Expanded ships this behaviour as its own gene,
    /// VRE_InsectPheromones - but the gene is only a flag: the work is done by two
    /// Harmony patches in that mod's assembly, which check
    /// HasActiveGene(VRE_InsectPheromones) and then tell the insect that the pawn
    /// is not a valid target. So the momos used to need two genes to say one thing:
    /// PMM_Gene_Insect for their body, and VRE's gene for the insects' behaviour.
    ///
    /// This file folds that flag into PMM_Gene_Insect, so a momo is entire on her
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
    /// "Unless she gives them a reason" is the NotProvoked helper: a momo who has
    /// the insect as her current target, a momo in a colony the insect's lord is
    /// assaulting, or any pairing where the two are already enemies of each other's
    /// faction or in the same lord, is fair game again.
    /// </summary>
    public static class InsectPheromones
    {
        private static GeneDef cachedGene;

        /// <summary>
        /// Looked up by defName rather than through a DefOf class, so a load-order
        /// or rename problem cannot throw during static constructor resolution -
        /// the same choice PollutedSettlement makes.
        /// </summary>
        private static GeneDef Gene
        {
            get
            {
                if (cachedGene == null)
                {
                    cachedGene = DefDatabase<GeneDef>.GetNamedSilentFail("PMM_Gene_Insect");
                }
                return cachedGene;
            }
        }

        /// <summary>True for a pawn who wears the insectoid gene, momo or not.</summary>
        public static bool Carries(Pawn pawn)
        {
            GeneDef gene = Gene;
            return gene != null && pawn != null && pawn.genes != null && pawn.genes.HasActiveGene(gene);
        }

        /// <summary>
        /// The equivalent of VRE's CheckHostility. Returns whether the attack is
        /// still allowed: false means the insect should not bother her.
        /// </summary>
        public static bool CheckHostility(bool result, Pawn pawn1, Pawn pawn2)
        {
            if (result && pawn1 != null && pawn2 != null)
            {
                if (pawn1.RaceProps.Insect && Carries(pawn2) && NotProvoked(pawn2, pawn1))
                {
                    result = false;
                }
                else if (pawn2.RaceProps.Insect && Carries(pawn1) && NotProvoked(pawn1, pawn2))
                {
                    result = false;
                }
            }
            return result;
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
    /// The validator nested inside AttackTargetFinder.BestAttackTarget. Its name is
    /// compiler generated, so VRE finds it by shape and so do we: the method that
    /// takes exactly one IAttackTarget.
    /// </summary>
    [HarmonyPatch]
    public static class Patch_BestAttackTarget_InsectPheromones
    {
        public static MethodBase TargetMethod()
        {
            foreach (System.Type type in typeof(AttackTargetFinder).GetNestedTypes(AccessTools.all))
            {
                MethodInfo method = type.GetMethods(AccessTools.all).FirstOrDefault(x =>
                    x.Name.Contains("<BestAttackTarget>")
                    && x.GetParameters().Length == 1
                    && x.GetParameters()[0].ParameterType == typeof(IAttackTarget));
                if (method != null)
                {
                    return method;
                }
            }
            return null;
        }

        public static void Postfix(ref bool __result, IAttackTarget t, Thing ___searcherThing)
        {
            if (__result)
            {
                __result = InsectPheromones.CheckHostility(
                    true, t.Thing as Pawn, ___searcherThing as Pawn);
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
            if (!__result && !InsectPheromones.CheckHostility(true, __instance, otherPawn))
            {
                __result = true;
            }
        }
    }
}
