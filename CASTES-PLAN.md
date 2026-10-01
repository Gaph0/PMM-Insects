# Five new castes - plan for review

Drafted 2026-09-30 from the five draw.io mockups in `MGEWiki/`
(`hornetxenotypemockup.xml`, `mantismockup.xml`, `mothmanmockup.xml`, `papillonmockup.xml`,
`soldierbeetlemockup.xml`).

**Phase 1 is built** - the two genes, defs, classes and placeholder icons, 2026-09-30. The four
castes are not, and nothing in the mod carries either gene yet. The gene work is costed in
`GENES-FEASIBILITY.md`; read the two together.

**Decisions taken 2026-09-30** are recorded in §0, and everything below is written for them.

## 0. Decisions (user, 2026-09-30)

| # | Question | Decision |
|---|---|---|
| 1 | The light gene's condition | **exactly 50%** - the level a man-made light gives - and **not** the growing lamps |
| 2 | The quarrel | **one way** for the hornet (she hates bees, they do not care), **mutual** for the beetle, **-50** |
| 3 | The mantis's voice, the beetle's hunger | `VRE_VocalChitters` **out**, `VRE_LowOctopamine` **in**; the kill thirst **stays** |
| 4 | The two passive castes | spawn naturally at a **low rate**, and be **much more likely to be in a trade caravan** |
| 5 | Sizes | **body size 1** for all four |
| 6 | The soldier beetle | **rebuild** her gene list to the mockup |
| 7 | The growing lamp's outer ring | **my call: leave it.** Its bright core is out; the ring reads 0.5 like any other lamp (§3.3) |
| 8 | Glowing cave plants | **artificial** - a glowing cave is light |
| 9 | The caravan slot | **their own trader kinds** - the mothman and the papillon *lead* caravans |
| 10 | Health scale and `combatPower` | **confirmed as proposed** (0.8 / 0.7 / 0.8 / 0.5) |
| 11 | The beetle's race tools | **emptied** - the gene owns her weapon |
| 12 | Art | **hand art**, dropped in `Project Mamono Textures/` and copied into the mod |
| 13 | The queen's brood list | **all four** - the abaddon can lay any of the new castes |

What each one changes, in a line each:

- **1** lands the light test on a value the engine separates by itself: 0.5 under a lamp, 1.0 under a
  growing lamp's bright core (§3.3, and `GENES-FEASIBILITY.md` §4).
- **2** makes the *disliked* gene the dial: `PMM_Gene_Honey` for the hornet, her own gene for the beetle.
- **3** keeps both castes able to speak, and pays with a doubled lovin' cooldown instead.
- **4** means no Combat or Settlement weight for the mothman and the papillon, a small roll wherever
  the hive spawns people, and a trader pawn kind each (§3.3, §3.4).
- **5** removes the per-caste size differences this plan had proposed.
- **6** is the biggest change: she loses six genes she carries today, and the mockup's list has no
  flight and no large frame (§3.5).
- **7** keeps the light test a single grid lookup, and **8** leaves cave plants out of its filter, so
  a glowing cave is light to her.
- **9** adds two `<trader>true</trader>` kinds beside `PMM_InsectTrader`; neither caste walks in an
  escort.
- **10** keeps the health scales and `combatPower` figures as drafted, and **11** empties
  `PMM_Race_SoldierBeetle`'s tool list.
- **12** means four hand-drawn icons at 64x64, the size the mod's real xenotype icons use, dropped
  in `Project Mamono Textures/` and copied to `Textures/UI/Icons/Xenotypes/<Name>.png`.
- **13** puts all four in the queen's brood list. That list feeds both "choose the caste" and "a
  random mamono", so each new caste also becomes something a queen can hatch by chance.

---

## 1. What the mockups ask for

