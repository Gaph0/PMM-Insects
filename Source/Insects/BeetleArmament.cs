using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;

namespace PMM_Insects
{
    /// <summary>
    /// The soldier beetle's armament, and the rule that she grows one of three.
    ///
    /// The mockup asks for "one of ripper blades, charger claw or megaspider horn", so there is
    /// nothing to invent: all three genes exist in VRE Insector. A gene cannot roll for itself, so
    /// this is the shape the mod already uses for the castes' colours (`InsectColours.cs`,
    /// `HANDOFF.md` §5.13) - one def carries the list, one class picks from it when the gene lands,
    /// and one `PawnGenerator` postfix re-rolls at the end of generation so a daughter gets her own
    /// pick rather than her mother's.
    ///
    /// Three engine facts this rests on, checked against the 1.6 assembly and VRE's own files:
    ///   * `Pawn_GeneTracker.AddGene` ends in `gene.PostAdd()`, so the roll runs however she came
    ///     by the gene - a xenotype, a genepack, or a transformation into her caste.
    ///   * `Pawn.Tools` is `def.tools` and nothing else, and VRE's weapon genes do not add to it:
    ///     each grants a hediff carrying a `HediffCompProperties_VerbGiver`, and the verbs from
    ///     that hediff are what she attacks with (`Hediffs_Attacks.xml`, `VRE_RipperBlades` line
    ///     57: Cut, power 18, armour penetration 0.27). That is why her race's own tool list can
    ///     be emptied without disarming her, which is what `CASTES-PLAN.md` §0 decision 11 asks
    ///     for. It also means a beetle who loses the body part the hediff hangs on loses the
    ///     weapon with it - no fists are left underneath by design.
    ///   * The three genes exclude each other's ground (`VRE_MegaspiderHorns` excludes the
    ///     `Headbone` cosmetic tag, the other two carry their own lists), so this roll has to be
    ///     their only writer. It removes whichever one she already has before it adds the new one.
    ///   * The pick is added as an ENDOGENE, which is how her xenotype's own genes arrive:
    ///     `Pawn_GeneTracker.SetXenotype` adds an inheritable xenotype's genes with
    ///     `AddGene(gene, xenogene: false)`, and every caste here is `inheritable: true`. A
    ///     xenogene applied its effects and then sat in a list the gene page does not show beside
    ///     the rest of her genes (fixed 2026-10-01).
    ///
    /// A roll is invisible until it lands: her xenotype lists this gene, not the result, and the
    /// player reads which armament she got on her gene page. A save keeps it - nothing re-rolls on
    /// load, because `PostAdd` fires when a gene is added and not when it is loaded.
    /// </summary>
    internal static class BeetleArmament
    {
        /// <summary>
        /// The def that carries the list, looked up by name rather than through a `[DefOf]` field:
        /// the def ships with this mod, but a `[DefOf]` field pointing at a def that is not loaded
        /// is a load error, so the lookup stays defensive.
        /// </summary>
        private static GeneDef ArmamentGene =>
            DefDatabase<GeneDef>.GetNamedSilentFail("PMM_Gene_BeetleArmament");

        /// <summary>The three she rolls between, off the gene def, or null when it names none.</summary>
        public static List<GeneDef> Options(Pawn pawn)
        {
            return ArmamentGene?.GetModExtension<BeetleArmamentRoll>()?.armaments;
        }

        /// <summary>
        /// Gives her a freshly rolled armament, replacing whichever of the three she already has.
        /// A woman without the roll gene - any caste but the beetle - is left alone.
        /// </summary>
        public static void Roll(Pawn pawn)
        {
            if (pawn?.genes == null)
            {
                return;
            }

            GeneDef geneDef = ArmamentGene;
            if (geneDef == null || !pawn.genes.HasActiveGene(geneDef))
            {
                return;
            }

            List<GeneDef> options = Options(pawn);
            if (options == null || options.Count == 0)
            {
                return;
            }

            foreach (GeneDef option in options)
            {
                if (option == null)
                {
                    continue;
                }
                Gene existing = pawn.genes.GetGene(option);
                if (existing != null)
                {
                    pawn.genes.RemoveGene(existing);
                }
            }

            // Endogene, not xenogene (fixed 2026-10-01). `Pawn_GeneTracker.SetXenotype` adds a
            // xenotype's genes with `AddGene(xenotype.genes[i], !xenotype.inheritable)`, and every
            // caste here is `inheritable: true` - so her sisters' genes, this roll gene included,
            // are all endogenes and read under the gene page's "Germline genes". A gene added as a
            // xenogene lands in the Xenogenes list instead: it worked, its hediff and its graphics
            // were applied, and it was nowhere on the page the player was reading.
            pawn.genes.AddGene(options.RandomElement(), xenogene: false);
        }
    }

    /// <summary>
    /// The armament gene's own class. It does nothing but roll on arrival, which is also the moment
    /// a woman transformed into a soldier beetle picks her weapon.
    /// </summary>
    public class Gene_BeetleArmament : Gene
    {
        public override void PostAdd()
        {
            base.PostAdd();
            BeetleArmament.Roll(pawn);
        }
    }

    /// <summary>
    /// A generated beetle - a wild one, a raider, or a daughter born into a village - rolls her
    /// armament here, last word on the matter. A daughter needs it: her mother's genes arrive with
    /// her, weapon included, and this roll is what makes the weapon her own.
    /// </summary>
    [HarmonyPatch(typeof(PawnGenerator), nameof(PawnGenerator.GeneratePawn), new[] { typeof(PawnGenerationRequest) })]
    public static class BeetleArmamentSpawnPatch
    {
        public static void Postfix(Pawn __result)
        {
            BeetleArmament.Roll(__result);
        }
    }

    /// <summary>
    /// The three genes she rolls between, declared on the gene def that owns the roll.
    ///
    /// It lives there rather than in code for the same reason a caste's palette does: the set stays
    /// data, so a fourth armament - if VRE or another mod ever adds one - is one line of XML and no
    /// code change.
    /// </summary>
    public class BeetleArmamentRoll : DefModExtension
    {
        public List<GeneDef> armaments;
    }
}
