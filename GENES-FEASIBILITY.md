# New-gene feasibility study

Drafted 2026-09-30 from the five mockups in `MGEWiki/`. **The two new genes at §4 and §5 are built
(2026-09-30); the castes that carry them are not.** This file resolves every gene the mockups name to
a real def or marks it new, and works the new ones out against the 1.6 assembly.

The plan that uses this study is `CASTES-PLAN.md`.

**The answer in one line.** Only two genes are genuinely new. Both are feasible with patterns this
mod already uses, and neither needs an engine hack. Everything else is a def that exists today and
can be written onto a xenotype as it is.

---

## 1. What is new and what is not

| Caste | Mockup gene | Status |
|---|---|---|
| Hornet | compound eyes | exists - `VRE_CompoundEyes` |
| Hornet | antimicrobial chitin | exists - `VRE_AntimicrobialPeptides` |
| Hornet | rapid life cycle | exists - `VRE_RapidLifeCycle` |
| Hornet | volatile | exists - `VRE_InsectVolatile` |
| Hornet | antennas | exists - `VRE_InsectAntennae` |
| Hornet | poor plants, poor animals, poor intellect, poor social | exist - aptitudes (§3.1) |
| Hornet | sterile | exists - `Sterile` |
| Hornet | great melee, great shooting | exist - aptitudes (§3.1) |
| Hornet | weak flight | exists - `PMM_Gene_FlightWeak` (this workspace, core) |
| Hornet | "don't like honey bees" (flavour) | **new** - the quarrelsome gene (§5), aimed at the bee's gene |
| Mantis | ripper claws | exists - `VRE_RipperBlades` |
| Mantis | compound eyes, infrared sensors | exist - `VRE_CompoundEyes`, `VRE_InfraredSensors` |
| Mantis | vocal chitters, weakened chitin, ecdysone overdrive | exist - `VRE_*` |
| Mantis | antennas | exists |
| Mantis | awful social, great melee | exist - aptitudes |
| Mantis | strong melee damage | exists - `MeleeDamage_Strong` |
| Mantis | hyper aggressive | exists - `Aggression_HyperAggressive` |
| Mothman | infrared sensors, serotonin, vocal glands, passive | exist - `VRE_*` |
| Mothman | heatstress, hypothermic hibernation | exist - `VRE_Heatstress`, `VRE_HypothermicHibernation` |
| Mothman | antennas, kind instinct, sleepy, extra pain | exist |
| Mothman | great social, weak flight | exist |
| Mothman | **disorientating lights** | **new** (§4) |
| Papillon | vocal glands, antimicrobial chitin, high grey matter | exist - `VRE_*` |
| Papillon | sensitive brain | exists - `VRE_SensitiveBrainGoop` |
| Papillon | antennas, kind instinct, sleepy, extra pain, violence disabled, weak flight | exist |
| Papillon | great social | exists - aptitude |
| Soldier beetle | hardened chitin, antimicrobial chitin, hard locked joints | exist - `VRE_*` |
| Soldier beetle | ripper blades / charger claw / megaspider horn | exist - three defs, one choice (§6) |
| Soldier beetle | vocal chitters, ecdysonal overdrive | exist - `VRE_*` |
| Soldier beetle | great melee, awful social, poor crafting, poor construction, poor intellect | exist - aptitudes |
| Soldier beetle | **-50 opinion and more fights with her own kind** | **new** (§5) |

**Three rows change under the 2026-09-30 decisions** (`CASTES-PLAN.md` §0): `VRE_VocalChitters` is
out on the mantis and the beetle in favour of `VRE_LowOctopamine` (§3.2), the beetle's kill thirst
stays in, and her gene list becomes a **rebuild** rather than an addition (§6).

---

## 2. Where this load order's genes come from, and the two traps in reading them