| Mockup | Role, as you wrote it | Genes, as you wrote them | What is really new |
|---|---|---|---|
| Hornet | "fighters for a bee hive, very angry. Don't like honey bee's (flavour)" | compound eyes, antimicrobial chitin, rapid life cycle, volatile, antennas, poor plants, poor animals, poor intellect, poor social, sterile, great melee, great shooting, weak flight | the dislike of honey bees - the quarrelsome gene |
| Mantis | "Cold, emotionless, ruthless killers, fragile, glass cannons" | ripper claws, compound eyes, infrared sensors, vocal chitters, weakened chitin, ecdysone overdrive, antennas, awful social, great melee, strong melee damage, hyper aggressive | nothing - every gene exists |
| Mothman | "Prefers dark places, loves light, gentle, simple minded" | infrared sensors, serotonin, vocal glands, passive, heatstress, hypothermic hibernation, antennas, kind instinct, sleepy, extra pain, great social, weak flight | the light gene (mood and consciousness) |
| Papillon | "Beautiful, fragile, great mamono to have around" | vocal glands, antimicrobial chitin, high grey matter, sensitive brain, antennas, great social, kind instinct, sleepy, extra pain, violence disabled, weak flight | nothing - every gene exists |
| Soldier beetle | "Tanky, slow, dangerous, not good talkers, violent towards other soldier beetles" | hardened chitin, one of ripper blades / charger claw / megaspider horn, antimicrobial chitin, hard locked joints, vocal chitters, ecdysonal overdrive, antennas, great melee, awful social, poor crafting, poor construction, poor intellect | the quarrelsome gene, and the one-of-three weapon roll |

So the build order is driven by two things only: **two new genes**, and **four castes to wire up**.

---

## 2. Verified facts this plan rests on

**Bodies.** Big & Small's `SimplyRaces` ships ten: Centaur, DefaultHumanlike, FourArms, LamiaBody,
Mech, SixArms, Spider, TailedHuman, WingedHuman, Yukkuri. There is no moth, mantis or hornet body.
But this mod already borrows three of them, so two of the four new castes need no new body work:

| Body | Already used by |
|---|---|
| `BS_HumanoidWithWings_Body` (B&S WingedHuman) | three existing races (lines 379, 612, 1295 of `Races_InsectMamono_BS.xml`) |
| `PMM_Body_FourArmedWinged` (ours, made from B&S `FourArms`) | the abaddon and her folk |
| `BS_SpiderHybrid` | the arachne, the girtablilu, the ant arachne |

**Genes.** Every mockup gene resolves to a def that is already in this load order, and each one's
real effect is written out in `GENES-FEASIBILITY.md` §3. Two genes do not exist yet and are the only
new work: the mothman's light gene (§4 there) and the quarrelsome gene (§5 there).

**The wiring checklist for one caste.** This is what the honey bee's pass touched, and a new caste
touches the same list:

| # | File | What goes in it |
|---|---|---|
| 1 | `Defs/XenotypeDefs/Xenotypes_Insect.xml` | the xenotype: genes, `setRace` + `forceRace` to her own race, `factionlessGenerationWeight 0`, her icon path, **and her three body colours** as a `PMM_Insects.InsectSkinColours` list - three new defs in `Genes_InsectSkin.xml`, one of them rolled for her when she is generated (`HANDOFF.md` §5.13) |
| 2 | `Defs/ThingDefs/Races_InsectMamono_BS.xml` | the race (body, sizes, armour, melee tools, meat/leather) **and** her race tracker hediff (`PMM_RaceTracker_*`) |
| 3 | `Defs/PawnKindDefs/PawnKinds_InsectorTribe.xml` | the pawn kind, `combatPower`, forced traits - and, for the two social castes, a second kind with `<trader>true</trader>` modelled on `PMM_InsectTrader` |
| 4 | `Defs/FactionDefs/Factions_InsectorTribes.xml` | her row in the tribe's `xenotypeSet` (weights must sum to 1) and in each group she belongs to |
| 5 | `Source/Insects/HealthScalePatch.cs` | a factor only if her race base is small. The patch multiplies `Pawn.HealthScale`; races whose base already meets a human's are deliberately absent from its switch (the beetle's 2.5, the ant's 1.7, the abaddon's 9.8) |
| 6 | `Source/Insects/InsectMamonoCorpses.cs` | so her corpse files with the mamonos, not with humans |
| 7 | `Source/Insects/BroodOrder.cs` + the queen's race | **yes, all four castes** (decided 2026-09-30) - and note the same list also feeds her "a random mamono" draw |
| 8 | `Textures/UI/Icons/Xenotypes/<Name>.png` | hand art at 64x64, the size the shipped castes' icons use (decided 2026-10-01) |
| 9 | `CHANGELOG.md`, `HANDOFF.md` | one line each, in this project's formats |

