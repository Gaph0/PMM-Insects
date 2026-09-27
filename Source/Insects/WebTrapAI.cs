using RimWorld;
using Verse;

namespace PMM_Insects
{
    /// <summary>
    /// Makes the tribe's arachnes actually throw their web (user's call 2026-09-27: "give enemy
    /// arachnes the ability too"). Until now only the player could fire it - `aiCanUse` was false and
    /// nothing in any think tree named the ability. Vanilla's `JobGiver_AICastAbility` cannot be
    /// dropped into a tree on its own because it is abstract: it has `GetTarget` and no implementation.
    /// This is that implementation - her current enemy, in range, in sight.
    ///
    /// Restricted to pawns hostile to the player on purpose: the request was for the tribe's arachnes,
    /// and a player's own arachne stays a button the player presses. Widening it later is one line.
    ///
    /// The cooldown is honoured for free. The base giver returns null while `Ability.CanCast` is false
    /// and skips a pawn who is already casting this ability, so she throws one web and then fights on
    /// while it recharges. `Verb_WebTrap` remains the second line of defence if anything ever bypasses
    /// this path - that is the class the "fires while on cooldown" bug was fixed in.
    ///
    /// `Patches/ArachneWeb_EnemyUse.xml` puts it in the humanlike think tree, gated on Alpha Animals,
    /// because the ability def only exists when AA does.
    /// </summary>
    public class JobGiver_WebTrapTarget : JobGiver_AICastAbility
    {
        protected override LocalTargetInfo GetTarget(Pawn caster, Ability ability)
        {
            if (caster == null || !caster.HostileTo(Faction.OfPlayer))
            {
                return LocalTargetInfo.Invalid;
            }
            Thing target = caster.mindState?.enemyTarget;
            if (target == null || !target.Spawned || target.Map != caster.Map || target == caster)
            {
                return LocalTargetInfo.Invalid;
            }
            float range = ability?.verb?.verbProps?.range ?? 0f;
            if (range > 0f && caster.Position.DistanceTo(target.Position) > range)
            {
                return LocalTargetInfo.Invalid;
            }
            if (!GenSight.LineOfSight(caster.Position, target.Position, caster.Map))
            {
                return LocalTargetInfo.Invalid;
            }
            return target;
        }
    }
}
