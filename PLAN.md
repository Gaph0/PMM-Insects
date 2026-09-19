# Project Momo Insects — Design Plan

The six vanilla/Odyssey insects become momos: humanlike pawns (Human body, women)
whose xenotypes carry `ProjectMomo_Momo`. Stats are copied 1:1 from the insect defs —
the bug girls hit exactly as hard and take exactly the same punishment as the bugs
they replace. Named after their MGE counterparts (`MGEWiki/Insects/`).

| RimWorld insect | MGE momo      | Source DLC        |
|-----------------|---------------|-------------------|
| Megascarab      | Devil Bug     | Core              |
| Spelopede       | Giant Ant     | Core              |
| Megaspider      | Soldier Beetle| Core              |
| Larva           | Greenworm     | Odyssey (soft)    |
| Locust          | Vamp Mosquito | Odyssey (soft)    |
| HiveQueen       | Abaddon       | Odyssey (soft)    |

---

## 1. Mod skeleton (mirrors Reptiles)

- packageId `PMM.Insects`, assembly `PMM_Insects.dll`, namespace `PMM_Insects`.
- Hard deps: Harmony, Biotech, PMM.Core. **Odyssey is a soft dep** — Larva/Locust/
  HiveQueen defs are MayRequire-gated, and the swap patch skips kinds that failed
  to load; the three Core species work without the DLC.
- `loadAfter`: Harmony, Biotech, PMM.Core. VEF not needed (no egg-laying here —
  insects reproduce via the normal momo pregnancy).
- `build.sh` / `sync.sh` / `release.sh` copied from Reptiles (same csc pattern,
  minus the VEF reference).

## 2. Core mechanism — the generation swap

A Harmony **prefix on `PawnGenerator.GeneratePawn(ref PawnGenerationRequest)`**:
when `request.KindDef.defName` is one of the six vanilla insect kinds, it is
replaced with the matching PMM pawnkind. Every spawn source funnels through
`PawnGenerator`, so the single patch catches infestations/hives, Odyssey insect
lairs and egg sacs, wild biome spawns, ancient dangers, deep-drill bugs, quests
and trader livestock.

Context split inside the patch:

- `request.Faction == Insect` -> `PMM_Insect<Species>` kind (stays in the Insect
  faction: hostile, joins the hive lord, takes part in raids).
- otherwise -> `PMM_Wild<Species>` kind (wild woman, tameable).

Everything else in the request is left untouched — faction, position, lord
assignment flow through as before.

## 3. Keeping their stats — one cloned humanlike race per species

