using RimWorld;
using Verse;

namespace PMM_Insects
{
    /// <summary>
    /// The web trap's verb, and the reason it exists at all: nothing on the normal cast path asks the
    /// ability whether it is on cooldown. `Ability.Activate` only applies effects, `Toils_Combat.CastVerb`
    /// hands the cast to `Verb.TryStartCastOn`, and that one checks the caster, the verb's own state and
    /// the shot line - it never reads `Ability.CanCast`, the only property that reports the cooldown.
    /// Just the gizmo and `JobGiver_AICastAbility` consult it.
    ///
    /// The user hit the consequence twice. First webs kept flying through a 600-tick cooldown; then,
    /// once the shot itself was refused, the same rhythm of *attempts* with no web leaving - because
    /// refusing the shot still let the cast start, warm up and play its sound. Their own log settled it:
    /// ten refusals, 508/416/324/232 ticks left, about 92 ticks apart, the stack running
    /// `Stance_Warmup.Expire -> Verb.WarmupComplete -> TryCastNextBurstShot -> TryCastShot`. So something
    /// starts a fresh cast every warmup, and the refusal has to happen at the start.
    ///
    /// `TryStartCastOn` is therefore refused first, and the shot guard stays as the second line for the
    /// Harmony-patched `Verb.TryCastNextBurstShot` that shows up in that stack. Both read
    /// `Ability.CooldownTicksRemaining`; `Verb.Available()` is deliberately not used, because
    /// `TryStartCastOn` never asks it - only `TryCastNextBurstShot` does, once per shot. One web per
    /// cast, so neither guard can eat a shot belonging to the cast that just fired.
    /// </summary>
    public class Verb_WebTrap : Verb_AbilityShoot
    {
        /// <summary>
        /// Refuses the cast before it starts, so an attempt during cooldown costs nothing at all: no
        /// warmup stance, no sound, no stance lock, nothing for the player to see.
        /// </summary>
        public override bool TryStartCastOn(LocalTargetInfo castTarg, LocalTargetInfo destTarg,
            bool surpriseAttack = false, bool canHitNonTargetPawns = true, bool preventFriendlyFire = false,
            bool nonInterruptingSelfCast = false)
        {
            int remaining = Ability?.CooldownTicksRemaining ?? 0;
            if (remaining > 0)
            {
                return false;
            }
            return base.TryStartCastOn(castTarg, destTarg, surpriseAttack, canHitNonTargetPawns,
                preventFriendlyFire, nonInterruptingSelfCast);
        }

        /// <summary>Second line: anything that reaches a shot without going through the cast start.</summary>
        protected override bool TryCastShot()
        {
            int remaining = Ability?.CooldownTicksRemaining ?? 0;
            if (remaining > 0)
            {
                return false;
            }
            return base.TryCastShot();
        }
    }
}
