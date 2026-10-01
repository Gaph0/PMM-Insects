using System.Collections.Generic;
using RimWorld;
using Verse;

namespace PMM_Insects
{
    /// <summary>
    /// An arachne's caffeine is alcohol (user's calls 2026-09-27: "just arachne", "all beverages with
    /// caffeine; tea, coffee, iced coffee, every beverage from the coffees and tea VBE mod", "as drunk as
    /// the caffeine severity grants", and an addiction she already has is *not* forgiven; widened
    /// 2026-10-01 to both arachnes, the weaver and her ant mimic, on a review that found the mimic left
    /// out of a reaction meant for her kind).
    ///
    /// `Patches/ArachneCoffee_VBE.xml` appends this doer to the end of a caffeinated drink's
    /// `ingestible.outcomeDoers`. Ingestible doers run in list order, so by the time this one fires the
    /// drink has already applied its own caffeine. That ordering is the whole trick - it means this doer
    /// can read what was applied and undo it, and it needs no list of drink names to be maintained:
    ///   - it finds the drink's own doers whose `toleranceChemical` is the configured caffeine chemical
    ///     and mirrors the largest of their severities onto the drunk hediff, so coffee, iced coffee,
    ///     energy drinks and teas each hand her as much intoxication as they would have handed caffeine;
    ///   - it then removes the hediffs those doers applied, which is what makes it *instead of* rather
    ///     than *as well as*. Removing `VBE_CaffeineTolerance` is also what stops a caffeine addiction
    ///     from ever starting, because vanilla cannot addict a pawn before tolerance crosses
    ///     `minToleranceToAddict`. An addiction already on her is deliberately left alone, which does
    ///     mean it will slide into withdrawal now that coffee no longer feeds it.
    ///
    /// A drink whose caffeine comes from a chemical rather than a doer has nothing to mirror, so the
    /// severity falls back to `severityFallback` - still drunk, just not measured off the drink.
    /// `severityFactor` is the single tuning dial and the patch sets it to 0.15: one coffee is one
    /// beer. A straight mirror was tried first and blacked an arachne out on a single black coffee -
    /// every caffeine high in this load order is severity 1, and one point of `AlcoholHigh` is past the
    /// stage that caps Consciousness at 0.1.
    ///
    /// Every defName lives in the patch XML, none in here, so nothing dangles in a load order without
    /// VBE - and the class is only ever reached through that patch.
    /// </summary>
    public class IngestionOutcomeDoer_CoffeeDrunk : IngestionOutcomeDoer
    {
        public ThingDef race;
        public ChemicalDef caffeine;
        public HediffDef drunkHediff;
        public float severityFactor = 0.15f;
        public float severityFallback = 0.15f;

        protected override void DoIngestionOutcomeSpecial(Pawn pawn, Thing ingested, int ingestedCount)
        {
            List<IngestionOutcomeDoer> doers = ingested?.def?.ingestible?.outcomeDoers;
            if (pawn?.health == null || doers == null || drunkHediff == null)
            {
                return;
            }
            if (race != null && pawn.def != race)
            {
                return;
            }

            float severity = 0f;
            List<HediffDef> applied = null;
            foreach (IngestionOutcomeDoer doer in doers)
            {
                if (!(doer is IngestionOutcomeDoer_GiveHediff give) || give.toleranceChemical != caffeine)
                {
                    continue;
                }
                // Same reading as IngestionOutcomeDoer_GiveHediff itself uses: a severity of -1 in the
                // XML means "use the hediff's own initial severity".
                float mirror = give.severity > 0f ? give.severity : (give.hediffDef?.initialSeverity ?? 0f);
                if (mirror > severity)
                {
                    severity = mirror;
                }
                if (give.hediffDef != null)
                {
                    applied = applied ?? new List<HediffDef>();
                    applied.Add(give.hediffDef);
                }
            }

            HealthUtility.AdjustSeverity(pawn, drunkHediff,
                (severity > 0f ? severity : severityFallback) * severityFactor);

            if (applied != null)
            {
                foreach (HediffDef def in applied)
                {
                    Hediff hediff = pawn.health.hediffSet.GetFirstHediffOfDef(def);
                    if (hediff != null)
                    {
                        pawn.health.RemoveHediff(hediff);
                    }
                }
            }
        }
    }
}