**The rosters today.** The hive fields no flier at all - its Combat group says so in its own comment
("The hive keeps no flier, so there is one Combat group here and not the swarm's two"), and its
`xenotypeSet` is devil bug 0.28, giant ant 0.28, soldier beetle 0.20, ant arachne 0.12, honey bee
0.12. The swarm carries the arachne, the beelzebub, the girtablilu, the queen and the vamp mosquito,
and its Combat groups do include a flier. **A hornet in the hive changes a standing decision**, so it
is question 2 below.

**Two rules that bite.** Genelines cannot be put on NPC pawns (`HANDOFF.md` §6.1) - individual genes
can, which is how every existing caste works. And a `[DefOf]` field whose def is missing is a load
error, so anything new that C# reads by name must ship in this repo.

---

## 3. The five, one at a time

Proposed numbers are marked **proposal**; every one is a comparison with her sisters, which is how
the honey bee was set (she is the one caste with no bug to copy).

### 3.1 Hornet - the hive's fighter

- **Role.** "Fighters for a bee hive, very angry. Don't like honey bee's."
- **Genes.** All thirteen exist (`GENES-FEASIBILITY.md` §1). Two notes: `VRE_RapidLifeCycle` is a
  cost gene (lifespan x0.325), and `VRE_InsectVolatile` already doubles her social-fight chance, so
  she is quarrelsome by numbers before the new gene is added.
- **New gene.** The dislike of honey bees is the quarrelsome gene aimed at `PMM_Gene_Honey`
  (`GENES-FEASIBILITY.md` §5). One-way: the bee is not bothered.
- **Race proposal.** `BS_HumanoidWithWings_Body`, size pair 1.0, health scale 0.4 base x 2.0 -> 0.8
  (frailer than the beetle, tougher than the papillon), light armour, one melee tool - a sting - and
  **core's venom gene** (`ProjectMamono_MamonoVenom`). That is the girtablilu's setup: core's
  `VenomDamagePatch` is keyed to the venom gene, not to a verb, so every melee hit injects venom and
  no custom weapon def is needed. `combatPower` ~110, between the giant ant's 75 and the beetle's 150.
- **Placement.** The hive, and this is the decision: she is its first fighter and its first flier.
  Proposal: her own `Combat` group for the hive (the hive would then have two, like the swarm), plus
  `Settlement`, plus a `Hunters` row, and never `Farmers`.
- **Cost.** The usual eight-step wiring plus the new gene. No new body art beyond wings the body
  already draws.

### 3.2 Mantis - the swarm's glass cannon

- **Role.** "Cold, emotionless, ruthless killers, fragile, glass cannons."
- **Genes.** All eleven exist, with one swap decided 2026-09-30: `VRE_VocalChitters` is **out** (it
  sets her Talking capacity to 0, so she could not speak) and `VRE_LowOctopamine` is **in** - she
  keeps her voice and pays with a doubled lovin' cooldown. The kill thirst **stays**
  (`VRE_EcdysoneOverdrive`), which the girtablilu already carries, so it is proven in this mod.
  `VRE_WeakenedChitin` is the "glass" half - she takes x1.5 damage - and
  `Aggression_HyperAggressive` triples her fight chance.
