# Project Mamono Insects - Design Plan

The six vanilla insects become mamonos: humanlike pawns (Human body, women)
whose xenotypes carry `ProjectMamono_Mamono`. Stats are copied 1:1 from the insect defs -
the bug girls hit exactly as hard and take exactly the same punishment as the bugs
they replace. Named after their MGE counterparts (`MGEWiki/Insects/`).

> **This is the original design, not the shipped one.** It was written around a
> global spawn swap plus tameable wild mamonos. Both were dropped and deleted on
> 2026-09-20, and the mod shipped as two insector tribes instead. `HANDOFF.md` is the
> authority for what the mod is and what is left, and it wins wherever the two
> disagree. What is still useful here is the design reference: the species table
> above, the stat table in §3 and §7 on the Big & Small conversion. §2 (the swap),
> §5 (the wild layer) and §6 (their risk list) were deleted on 2026-09-24.

| RimWorld insect | MGE mamono      | Source DLC        |
|-----------------|---------------|-------------------|
| Megascarab      | Devil Bug     | Core              |
| Spelopede       | Giant Ant     | Core              |
| Megaspider      | Soldier Beetle| Core              |
| Larva           | Greenworm     | DLC               |
| Locust          | Vamp Mosquito | DLC               |
| HiveQueen       | Abaddon       | DLC               |

---

## 1. Mod skeleton (mirrors Reptiles)

- packageId `PMM.Insects`, assembly `PMM_Insects.dll`, namespace `PMM_Insects`.
- Hard deps: Harmony, Biotech, PMM.Core, VEF, VRE Insector, VFEI2 and
  BetterPrerequisites. **No DLC is needed**: the Greenworm, Vamp Mosquito and
  Abaddon kinds are MayRequire-gated, so the three Core species work without it.
- `loadAfter`: Harmony, Biotech, PMM.Core, VEF, VRE Insector, VFEI2.
- `build.sh` / `sync.sh` / `release.sh` copied from Reptiles (same csc pattern,
  minus the VEF reference).

## 3. Keeping their stats - one cloned humanlike race per species

Genes cannot touch `baseHealthScale`/`baseBodySize` (the Queen's 980% HP needs
race-level data), so each species gets a humanlike race cloned from `Human`
(`ParentName="BasePawn"`, `body Human`, `renderTree Humanlike`, Human life stages,
humanlike think trees, human food, human red blood - these are women, not bugs, and
whether she is an enemy is her tribe's business, not her race's).
On top of the clone, the insect's numbers are overlaid:

| Stat (race/statBases) | Devil Bug | Giant Ant | Soldier Beetle | Greenworm | Vamp Mosq. | Abaddon |
|---|---|---|---|---|---|---|
| MoveSpeed | 3.75 | 3.65 | 3.60 | 2.0 | 3.0 | 3.4 |
| Armor Sharp/Blunt | 0.72/0.18 | 0.18/0.18 | 0.27/0.18 | - | - | 0.22/0.27 |
| baseHealthScale | 0.4 | 1.7 | 2.5 | 0.25 | 0.7 | 9.8 |
| baseBodySize | 0.2 | 0.8 | 1.2 | 0.2 | 0.6 | 4.5 |
| baseHungerRate | 0.10 | 0.25 | 0.35 | 0.10 | 0.15 | (human) |
| ComfyTempMin/Max | 0/60 | -25/60 | -40/60 | 0/60 | -10/60 | -40/60 |
| MarketValue | 100 | 200 | 500 | 35 | 120 | 2000 |
| FilthRate | 1 | 1 | 1 | 1 | 1 | 28 |
| Toxic/Vacuum resist | 1/1 | 1/1 | 1/1 | 1/1 | 1/1 | 1/1 |
| lifeExpectancy | 10 | 6 | 6 | 4 | 10 | 75 |
| combatPower (pawnkind) | 40 | 75 | 150 | 25 | 55 | 500 |

Per-species extras kept from the bug defs:

- insect **melee tools** (mandibles 5 Bite / head claw 12 Cut / Queen 30-Cut claw)
  - exact DPS parity; the core tease-damage patch converts damage on non-mamono
  victims as usual.
- `ToxicEnvironmentResistance 0.8`; Abaddon: `needsRest=false` (never sleeps),
  `CompProperties_SpreadSludge` + `EggSpew` (her egg sac hatches a `VFEI2_Swarmling`), `CompProperties_LetterOnRevealed`,
  butcherProducts `InsectJelly x150`.
- Greenworm: `CompProperties_SpreadSludge` + `SludgeSpew` (acid spit kept).
- Pawnkinds keep `combatPower` identical so infestation point budgets are
  unchanged, and carry `moveSpeedFactorByTerrainTag` (2x on insect sludge).
- Tribe pawnkinds: `Wildness 0` (they are faction NPCs, not wild animals - 0.99
  wildness on a humanlike would drive them wild).

## 4. Xenotypes & genes

- `PMM_Gene_Insect` - the shared insect gene: +0.15 sharp armor chitin,
  insect-eater description (plain XML gene, no class).
- Six xenotypes (`ProjectMamono_Mamono` + `PMM_Gene_Insect` + species genes):
  Devil Bug (fast legs + great smell), Giant Ant (strong melee + mining),
  Soldier Beetle (robust, sturdy, hidden wings), Greenworm (slow, herbivore
  appetite), Vamp Mosquito (flight + hemogenic + beautiful), Abaddon (tough,
  strong melee, huge wings). Inheritable, all-female,
  `factionlessGenerationWeight 0` (they enter the world through the tribes only).
- Every pawnkind pins its xenotype via `xenotypeSet`, `fixedGender Female`,
  no gear (wild-man pattern).

