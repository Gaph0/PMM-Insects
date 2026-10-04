using RimWorld;
using Verse;

namespace PMM_Insects
{
    /// <summary>
    /// The light test for the light gene, shared by both halves of it (see
    /// Defs/GeneDefs/Genes_DisorientatingLights.xml for the rule itself).
    ///
    /// `GlowGrid` answers in three values, and it does the growing-lamp exclusion by itself:
    /// ordinary light sources are capped at 0.5 (MaxGameGlowFromNonOverlitGroundLights) and every
    /// cell inside a source's `overlightRadius` reads a flat 1.0 (AlphaOfOverlit). So a lamp, a
    /// torch, a brazier, a campfire or a glowing cave reads 0.5 and passes, while the bright core of
    /// a vanilla `SunLamp` (overlightRadius 7.0) reads 1.0 and does not. No def names, no scan of
    /// nearby buildings, one grid lookup.
    ///
    /// `ignoreSky: true` is what makes it *artificial*: the sky term is skipped, so daylight never
    /// counts however bright the day. `ignoreCavePlants` is deliberately left at its default, so a
    /// glowing cave does count - decided 2026-09-30.
    /// </summary>
    public static class DisorientatingLights
    {
        /// <summary>The value a man-made light source at full strength gives.</summary>
        public const float LitGlow = 0.5f;

        public static bool InLight(Pawn pawn)
        {
            Map map = pawn?.Map;
            if (map == null || !pawn.Spawned)
            {
                return false;
            }
            float glow = map.glowGrid.GroundGlowAt(pawn.Position, ignoreSky: true);
            return glow >= LitGlow && glow < 1f;
        }
    }

    /// <summary>
    /// The consciousness half of the light gene. The mood half cannot live here - `HediffStage` has
    /// only `overrideMoodBase`, which replaces the mood base instead of adding to it - so this
    /// hediff carries the -5% and `PMM_Thought_DisorientatingLights` carries the +10.
    ///
    /// Invisible on the Health tab: state rather than a condition, the same call the family's other
    /// marker hediffs make (Reptiles' `Hediff_Shedding`, the slime mod's `Hediff_SlimeJellyOozing`).
    /// </summary>
    public class Hediff_DisorientatingLights : HediffWithComps
    {
        public override bool Visible => false;
    }

    /// <summary>
    /// Adds and removes the hediff above as the light changes, on the honey gene's cadence - one
    /// in-game hour. Nothing has to happen at the instant she steps under a lamp, and the light only
    /// matters while she stands in it.
    ///
    /// `PostAdd` covers a gene arriving in play and `PostRemove` an implant being taken out. A save
    /// loaded with the gene but no hediff settles on the first poll, within the hour, because
    /// `Pawn_GeneTracker.ExposeData` does not call `PostAdd`.
    /// </summary>
    public class Gene_DisorientatingLights : Gene
    {
        private const int PollIntervalTicks = 2500;

        /// <summary>
        /// This gene on the pawn, or null. Only an ACTIVE gene answers
        /// (`Pawn_GeneTracker.GetFirstGeneOfType` skips a silenced one), which is the answer both
        /// halves of the gene want: the hediff below adds itself through it, and the thought worker
        /// refuses every pawn without it.
        /// </summary>
        public static Gene_DisorientatingLights Get(Pawn pawn)
        {
            return pawn?.genes?.GetFirstGeneOfType<Gene_DisorientatingLights>();
        }

        public override void PostAdd()
        {
            base.PostAdd();
            Sync();
        }

        public override void PostRemove()
        {
            base.PostRemove();
            RemoveHediff();
        }

        public override void Tick()
        {
            base.Tick();
            if (pawn == null || pawn.Dead)
            {
                return;
            }
            if (pawn.IsHashIntervalTick(PollIntervalTicks))
            {
                Sync();
            }
        }

        private void Sync()
        {
            if (pawn?.health == null)
            {
                return;
            }
            if (Active && DisorientatingLights.InLight(pawn))
            {
                if (pawn.health.hediffSet.GetFirstHediffOfDef(InsectDefOf.PMM_Hediff_DisorientatingLights) == null)
                {
                    pawn.health.AddHediff(InsectDefOf.PMM_Hediff_DisorientatingLights);
                }
            }
            else
            {
                RemoveHediff();
            }
        }

        private void RemoveHediff()
        {
            Hediff hediff = pawn?.health?.hediffSet?.GetFirstHediffOfDef(InsectDefOf.PMM_Hediff_DisorientatingLights);
            if (hediff != null)
            {
                pawn.health.RemoveHediff(hediff);
            }
        }
    }

    /// <summary>
    /// The mood half. A `ThoughtDef` with a worker and no duration becomes `Thought_Situational` by
    /// default (`RimWorld/ThoughtDef.cs`, `ThoughtClass`), which is exactly what a plain mood
    /// thought is. The same test as the hediff, so the two agree except for one tick at the light's
    /// edge.
    ///
    /// The gene check is load bearing, and until 2026-10-04 this worker did not have it. A
    /// non-social situational thought is not "asked about" by anything: the pawn's
    /// `SituationalThoughtHandler` walks the WHOLE database
    /// (`ThoughtUtility.situationalNonSocialThoughtDefs`) and instantiates every one of those defs
    /// for every pawn, then keeps the ones whose worker answers Active. So a worker that answers a
    /// condition alone answers it for the whole map - every pawn standing in a lamp's light, mamono
    /// or not, read +10 "giddy in the light" while only a carrier took the -5%. A worker here names
    /// its own source first.
    /// </summary>
    public class ThoughtWorker_DisorientatingLights : ThoughtWorker
    {
        protected override ThoughtState CurrentStateInternal(Pawn p)
        {
            if (Gene_DisorientatingLights.Get(p) == null)
            {
                return ThoughtState.Inactive;
            }
            return DisorientatingLights.InLight(p) ? ThoughtState.ActiveAtStage(0) : ThoughtState.Inactive;
        }
    }
}