- **Race proposal.** Plain Human body (she has no flight gene). Body size 1 (decided 2026-09-30),
  health scale 0.4 x 1.75 -> 0.7, MoveSpeed high (3.9, above the beetle's 3.60), light armour. Her
  weapons come from `VRE_RipperBlades` rather than from the race, so the race tool list stays short -
  one set of tools, not two. `combatPower` ~120.
- **Placement.** The hostile swarm only, alongside the arachne, beelzebub and girtablilu. Proposal:
  `Combat` in both groups, `Settlement`, and the `Hunters` role group. The queen can lay her, as she
  can the other three (decided 2026-09-30).
- **Cost.** The usual wiring. No new gene, no new body.

### 3.3 Mothman - the gentle one with the light gene

- **Role.** "Prefers dark places, loves light, gentle, simple minded."
- **Genes.** All twelve exist; the new one is hers. Note that `VRE_PassiveInsect` disables the
  Violent and Hunting work tags, so she will not fight at all, and `VRE_Heatstress` gives her fire
  terror plus x4 flame damage - a moth near a lamp and a moth near a fire are different problems.
  `KindInstinct` forces the Kind trait and `Pain_Extra` forces Wimp.
- **New gene.** "Disorientating lights": -5% consciousness and +10 mood in artificial light at
  **exactly 50%** - the level a lamp gives - and **not** under a growing lamp, decided 2026-09-30.
  The engine separates the two by itself, by light value rather than by def name: a lamp reads 0.5,
  a growing lamp's bright core reads 1.0 (`GENES-FEASIBILITY.md` §4). A glowing cave counts as
  light - "artificial" was read loosely at your call - and only the growing lamp's bright core is
  out. It is the only gene in this batch that needs a hidden hediff *and* a thought.
- **Race proposal.** `BS_HumanoidWithWings_Body`, body size 1 (decided), health scale 0.4 x 2.0 ->
  0.8, no weapons, `combatPower` ~45.
- **Placement.** Decided 2026-09-30: she spawns naturally wherever the hive spawns people, at a **low
  rate**, and she is **much more likely than her sisters to lead a trade caravan**. So: no Combat
  weight, a small `Peaceful` weight, and her own `<trader>true</trader>` pawn kind in the hive's
  Trader group.
- **Cost.** The usual wiring plus the one new gene (gene def, hidden hediff def, thought def, one
  class) and two language keys.

### 3.4 Papillon - the beautiful one

- **Role.** "Beautiful, fragile, great mamono to have around."
- **Genes.** All eleven exist. `ViolenceDisabled` is the strongest of them (she cannot fight, and it
  excludes melee-damage genes by tag), and `Pain_Extra` forces Wimp on top. `VRE_HighGreyMatter` and
  `VRE_SensitiveBrainGoop` make her sharp and psychic; the two sit together fine.
- **Race proposal.** `BS_HumanoidWithWings_Body`, body size 1 (decided), health scale 0.4 x 1.25 ->
  0.5 (the most fragile caste in the mod), no weapons, `combatPower` ~30.
- **Placement.** Decided 2026-09-30: the mothman's rule exactly - natural spawn at a low rate, a
  small `Peaceful` weight, and a `<trader>true</trader>` kind so the hive's caravans are often hers.
- **Cost.** The usual wiring. No new gene at all - she is the cheapest of the four.

### 3.5 Soldier beetle - a second pass

She already ships. This mockup is not a new caste but a list of changes, and some of it is already
done. Her live gene list is: `PMM_Gene_Insect`, `Robust`, `PMM_Gene_FlightWeak`, `PMM_Gene_LargeFrame`,
`MeleeDamage_Strong`, `VRE_HardenedChitin`, `VRE_CuticleShell`, `VRE_InefficientMidgut`,
`VRE_Stenothermic`, `VRE_InsectJellyDependency`.

| Mockup asks for | Today | Proposal |
|---|---|---|
| hardened chitin | already hers | nothing to do |
| one of ripper blades / charger claw / megaspider horn | not hers; her race carries its own melee tools | the one-of-three roll. The mod already answers who owns the tools - the girtablilu's race has **none**, and `VRE_ChargerClaws` is her weapon - so the clean shape is to move the beetle's tools onto the gene and keep one (`GENES-FEASIBILITY.md` §6) |
| antimicrobial chitin | not hers | add `VRE_AntimicrobialPeptides` |
| hard locked joints | not hers | add `VRE_HardLockedJoints` (she is already "slow"; this makes it a real cost) |
| vocal chitters | not hers | **out, decided 2026-09-30** - it silences her. Swapped for `VRE_LowOctopamine`, which keeps her voice and doubles her lovin' cooldown |
| ecdysonal overdrive | not hers | **in, decided** - she gets the KillThirst need. The girtablilu already carries it, so it is proven here |
| antennas | **not on her list** | check her art first - the family norm is to carry it |
| great melee | not hers | add `AptitudeRemarkable_Melee` |
| awful social | not hers | add `AptitudeTerrible_Social` |
| poor crafting, construction, intellect | not hers | add the three aptitudes |
| -50 opinion and more fights with her own kind | does not exist | the quarrelsome gene aimed at her own gene, so it is mutual (`GENES-FEASIBILITY.md` §5) |

**Decided 2026-09-30: rebuild her to this list.** That is a real change rather than an addition, and
she loses six genes she carries today:

| Dropped | What it was doing |
|---|---|
| `Robust` | `IncomingDamageFactor` 0.75, which stacked with `VRE_HardenedChitin`'s own 0.75 - together she took 56% of incoming damage, and without it she takes 75%. A third more hurt than today |
| `VRE_CuticleShell` | immunity to Flu, Malaria and Sleeping Sickness. She catches them again |
| `VRE_InefficientMidgut` | the raised food capacity that made her eat more |
| `VRE_Stenothermic` | her narrow comfort band (`ComfyTemperatureMin` +4, `ComfyTemperatureMax` -4) |
| `PMM_Gene_FlightWeak` | her weak flight. The mockup does not list it, and it does not say to drop it either |
| `PMM_Gene_LargeFrame` | the caravan frame, same note |

Her weapon also changes owner, decided 2026-09-30: her race's tool list is **emptied** and one of the
three VRE weapon genes becomes her weapon, which is the girtablilu's shape (that race has **no** melee
tools at all). Her damage then follows VRE's tuning rather than the megaspider's exact numbers, so
`archive/PLAN.md` §3's "exact DPS parity" stops describing her.