## 7. Big & Small conversion - Route A (all six species DONE 2026-09-19)

Route A chosen over Sapient Animals: convert each species to a humanlike B&S
race (`ParentName="Human"` + `BigAndSmall.RaceExtension` + race-tracker hediff
`ParentName="BS_DefaultRaceTracker"`).

All six species live in `Defs/ThingDefs/Races_InsectMamono_BS.xml`. The pattern,
proven on the Devil Bug pilot and then applied to the rest:

- ThingDef on `Human` base. Race-level data 1:1 from the bug: `baseBodySize`,
  `baseHealthScale`, `baseHungerRate`, insect flesh/ichor/meat, melee tools,
  species comps (sludge/egg spew, queen's letter, jelly butcher yield).
  New: `BigAndSmall.RaceExtension` -> the species tracker.
- `SM_BodySizeMultiplier` in statBases matches `baseBodySize` - this is the
  stat B&S's scaler actually reads for humanlike render size (`baseBodySize`
  alone only feeds vanilla mechanics; a 0.6 devil bug rendered 1.0 until this
  was added). Abaddon at 4.5 gets `renderCacheOff` from the scaler for free.
- Armor lives ONLY on the tracker as a stat stage - a copy in race statBases
  stacks (devil bug showed 1.44 sharp in-game before the dedup).
- Durability layers: `internalDamageDivisor 3` on Devil Bug's tracker (hard
  shell, squishy inside), `HealthScalePatch.cs` x2.5 for the two fragile
  species (Devil Bug 0.4, Greenworm 0.25 - the rest need no help). The old
  `PMM_Hediff_PartToughness` core hediff is gone (removed 2026-09-23): a
  hediff cannot change part HP, and `addedPartProps` only applies on
  `Hediff_AddedPart`.
- Trackers stay visible in the Health tab - `RaceTracker` hardcodes
  `Visible = true`; we follow the framework (an attempted Visible postfix was
  reverted).
- `Races_InsectMamono.xml` (the old `BasePawn` clone file) is gone - deleted
  2026-09-20. `Races_InsectMamono_BS.xml` is now the single source of race defs.

Test gate: each species renders at its size (devil bug/greenworm/mosquito
short, soldier beetle tall-ish, Abaddon towering), armor matches the bug's
numbers exactly once (no doubling), and hive nourishment works.

## 8. Out of scope

Custom pawn art (antennae/carapace overlays - the castes use human rendering, and
`mktex.py` gene icons except the shared insectoid gene's, which is hand art since
2026-09-30), Greenworm -> Papillon maturation (planned in its own
section below), and DLC lair-boss loop polish. The settlements, the RulePack namers
and the jelly economy this section used to list all shipped with the tribes - see
`HANDOFF.md` §4.
## Papillon maturation (planned 2026-09-27, decisions locked)

The greenworm is the larval form: with enough mana she pupates into a papillon. The
lore is already in the mod - `Defs/BackstoryDefs/Backstories_Insect.xml` says a
greenworm matures into a papillon - and the machinery to do it already exists.

**The swap is already built and never used.** `MamonoTransformation.ConvertXenotype`
in core is the momo-to-momo path, written for exactly this case: it strips the old
xenotype's signature endogenes, adds the new set, swaps the B&S race
(`ApplyXenotypeRace`), refreshes an unborn baby's pregnancy snapshot and dirties the
graphics. Nothing calls it yet, which is why step 0 below is a test and not a
formality.

**Locked decisions (user, 2026-09-27)**

1. One way. A papillon never goes back to a greenworm.
2. Mana is the fuel, and spent mana becomes charges. The comp watches the mana bar
   and counts every *fall* as mana spent; refills never subtract. That one rule
   covers both spenders - her passive drain, and giving essence away - and it also
   means a nest keeps her a grub: `CompHiveNourishment` tops her up at 1.5 bars a
   day, so charges do not accrue while she sits in a hive.
3. Colony greenworms only. Off-map village pawns never tick, so they are already
   safe, and spawned raiders are too: the greenworm is listed under `Peaceful`
   alone and in no raid group (verified 2026-09-27 - the `Combat` groups list the
   devilbug, giant ant, soldier beetle and mosquito, never her).
4. A cocoon, 15 days, reusing the abaddon egg sac's own art
   (`Things/Building/PMM_EggSac`). A hediff cannot render on its own, so the cocoon
   is a building spawned on her tile - the sac def is `PassThroughOnly`, so she can
   share the tile - with an immobility hediff holding her still for the duration.
5. The papillon's own genes, abilities and size are deferred; the user will spec her
   later. The comp therefore reads its target xenotype as a defName from XML and
   looks it up with `GetNamedSilentFail`, so this ships dormant and harmless until
   that xenotype exists.

**Steps**

0. Prove `ConvertXenotype` on a dev-spawned greenworm: race, genes, graphics and
   corpse category must all follow. If it misbehaves, the plan changes here.
1. `Source/Insects/PapillonMaturation.cs`: `CompProperties_PapillonMaturation`
   (`chargesNeeded`, `manaPerCharge`, `cocoonDays`, `requireColonist`,
   `targetXenotype`) on the greenworm race. The charge accumulator is scribed, so a
   reload cannot lose progress. One `PMMLog.Message` line in dev mode.
2. The cocoon: a `PMM_Cocoon` ThingDef (sac texPath, its own comp), the immobility
   hediff, and the hatch on expiry. Open question: what a killed cocoon means -
   losing her, or hatching early.
3. The papillon herself, once specified: xenotype, race, tracker, icon and the
   corpse entry.
4. Paperwork: this section, a `HANDOFF.md` section, changelog lines, and the
   backstory note that still says the in-game mamono keeps the worm body.