| Prefix | Source | Note for the next reader |
|---|---|---|
| `VRE_*` | Vanilla Races Expanded - Insector, workshop `3260509684` | These are **not** written as `<GeneDef>`. The node is `<VanillaRacesExpandedInsector.GenelineGeneDef>`, so a grep for `<GeneDef>` finds 12 of them and misses ~60. Read the mod's own files. |
| `Aptitude*` | Biotech | Four `GeneTemplateDef` templates (`AptitudeStrong` = "strong {0}", `AptitudePoor` = "poor {0}", `AptitudeRemarkable` = "great {0}", `AptitudeTerrible` = "awful {0}"). One gene per skill is generated from each, so `AptitudeRemarkable_Melee` is a real defName that no file spells out. `archive/ARACHNE-PLAN.md` already records this. |
| plain (`Sterile`, `Sleepy`, ...) | Core / Biotech | Ordinary genes. |
| `PMM_*` | this workspace | Core's `PMM_Gene_FlightWeak`, `PMM_Gene_LargeFrame`, and this mod's own. |

---

## 3. Every mockup name, resolved

### 3.1 Aptitudes

The mockups use tiers, not def names. They map like this - and this is already the mod's own
vocabulary (`archive/ARACHNE-PLAN.md`, 2026-09-26: "good" = `AptitudeStrong_<Skill>`, "great" =
`AptitudeRemarkable_<Skill>`):

| Mockup wording | defName pattern | Example |
|---|---|---|
| "great X" | `AptitudeRemarkable_<Skill>` | `AptitudeRemarkable_Melee` |
| "strong X" | `AptitudeStrong_<Skill>` | `AptitudeStrong_Plants` (the honey bee carries this one today) |
| "poor X" | `AptitudePoor_<Skill>` | `AptitudePoor_Social` |
| "awful X" | `AptitudeTerrible_<Skill>` | `AptitudeTerrible_Social` |

### 3.2 The VRE Insector genes, with what each one actually does

Read out of the defs themselves, not from their names. The **family** column is VRE's own split:
an *evolution* is a boon, a *mutation* (their file names call it a degrade) is a cost. The honey
bee and the greenworm both keep a deliberate balance of the two, and the greenworm's comment says
breaking it was on purpose - so this column is a decision input, not a rule.

| Mockup wording | defName | Family | Verified effect |
|---|---|---|---|
| compound eyes | `VRE_CompoundEyes` | evolution | `capMods` Sight +0.5 |
| antimicrobial chitin | `VRE_AntimicrobialPeptides` | evolution | `InjuryHealingFactor` x2, `ImmunityGainSpeed` x1.5 |
| rapid life cycle | `VRE_RapidLifeCycle` | mutation | `LifespanFactor` 0.325, plus an age curve |
| volatile | `VRE_InsectVolatile` | mutation | `socialFightChanceFactor` 2, aggro mental break chance up |
| antennas | `VRE_InsectAntennae` | cosmetic | `biostatCpx` 1; head cosmetic. Excludes the `Antennas`, `Antenna` and `AG_Antennae` tags |
| ripper claws | `VRE_RipperBlades` | evolution | `WorkSpeedGlobal` -0.3, grants the ripper tools |
| infrared sensors | `VRE_InfraredSensors` | evolution | `ShootingAccuracyFactor_Medium` x1.1, plus heat vision |
| ~~vocal chitters~~ **out** | `VRE_VocalChitters` | mutation | `capMods` Talking **set to 0** - she cannot speak. Dropped 2026-09-30 on the mantis and the beetle |
| **low octopamine** (the swap) | `VRE_LowOctopamine` | mutation | `lovinMTBFactor` 2 and `Fertility` 0.5. **The fertility half is dead on a mamono**: core's `MamonoFertilityPatch` (`Project Mamono/Source/ProjectMamono/MamonoFertilityPatch.cs`) floors an adult mamono's Fertility at 1.0 whatever her genes say, so only the doubled lovin' cooldown is felt |
| vocal glands | `VRE_VocalGlands` | evolution | `capMods` Talking +0.25, `SocialImpact` +0.25 |
| weakened chitin | `VRE_WeakenedChitin` | mutation | `capMods` Moving +0.15, `IncomingDamageFactor` x1.5 |
| ecdysone overdrive | `VRE_EcdysoneOverdrive` | mutation | `causesNeed` KillThirst, and excludes the `KillThirst` tag |
| serotonin | `VRE_Serotonin` | evolution | `socialFightChanceFactor` 2, plus a VGE extension for mood |
| passive | `VRE_PassiveInsect` | mutation | `disabledWorkTags` Violent and Hunting |
| heatstress | `VRE_Heatstress` | mutation | `damageFactors` Flame x4, `mentalBreakDef` FireTerror |
| hypothermic hibernation | `VRE_HypothermicHibernation` | mutation | immune to Hypothermia, `ComfyTemperatureMin` +5, plus a hediff |
| high grey matter | `VRE_HighGreyMatter` | evolution | `GlobalLearningFactor` x1.5; excludes the `InsectorBrain` tag |
| sensitive brain | `VRE_SensitiveBrainGoop` | evolution | `PsychicSensitivity` +0.3, `MeditationFocusGain` +0.2, psychic entropy recovery |
| hardened chitin | `VRE_HardenedChitin` | evolution | `capMods` Moving -0.15, `IncomingDamageFactor` x0.75 |
| hard locked joints | `VRE_HardLockedJoints` | mutation | `MoveSpeed` -0.4 |
| charger claw | `VRE_ChargerClaws` | evolution | `WorkSpeedGlobal` -0.4, grants the charger tools |
| megaspider horn | `VRE_MegaspiderHorns` | evolution | `MiningSpeed` +0.1; excludes the `Headbone` tag |
| microsized | `VRE_Microsized` | mutation | `VEF_BodySize_Offset` -0.45 |