Genes cannot touch `baseHealthScale`/`baseBodySize` (the Queen's 980% HP needs
race-level data), so each species gets a humanlike race cloned from `Human`
(`ParentName="BasePawn"`, `body Human`, `renderTree Humanlike`, Human life stages,
humanlike think trees, human food, human red blood — these are women, not bugs,
and the Insect faction's own hostility makes them enemies regardless of race).
On top of the clone, the insect's numbers are overlaid:

| Stat (race/statBases) | Devil Bug | Giant Ant | Soldier Beetle | Greenworm | Vamp Mosq. | Abaddon |
|---|---|---|---|---|---|---|
| MoveSpeed | 3.75 | 3.65 | 3.60 | 2.0 | 3.0 | 3.4 |
| Armor Sharp/Blunt | 0.72/0.18 | 0.18/0.18 | 0.27/0.18 | — | — | 0.22/0.27 |
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
  — exact DPS parity; the core tease-damage patch converts damage on non-momo
  victims as usual.
- `ToxicEnvironmentResistance 0.8`; Abaddon: `needsRest=false` (never sleeps),
  `CompProperties_SpreadSludge` + `EggSpew` (spawned Larvae are swapped into
  Greenworm daughters by the same patch), `CompProperties_LetterOnRevealed`,
  butcherProducts `InsectJelly x150`.
- Greenworm: `CompProperties_SpreadSludge` + `SludgeSpew` (acid spit kept).
- Pawnkinds keep `combatPower` identical so infestation point budgets are
  unchanged, and carry `moveSpeedFactorByTerrainTag` (2x on insect sludge).
- `Insect` faction pawnkinds: `Wildness 0` (they are faction NPCs, not wild
  animals — 0.99 wildness on a humanlike would drive them wild).

## 4. Xenotypes & genes

- `PMM_Gene_Insect` — the shared insect gene: +0.15 sharp armor chitin,
  insect-eater description (plain XML gene, no class).
- Six xenotypes (`ProjectMomo_Momo` + `PMM_Gene_Insect` + species genes):
  Devil Bug (fast legs + great smell), Giant Ant (strong melee + mining),
  Soldier Beetle (robust, sturdy, hidden wings), Greenworm (slow, herbivore
  appetite), Vamp Mosquito (flight + hemogenic + beautiful), Abaddon (tough,
  strong melee, huge wings). Inheritable, all-female,
  `factionlessGenerationWeight 0` (they only enter the world through the swap).
- Every pawnkind pins its xenotype via `xenotypeSet`, `fixedGender Female`,
  no gear (wild-man pattern).

## 5. Wild spawns — the four-piece wild-man recipe

Hard-learned on Reptiles (all four pieces or the pawn walks off the map on her
first think tick):

1. swap spawns wild kinds **factionless**;
2. `IsWildMan` postfix -> tameable;
3. `WildManShouldReachOutsideNow` postfix -> they linger;
4. `Defs/ThinkTreeDefs/ThinkTrees_InsectWild.xml` — `MainWildManBehaviorCore` +
   idle wander inserts for all six `PMM_Wild*` kinds.

The `Insect`-faction kinds are excluded from the wild patches — raid/infestation
bugs stay hostile NPCs; downed ones go through normal capture/recruit.

## 6. Risks / test gates

- **Highest risk:** `LordJob_DefendAndExpandHive` with humanlike pawns (vanilla
  only ever puts animals in it). Fallback if broken: patch the hive comp to a
  humanlike-compatible defence lord.
- `CanBeDormant`/`WakeUpDormant` on humanlikes (ancient danger bugs) — verify.
- Trader caravans selling insects will sell a bug girl — on-theme quirk, verify
  no error on purchase.
- Manhunter-on-harm/tame-fail mechanics disappear (humanlikes cannot manhunt) —
  accepted loss.
- Boot with zero red errors, with and without Odyssey (MayRequire gating).

## 7. Big & Small conversion — Route A (all six species DONE 2026-09-19)

Route A chosen over Sapient Animals: convert each species to a humanlike B&S
race (`ParentName="Human"` + `BigAndSmall.RaceExtension` + race-tracker hediff
`ParentName="BS_DefaultRaceTracker"`).

All six species live in `Defs/ThingDefs/Races_InsectMomo_BS.xml`. The pattern,
proven on the Devil Bug pilot and then applied to the rest:

- ThingDef on `Human` base. Race-level data 1:1 from the bug: `baseBodySize`,
  `baseHealthScale`, `baseHungerRate`, insect flesh/ichor/meat, melee tools,
  species comps (sludge/egg spew, queen's letter, jelly butcher yield).
  New: `BigAndSmall.RaceExtension` -> the species tracker.
- `SM_BodySizeMultiplier` in statBases matches `baseBodySize` — this is the
  stat B&S's scaler actually reads for humanlike render size (`baseBodySize`
  alone only feeds vanilla mechanics; a 0.6 devil bug rendered 1.0 until this
  was added). Abaddon at 4.5 gets `renderCacheOff` from the scaler for free.
- Armor lives ONLY on the tracker as a stat stage — a copy in race statBases
  stacks (devil bug showed 1.44 sharp in-game before the dedup).
- Durability layers: `internalDamageDivisor 3` on Devil Bug's tracker (hard
  shell, squishy inside), `HealthScalePatch.cs` x2.5 for the two fragile
  species (Devil Bug 0.4, Greenworm 0.25 — the rest need no help), and the
  shared core hediff `PMM_Hediff_PartToughness` for B&S-added parts (wings).
- Trackers stay visible in the Health tab — `RaceTracker` hardcodes
  `Visible = true`; we follow the framework (an attempted Visible postfix was
  reverted).
- `Races_InsectMomo.xml` (the old `BasePawn` clone file) is gone — deleted
  2026-09-20. `Races_InsectMomo_BS.xml` is now the single source of race defs.

Test gate: each species renders at its size (devil bug/greenworm/mosquito
short, soldier beetle tall-ish, Abaddon towering), armor matches the bug's
numbers exactly once (no doubling), hive nourishment works, and the wild-man
recipe still functions on Human-based races.

## 8. Out of scope (later phases)

Custom pawn art (antennae/carapace overlays — phase 1 uses human rendering +
gene icons via mktex.py), RulePack namers, a proper hive faction with
settlements, jelly economy, Greenworm->Papillon maturation (MGE cocoon lore),
Odyssey lair-boss loop polish.
