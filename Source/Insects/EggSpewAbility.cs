using RimWorld;
using UnityEngine;
using Verse;

namespace PMM_Insects
{
    /// <summary>
    /// The egg spew's button, and since 2026-09-26 the brood picker as well (the user's call:
    /// "merge the option to choose which mamono the Abaddon lays and the spawn egg gizmo").
    ///
    /// Two gizmos used to sit on the queen: vanilla's egg spew button, and a second button from
    /// CompBroodOrder that opened the caste menu. They described one act, so they are one button
    /// now, and the menu comes first: press, choose the daughter, then the targeting cursor, then
    /// she lays. The order is written before the cursor opens, so choosing a caste and then
    /// cancelling the cursor keeps the new order and costs no cooldown - a cast is what starts the
    /// cooldown, and cancelling is not one.
    ///
    /// Installed with one XML line, `&lt;gizmoClass&gt;` on the ability def: AbilityDef has that field
    /// and vanilla's own Command_AbilitySpeech uses it the same way (same (Ability, Pawn)
    /// constructor, same Tooltip override). No patch is needed, and none is possible on the other
    /// side - Ability itself has no virtual gizmo member to override.
    /// </summary>
    public class Command_Ability_EggSpew : Command_Ability
    {
        public Command_Ability_EggSpew(Ability ability, Pawn pawn)
            : base(ability, pawn)
        {
        }

        /// <summary>
        /// The queen's brood comp, or null when she has none - a pawn wearing the xenotype without
        /// the race comp, or defs that failed to load. Null degrades this to a plain egg spew
        /// button rather than a broken one.
        /// </summary>
        private CompBroodOrder Brood => ability?.pawn?.GetComp<CompBroodOrder>();

        /// <summary>
        /// Disabled for somebody else's queen, exactly as the old picker was: an order she cannot
        /// be given is pointless, and a button that vanishes reads as a bug. The tooltip below
        /// carries the same reason string the picker used.
        /// </summary>
        public override bool Disabled
        {
            get => base.Disabled || Brood?.CanOrder == false;
            set => base.Disabled = value;
        }

        /// <summary>
        /// The ability's own tooltip plus what she is currently told to lay. That line is the old
        /// picker's label doing the job it always did - telling the player the standing order at a
        /// glance - now that no separate button carries it.
        /// </summary>
        public override string Tooltip
        {
            get
            {
                string text = base.Tooltip;
                CompBroodOrder brood = Brood;
                if (brood == null)
                {
                    return text;
                }
                text += "\n\n" + "PMM_BroodGizmoLabel".Translate(brood.CurrentLabel).Resolve();
                if (!brood.CanOrder)
                {
                    text += "\n" + "PMM_BroodNotMine".Translate().Resolve();
                }
                return text + "\n" + "PMM_BroodGizmoDesc".Translate().Resolve();
            }
        }

        /// <summary>
        /// Ask for the brood first, then hand over to vanilla, which opens the targeting cursor
        /// from `ability.verb` (Command_Ability.ProcessInput). With no brood comp, or nobody who
        /// may give her an order, or nothing in the menu to pick, this is a plain press of the
        /// egg spew button.
        /// </summary>
        public override void ProcessInput(Event ev)
        {
            CompBroodOrder brood = Brood;
            if (brood != null && brood.CanOrder && brood.ShowOrderMenu(delegate { base.ProcessInput(ev); }))
            {
                return;
            }
            base.ProcessInput(ev);
        }
    }
}