Two of these carry a gameplay consequence that a name does not show, and they are the ones to look
at hardest before writing them down:

- **`VRE_VocalChitters` silences her.** The caption says "vocal chitters", the def sets the Talking
  capacity to 0. A silent killer is a fine idea for the mantis, but it also means no speech bubbles
  and no social interaction from her side.
- **`VRE_EcdysoneOverdrive` gives her kill thirst.** A need that has to be fed with kills. It is
  already shipped on the girtablilu, so it is proven in this mod; on the mantis it is exactly the
  "ruthless killer" note, and on the beetle it is a bigger change than the name suggests.

### 3.3 The vanilla genes

| Mockup wording | defName | Verified effect |
|---|---|---|
| sterile | `Sterile` | `sterilize` true, Fertility 0, `biostatMet` +1 |
| kind instinct | `KindInstinct` | forces the Kind trait, `biostatMet` -1 |
| sleepy | `Sleepy` | `RestFallRateFactor` 1.4, `biostatMet` +2 |
| extra pain | `Pain_Extra` | forces the Wimp trait, `biostatMet` +2 |
| violence disabled | `ViolenceDisabled` | `disabledWorkTags` Violent, `biostatMet` +3; excludes the `MeleeDamage` tag |
| hyper aggressive | `Aggression_HyperAggressive` | `socialFightChanceFactor` 3, aggro mental break selection chance 999, prison break MTBF x0.4 |
| strong melee damage | `MeleeDamage_Strong` | `MeleeDamageFactor` x1.5, `biostatMet` -2 |

### 3.4 Nothing is missing

Every name in the five mockups resolves to a def. There is no gap to invent around.

---

## 4. New gene 1 - the mothman's "disorientating lights" (built 2026-09-30)

**The design, as written in the mockup.** In an artificially lit area of 50%, she takes -5%
consciousness and +10 mood.

**Verdict: feasible, no engine hack, three small defs and one class.** The two halves need different
homes, and that is the only interesting part of this gene. A hediff cannot carry the mood half -
`HediffStage` has `overrideMoodBase` (`Verse/HediffStage.cs:74`) and that *replaces* her mood base
rather than adding to it, so it is the wrong tool. Mood comes from a thought, which is what vanilla
and VRE both use.