---

## 4. The order of work, with gates

Two new genes first, because they are the only part that needs code, and the code is what a review
can still change cheaply.

| Phase | Work | Gate before moving on |
|---|---|---|
| 1 | The two new genes: defs, classes, placeholder icons | **Built 2026-09-30** - `build.sh` clean, XML parses, deployed; no language keys were needed. Left to check in game: give the genes to pawns in the dev gene editor, then look for "quarrelsome kin -50" between two carriers and "giddy in the light" in a lamp-lit room |
| 2 | Mothman and papillon: race, tracker, xenotype, pawn kind, rosters, corpse list, icons | boot with zero red errors, spawn each in dev mode, check her graphics, her gene page and her place in a hive |
| 3 | Hornet: same wiring plus the hive's new Combat group and the dislike gene aimed at honey | boot clean; a hive village fields her; she fights the player's bees-not-her-own (the bee is unbothered) |
| 4 | Mantis: wiring plus swarm rosters | boot clean; she appears in a swarm raid, cannot speak, and her KillThirst fires |
| 5 | Soldier beetle second pass | boot clean; the roll lands one of the three weapons; her opinion of her sisters reads -50 |
| 6 | Docs: one `HANDOFF.md` section per caste change, one changelog line each | `changelog-check.sh` passes |

Each phase is one `build.sh` + `sync.sh` + an in-game check, the way the 2026-09-27 honey pass was
done. Phases 2 to 5 are independent and can be reordered.

---

## 5. Open questions - none left

All thirteen decisions are in §0, and nothing in this plan is waiting on an answer. The last one, the
queen's brood list, was decided 2026-09-30: **all four castes go in**, so the abaddon can lay any of
them on purpose and any of them can also come out of her "a random mamono" draw.

What is left is work, not questions: phase 1 of §4 - the two new genes - and then one phase per caste.

---

## 6. What this costs