| Half | Home | Why |
|---|---|---|
| -5% consciousness | a hidden hediff with a `capMods` entry | `HediffStage.capMods` (`Verse/HediffStage.cs:86`) is the standard capacity modifier, and this mod already ships invisible marker hediffs (`Hediff_Shedding` in Reptiles, `Hediff_SlimeJellyOozing` in Slime, `PMM_RaceTracker_*` here) |
| +10 mood | a `ThoughtDef` with a custom `ThoughtWorker` | `ThoughtStage.baseMoodEffect` (`RimWorld/ThoughtStage.cs:20`); the worker hook is `ThoughtWorker.CurrentStateInternal(Pawn)` (`RimWorld/ThoughtWorker.cs:48`), and `ThoughtDef.workerClass` is instantiated by reflection (`RimWorld/ThoughtDef.cs:179`) |
| the light test | one shared helper | `Verse/GlowGrid.cs:244` |

**The light test is the one piece of engine knowledge this gene needs, and vanilla hands it over -
including the growing-lamp exclusion you asked for.** `GlowGrid` already splits the kinds of light,
and it splits them by *value*, which is what makes "exactly 50%" the right way to write the rule
(`Verse/GlowGrid.cs`):

| Light at her cell | Value | Comes from |
|---|---|---|
| daylight | 1.0 | the sky term, added when the cell is not roofed and `ignoreSky` is false |
| a light source's bright core | 1.0 | `overlightRadius` |
| a light source at full strength | **0.5** | `MaxGameGlowFromNonOverlitGroundLights = 0.5f` (line 195) |
| dark | 0.0 | nothing lights the cell |

The split is an `overlightRadius` on the light source. `GlowGrid` stamps the alpha byte with
`AlphaOfOverlit = 1` (line 187) on every cell inside it, and `GroundGlowAt` returns a flat **1.0**
the moment it sees that byte (line 250, `if (accumulatedGlowAt.a == 1) return 1f;`). Every other
light source is capped at 0.5 (line 195).

The two sources in question are therefore already different values, with no def names in our code:

- `StandingLamp` - `glowRadius 12`, `glowColor (214,148,94,0)`, no overlight radius -> **0.5**;
- `SunLamp`, the growing lamp - `glowRadius 14`, `glowColor (370,370,370,0)`, **`overlightRadius
  7.0`** -> **1.0** in its bright core, 0.5 out in the ring.

The test is `map.glowGrid.GroundGlowAt(pawn.Position, ignoreSky: true) == 0.5f`, written as the band
`>= 0.5f && < 1f` for readability. Lamp, torch, brazier and campfire light all pass it; the growing
lamp's core does not. One grid lookup per check, no patch, no polling of buildings.

Note the argument that is *not* passed: `ignoreCavePlants` stays at its default of `false`, so a
glowing cave counts as light. Decided 2026-09-30 - she takes light where she finds it - and it is a
one-word change if that ever feels wrong.

**The growing lamp's outer ring stays in, decided 2026-09-30 on my call.** Tiles 7 to 14 from a
growing lamp read 0.5 like any other light, so a mothman at its edge still gets the effect. Excluding
the ring means scanning the map's light sources on every check instead of reading one cell. The core -
where the crops and anyone working them actually stand - is out, which is the part that matters. If it
ever turns into a mood-farming trick, that scan is the fix.

**Cost.** One gene def, one hidden hediff def, one thought def, one small class file (the gene class
that adds and removes the hediff, and the worker that answers the same test - one file, like
`InsectGrief.cs`). The hediff toggle belongs on a slow poll, the honey gene's
`IsHashIntervalTick(2500)` cadence, not on every tick.

**Risks, and what I need you to decide.**

1. **Settled 2026-09-30: exactly 50%, the value a man-made light gives, with the growing lamps out.**
   The engine does the exclusion for us, by light value (see above).
2. **Glowing cave plants count, decided 2026-09-30.** She takes light where she finds it, so the test
   does not filter them out.
3. **A campfire is a light source.** So is a brazier and a burning wall. She is happy next to a fire,
   and the 50% rule keeps that.
4. **Outdoors at night is not artificial.** At night, an unroofed cell has no sky glow and no
   glower, so she gets nothing - correct by the design, worth knowing.
5. **Two mechanics, one condition.** The hediff and the thought run the same test in two places, so
   they can disagree for one tick at the boundary. Harmless, and cheaper than one shared state
   object. Say the word if you want one object holding both.

**Rejected alternatives.** `HediffStage.overrideMoodBase` (replaces the mood base, would flatten
other thoughts); a mood stat offset (no such stat exists - mood is thoughts); a Harmony patch on
`Pawn` (nothing to patch for a condition this simple); `DarklightUtility`/psych-glow instead of raw
glow (that is about darkness for the Anomaly shamblers, not about lamps).

---

## 5. New gene 2 - the quarrelsome gene (built 2026-09-30)

**The design, as written in the soldier beetle mockup.** A gene that gives -50 opinion toward others
carrying the same gene, and makes her more likely to start fights with them.

**The hornet mockup asks for the same thing in reverse** ("don't like honey bees"), which is the
same machinery aimed at a *different* gene. That is why this is written as one gene with a dial, not
two genes.

**Verdict: feasible, and the second half costs nothing at all - it falls out of the first.**
This is the useful finding of this study.

*Half one: the opinion.* A pairwise opinion modifier is a first-class vanilla thing. A `ThoughtDef`
whose thought class is `Thought_SituationalSocial` (`RimWorld/Thought_SituationalSocial.cs:5`) is
collected per other pawn by `ThoughtHandler.GetSocialThoughts` (`RimWorld/ThoughtHandler.cs:133`),
and the worker hook for "is this thought active about *that* pawn" is
`ThoughtWorker.CurrentSocialStateInternal(Pawn p, Pawn otherPawn)` (`RimWorld/ThoughtWorker.cs:53`).
The opinion value itself is `ThoughtStage.baseOpinionOffset` (`RimWorld/ThoughtStage.cs:22`).

Inside that worker, the check is `otherPawn.genes.HasActiveGene(geneDef)` -
`Pawn_GeneTracker.HasActiveGene` (`RimWorld/Pawn_GeneTracker.cs:525`). Which gene is named is the
dial: her own gene for the soldier beetle (mutual dislike), or the honey gene for the hornet (one
way, and the bee is unbothered).

*Half two: the fights.* No code is needed, because opinion already drives the fight roll.
`Pawn_InteractionsTracker` computes the chance and then does this
(`RimWorld/Pawn_InteractionsTracker.cs:451-452`):

```csharp
float num = pawn.relations.OpinionOf(initiator);
socialFightBaseChance = num < 0f
    ? socialFightBaseChance * GenMath.LerpDouble(-100f, 0f, 4f, 1f, num)
    : socialFightBaseChance * GenMath.LerpDouble(0f, 100f, 1f, 0.6f, num);
```

At an opinion of -50 that multiplier is **x2.5**, and the ceiling at -100 is x4. So a -50 opinion
against exactly the pawns she dislikes *is* "much more likely to start a fight with them", applied
by the engine, pairwise, with no patch from us.

**Cost.** One gene def (a plain marker, with a mod extension naming the disliked gene), one thought
def, one small worker class. No Harmony patch, no stat, no new state on the pawn.

**Decision and consequences.**

**Decided 2026-09-30.** One way for the hornet - her thought names `PMM_Gene_Honey`, so honey bees
never notice her grudge - and mutual for the beetle, whose thought names her own gene. **-50 is the
number.** Three consequences worth keeping in view, none of them open questions:

1. **It multiplies with the fight-chance factor these castes may already carry.** The mantis takes
   `Aggression_HyperAggressive` (`socialFightChanceFactor` 3) and the hornet's `VRE_InsectVolatile`
   is already 2. The beetle should probably not also take a fight-chance gene.
2. **It fires on sight, not on interaction.** Opinion is continuous, so the penalty applies the
   moment both are on the map. No tale, no insult, no event. That matches "very angry" but it is not
   how vanilla writes a social thought, and it is worth deciding on purpose.
3. **Language keys.** Two new lines per thought (`label` and `labelSocial`), in
   `Languages/English/Keyed/`.

---

## 6. The mockup's question: "how do you do a package of one of three genes?"