| Item | Count |
|---|---|
| New races + trackers | 4 |
| New xenotypes | 4 |
| New pawn kinds | 6 (four castes, plus a `<trader>true</trader>` kind each for the mothman and the papillon) |
| New bodies | 0 (the winged body is already in use) |
| New gene defs | 3 (the light gene, its hidden hediff, the quarrelsome gene) |
| New thought defs | 2 (the light thought, the quarrel thought) |
| New C# files | ~3 small ones (light gene class + worker, quarrel worker, weapon roll) |
| New icons | 4, hand-drawn at 64x64 like the shipped xenotype icons |
| Rosters touched | 2 factions |
| Docs | 1 `HANDOFF.md` section per caste, 1 changelog line per change, this file and the feasibility study |

The risk sits almost entirely in phase 1. Phases 2 to 5 are the work the honey bee's pass already
proved: def wiring, rosters, and an in-game check.

---

## 7. Papillon maturation (moved here from `archive/PLAN.md`, 2026-10-01)

The greenworm is the larval form: with enough mana she pupates into a papillon. The lore is already
in the mod - `Defs/BackstoryDefs/Backstories_Insect.xml` says a greenworm matures into a papillon -
and the machinery to do it already exists. **Nothing in this section is built**, and it is not one of
the phases above: it depends on phase 2, which is what gives it a papillon (`PMM_InsectPapillon`) to
turn into.

**The swap is already built and never used.** `MamonoTransformation.ConvertXenotype` in core is the
mamono-to-mamono path, written for exactly this case: it strips the old xenotype's signature endogenes,
adds the new set, swaps the B&S race (`ApplyXenotypeRace`), refreshes an unborn baby's pregnancy
snapshot and dirties the graphics. Nothing calls it yet, which is why step 0 below is a test and not a
formality.

**Locked decisions (user, 2026-09-27)**

1. One way. A papillon never goes back to a greenworm.
2. Mana is the fuel, and spent mana becomes charges. The comp watches the mana bar and counts every
   *fall* as mana spent; refills never subtract. That one rule covers both spenders - her passive
   drain, and giving essence away - and it also means a nest keeps her a grub:
   `CompHiveNourishment` tops her up at 1.5 bars a day, so charges do not accrue while she sits in a
   hive.
3. Colony greenworms only. Off-map village pawns never tick, so they are already safe, and spawned
   raiders are too: the greenworm is listed under `Peaceful` alone and in no raid group (verified
   2026-09-27 - the `Combat` groups list the devil bug, the giant ant, the soldier beetle and the
   mosquito, never her).
4. A cocoon, 15 days, reusing the abaddon egg sac's own art (`Things/Building/PMM_EggSac`). A hediff
   cannot render on its own, so the cocoon is a building spawned on her tile - the sac def is
   `PassThroughOnly`, so she can share the tile - with an immobility hediff holding her still for the
   duration.
5. The papillon's own genes, abilities and size are hers now (phase 2); what is left deferred is any
   ability the maturation itself needs. The comp reads its target xenotype as a defName from XML and
   looks it up with `GetNamedSilentFail`, so it ships dormant and harmless.

**Steps**

0. Prove `ConvertXenotype` on a dev-spawned greenworm: race, genes, graphics and corpse category must
   all follow. If it misbehaves, the plan changes here.
1. `Source/Insects/PapillonMaturation.cs`: `CompProperties_PapillonMaturation` (`chargesNeeded`,
   `manaPerCharge`, `cocoonDays`, `requireColonist`, `targetXenotype`) on the greenworm race. The
   charge accumulator is scribed, so a reload cannot lose progress. One `PMMLog.Message` line in dev
   mode.
2. The cocoon: a `PMM_Cocoon` ThingDef (sac texPath, its own comp), the immobility hediff, and the
   hatch on expiry. Open question: what a killed cocoon means - losing her, or hatching early.
3. The papillon herself: `CASTES-PLAN.md` phase 2 builds her.
4. Paperwork: a `HANDOFF.md` section, changelog lines, and the backstory note that still says the
   in-game mamono keeps the worm body.