The soldier beetle mockup asks how to give a caste one of ripper blades, charger claw or megaspider
horn. All three exist (`VRE_RipperBlades`, `VRE_ChargerClaws`, `VRE_MegaspiderHorns`).

**The pattern is already in this mod, and it was built for exactly this shape of question.** The
body colour roll (`Source/Insects/InsectColours.cs`, `HANDOFF.md` §5.13): the shared insect gene
carries a class that picks one of her caste's three when the gene lands, and a
`PawnGenerator.GeneratePawn` postfix re-rolls at the end of generation so a daughter gets a fresh
pick rather than her mother's. A weapon gene works the same way: one class, one list of three, one
added to her. The colour roll also shows the cheapest place to keep the list - on the def that owns
it, as a `DefModExtension`, so the set stays data and the code never names a colour.

**Three things to decide, because the mockup's version is not free.**

1. **Settled 2026-09-30, and confirmed: her race tool list is emptied and the gene owns the weapon.**
   The girtablilu is the precedent - that race def carries **zero** melee tools and
   `VRE_ChargerClaws` is its weapon. Expect a balance change: the beetle's damage then follows VRE's
   tuning rather than the megaspider's exact numbers, so `archive/PLAN.md` §3's "exact DPS parity" stops
   describing her.
2. **The three genes exclude things.** `VRE_MegaspiderHorns` excludes the `Headbone` cosmetic tag,
   and the others have their own lists. Adding exactly one is fine; adding two is not. The roll must
   therefore be the only writer of them.
3. **A roll is invisible until it lands.** Same as the body colours today: the xenotype lists the
   roller, not the result. The player sees which one she got on her gene page, and a save keeps it.

**Cost.** One gene def, one class, one `PawnGenerator` postfix (which the mod already has for
body colours, so the postfix is a second entry in one existing patch).

---

## 7. Rules the build has to respect

1. **Genelines cannot be applied to NPC pawns** (`HANDOFF.md` §6.1). This is why the existing
   castes work at all: they list *individual* genes on the xenotype, and never the geneline
   system. Keep doing exactly that.
2. **`unlockable` and `selectionWeight 0` do not block us.** Many VRE genes are picker-only. The
   honey bee already carries four of them, and they behave normally on a pawn.
3. **Metabolic cost adds up.** Every `biostatMet` on the list is food she eats. A quick sum, so the
   numbers are visible while the lists are being agreed: sterile +1, sleepy +2, extra pain +2,
   violence disabled +3, kind instinct -1, strong melee damage -2, jelly dependency +3, and the
   VRE evolutions/mutations each carry their own. This is the number to look at when a caste comes
   out eating twice what her sisters do.
4. **Exclusion tags are real.** `ViolenceDisabled` cannot sit with a melee damage gene;
   `VRE_HighGreyMatter` cannot sit with `VRE_LowGreyMatter` or `VRE_SolidGreyMatter`;
   `VRE_InsectAntennae` blocks other head cosmetics; `VRE_MegaspiderHorns` blocks `Headbone`. A
   caste that fails this fails at load with a red error, not silently.
5. **Antennae are the family norm already.** `VRE_InsectAntennae` is on the honey bee and the other
   newer castes. The soldier beetle's current list does **not** carry it - worth checking against
   her art before the mockup's "antennas" line is acted on.
6. **Every new gene needs an icon,** and the icon goes on the gene page next to the effect lines.
   The family's convention is one PNG per gene in `Textures/UI/Icons/Genes/`, hand-drawn.

---

## 8. What is settled, and what is left

Every gene question from the first review is answered and written into the sections above: the light
rule (exactly 50%, the growing lamp's bright core out, a glowing cave counting as light), the quarrel
(one way for the hornet, mutual for the beetle, -50), the vocal-chitters swap, the kill thirst, the
beetle's rebuild with her race tools emptied, and placeholder art first.

**Nothing is left.** The queen lays all four (decided 2026-09-30, `CASTES-PLAN.md` §0), so the plan
has no open questions and implementation can start at its phase 1.
