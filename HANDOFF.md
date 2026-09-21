# PMM Insects — Insector Tribes hand-off

For the next agent picking this up, and for the user. This file is the plan for
the Insects overhaul.

**Status: design locked. Phases 1 to 4 are done and field-tested (2026-09-20);
phase 5 is built and awaiting its field test.**
The spawn swap and the wild layer are gone, the two insector tribes exist with
their names, their VFEI2 villages and their castes, and the six castes now carry
their VRE gene packages (§5.3). Phase 3 added the melee groups, the four vanilla
work groups and the children; **the user dropped the children on 2026-09-20**, so no
tribal children ship and that research is kept for reference only (§5.5, §12.8).
Three extras came out of the same conversation and are built: chitin instead of
human leather from a corpse, insect flesh so eating insect meat is cannibalism to a
momo, and one "momo corpses" line in the item filters (§10, §12.9). That line moved
into core on 2026-09-20, so the slime and elemental momos share it (§11 item 5).
Phases 6 to 8 are open: the hive build gate, the kidnapping rework, trade and
art.

`PLAN.md` holds the old design: a total spawn swap, plus tameable wild momos.
Where the two files disagree, this one wins. `PLAN.md` §5 (the wild-man recipe)
and §2 (the swap) are now history.

---

## 0. How to read this file

- §1 is the change in one page. Read that first.
- §2 is the list of rulings the user already made. Do not re-litigate them.
- §3 to §9 are the build spec.
- §10 to §11 are the work order and the open questions.
- §12 is the evidence: the engine behaviour this plan rests on, with the exact
  methods that were decompiled. Read it before you doubt §4 to §8.

Simple English rules apply to this file: short sentences, everyday words, keep
every def name and path exact.

---

## 1. The change in one page

| | Today | After the overhaul |
|---|---|---|
| Vanilla insects | Removed from the game. Every spawn becomes a momo. | Normal. Real bugs spawn everywhere again. |
| Wild momos | Exist. Spawn from factionless bug spawns, tameable. | Gone. The wild layer is deleted. |
| Insect momos | Two jobs: the vanilla Insect faction, or a wild woman. | One job: citizens, raiders and brood of the new insector tribes. |
| Their home | None. They use vanilla and VFEI2 hives. | Their own villages, built from VFEI2 hive walls and hives. |
| Their character | Six species with their own genes. | Six castes, each with a VRE Insector gene package. |
| Mods required | Harmony, Biotech, PMM.Core, B&S. | Plus VRE Insector, VFEI2 and VEF. All hard. |
| Code | A global prefix on pawn generation. | No pawn-generation hook at all. |

The mod becomes **purely additive**. It stops touching vanilla pawn generation,
so vanilla insects, VFEI2 genelines and other insect mods all work normally.

---

## 2. Locked rulings (user, 2026-09-20)

1. **Wild layer: option C.** Delete the swap and the wild momos. Momos live only
   in the tribes.
2. **Castes: option 1.** All six species stay, all six spawn in the tribes and
   villages.
3. **Kidnapping is allowed**, vanilla style. The tribes now have bases, so a
   prisoner can be rescued from one.
4. **VFEI2 is a hard dependency.**
5. **No momos in the vanilla Insect faction.** The tribes own every faction-side
   momo kind.
6. **The hive-abduction wrapper goes.** No "lost forever", no vanish, no special
   letter. Vanilla kidnapping only.
7. **The bond rule stays true:** when a hive momo wins a tsugai bond she never
   joins the colony. She carries the man back to her hive.
8. **No chestburst pregnancy on momos.** Hard no.
9. **Jelly dependency: combat castes only.** Only the castes that read as fighters
   carry `VRE_InsectJellyDependency`. The greenworm does not.
10. **No `VRE_JellySacks` on any momo.** Pawns never make jelly. All of it comes
    from the world or from VFEI2 buildings the player raises.
11. **Melee only.** No ranged castes and no ranged pawn groups.
12. **Children spawn in the villages**, like a vanilla tribe.
13. **The tribes trade.** See §4.6 for one correction about Dragonia.
14. **Village layout: VFEI2's own settlement layouts.** We do not write one.
15. **The tribe starts neutral** with the player, like Dragonia.
16. **Settlement names: the vanilla tribal namer.** `settlementNameMaker` =
    `NamerSettlementTribal`.
17. **Faction names: one small pack holding the five Abaddon names**, ruled. The
    user listed "The Abaddon Hive", "Abaddon Brood", "Abaddon Swarm", "Tribe of
    the Abaddon" and "Abaddon Nest". `leaderTitle` "hive queen" is confirmed.
    See §4.5 for the split between the two tribes.
18. **Two tribes: one neutral, one hostile.** Neutral parents off `TribeBase`,
    hostile off `TribeSavageBase`. See §4.7.
19. **Pherocores are rare trader stock**, not a regular line.
20. **The queen keeps the jelly dependency.**
21. **Both tribes fight the same way.** Only relations, trade and looks differ.
22. **The feral insect faction keeps no bases.** It still raids, infests, spawns
    in caves and ancient dangers, and its bosses stay reachable through the
    thumper buildings. Two hostile factions holding overworld bases read as
    redundant (2026-09-20). See §12.7.
23. **The tribes settle polluted ground only** (§4.8).
24. **Phase 4 gives all six castes full environmental pollution immunity**
    (§5.3). They currently only resist it, like the vanilla bugs.
25. The user picks the final names and numbers. Proposals in this file are
    proposals until then.

---

## 3. File-by-file change list

### Delete

- `Source/Insects/InsectSwap.cs` — the whole `Patch_InsectGenerationSwap`.
  Nothing needs it once the momos are a faction.
- `Source/Insects/WildInsects.cs` — `Patch_InsectIsWildMan` and
  `Patch_InsectShouldNotReachOutside`. No factionless momos means no wild-man
  behaviour to fake.
- `Defs/PawnKindDefs/PawnKinds_InsectWild.xml` — all six `PMM_Wild*` kinds.
- `Defs/ThinkTreeDefs/ThinkTrees_InsectWild.xml` — the wild think trees.

### Keep

- `Source/Insects/InsectsMod.cs` — Harmony bootstrap. Add the new patch classes
  here or beside them.
- `Source/Insects/HealthScalePatch.cs` — still needed for the two fragile
  species.
- `Source/Insects/HiveNourishment.cs` — keep, but widen the hive lookup (§7).
- `Defs/ThingDefs/Races_InsectMomo_BS.xml` — the six races. Still the home of
  every stat.
- `Defs/GeneDefs/*` — the shared insect gene and the chitin skin genes.
- `Defs/BackstoryDefs/Backstories_Insect.xml` — keep, but re-read the stories.
  Any line that talks about being found in the wild now points at the wrong
  place.

### Rewrite

- `Defs/PawnKindDefs/PawnKinds_InsectFaction.xml` — becomes the tribe's kind
  file. Change `defaultFactionDef` from `Insect` to the new faction. Drop the
  `ecoSystemWeight` lines: they only count for factionless pawns, so they do
  nothing here. Rename the file to `PawnKinds_InsectorTribe.xml` if you like.
  Keep `combatPower` at the vanilla bug's value.
- `Source/Insects/HiveBondKidnap.cs` — keep the "she never joins" forced
  kidnap. Move the faction test off `Faction.OfInsects` and onto the new
  faction. Delete the custom letter text.
- `Source/Insects/HiveKidnapVanish.cs` — delete the vanish behaviour. The man
  becomes a normal prisoner.

### Add

Landed already:

- `Defs/FactionDefs/Factions_InsectorTribes.xml` — one shared abstract base and
  two factions, neutral and hostile (§4, §4.7).
- `Defs/RulePackDefs/Namers_Insector.xml` — the five faction names (§4.5).
- `Source/Insects/InsectMomoCorpses.cs` — registers the six insect races on the
  family's shared `PMM_MomoCorpses` line. The category def and the mover moved into
  `Project Momo` on 2026-09-20 (`Defs/ThingCategoryDefs/ThingCategories_MomoCorpses.xml`
  and `Source/ProjectMomo/MomoCorpses.cs`), so the insects, the slimes and the
  elementals share one line (§11 item 5, §12.9).
- `Source/Insects/InsectPheromones.cs` — the pheromone patches VRE keeps inside
  their gene, moved onto `PMM_Gene_Insect` (§5.3).
- `Defs/GeneDefs/Genes_Insect.xml` — extended, not new: chitin skin, feelers, full
  toxin immunity, pheromones and insect flesh all live on this one gene (§5.3).
- `Patches/InsectFaction_NoSettlements.xml` — the feral faction's bases off the map
  (§12.7).
- `Defs/AbilityDefs/Abilities_EggSpew.xml` and `Defs/ThingDefs/Things_EggSpew.xml`
  — the brood, landed 2026-09-20 (§5.4). Odyssey-gated, and it hatches a
  `VFEI2_Swarmling`.

Still to come:

- `Defs/TraderKindDefs/Trader_InsectorTribe.xml` — the caravan trader (§4.6).
- `Textures/` — art for the neutral tribe (§4.4).
- `About/About.xml` — already carries the dependency block (§9).
- `CHANGELOG.md` — entries as the work lands (§13).

Dropped, do not re-add without asking: `Defs/PawnKindDefs/PawnKinds_InsectChild.xml`
and `Source/Insects/ChildStagePatch.cs`. Both were written, shipped for a few hours
and removed on 2026-09-20 (§5.5).

---

## 4. The faction

### 4.1 FactionDef

Proposed defName `PMM_InsectorTribe`. Rename if the user prefers.

Parent for the shared abstract base `PMM_InsectorTribeBase`: vanilla `TribeBase`
(`Data/Core/Defs/FactionDefs/Factions_Misc.xml`, line 219). That is the abstract
base the **gentle tribe** uses, so it is the neutral tribal parent. It brings
Neolithic tech, the tribal meme weights, `settlementGenerationWeight 1` and
`canStageAttacks true`.

The neutral faction parents off `PMM_InsectorTribeBase`. The hostile faction
parents off `TribeSavageBase` instead, because that is the one carrying
`permanentEnemy`. See §4.7. Neither faction uses `TribeRoughBase`.

`TribeBase` also sets `factionNameMaker NamerFactionTribal` and
`settlementNameMaker NamerSettlementTribal`. We override the faction namer (§4.5)
and keep the settlement one.

Follow the Reptiles house pattern too. Its abstract faction base in
`Project Momo Reptiles/Defs/FactionDefs/Factions_Reptile.xml` shows the project
conventions: `categoryTag`, `listOrderPriority`, `settlementGenerationWeight`,
`maxConfigurableAtWorldCreation`, `configurationListOrderPriority`,
`allowedCultures`, `backstoryFilters`, `raidLootMaker`, and a
`maxPawnCostPerTotalPointsCurve`. The curve matters here, because the queen
costs 500 points.

Fields to set, with why:

| Field | Value | Why |
|---|---|---|
| `defName` | `PMM_InsectorTribe` | Placeholder. See §11. |
| `label` | "insector tribe" | Player-facing. |
| `description` | own text | Player-facing. Simple English. |
| `pawnSingular` / `pawnsPlural` | "insector" / "insectors" | |
| `leaderTitle` | "hive queen" | Reptiles sets `leaderTitle` on both its factions. Fits the Abaddon. |
| `settlementGenerationWeight` | `1` | Inherited from `TribeBase`. Keeps villages on the map. |
| `factionNameMaker` | `PMM_NamerFactionInsectorHive` (neutral), `PMM_NamerFactionInsectorSwarm` (hostile) — §4.5 | |
| `settlementNameMaker` | `NamerSettlementTribal` | Vanilla tribal namer, ruled. |
| `factionIconPath` | neutral `World/WorldObjects/Expanding/Insects` (vanilla), hostile `World/WorldObjects/Expanding/PMM_InsectorSwarm` (own art) | The neutral icon is still a placeholder. |
| `settlementTexturePath` | neutral `UI/InsectoidHive` (VFEI2), hostile `World/WorldObjects/Expanding/PMM_InsectorSwarm` (own art) | Mandatory: leaving it unset throws on every settlement draw. It is what draws the village on the world map, not `factionIconPath`. |
| `canStageAttacks` | `true` | Inherited. See the note below. |
| `raidLootMaker` | `TribeRaidLootMaker` | Vanilla def. The Broods use it too. |
| `requiredCountAtGameStart` | `1` | House pattern. The world always gets one tribe. |
| `startingCountAtWorldCreation` | `1` | **This is faction instances, not settlements.** In Reptiles, 4 produced four duplicate Dragonias. |
| `maxConfigurableAtWorldCreation` | `10` | House pattern. |
| `allowedCultures` | `Corunan` | House pattern. |
| `backstoryFilters` | `Tribal` | House pattern. |
| `xenotypeSet` | `Inherit="False"`, the six momo xenotypes | Keeps random pawns inside the mod. |
| `autoFlee` | `false` | Bugs do not run. |
| `colorSpectrum` | chitin tones | World map and letters. |
| `maxPawnCostPerTotalPointsCurve` | own curve, see §4.7 | The vanilla tribal curve cannot allow a 500-point pawn into a village at all. |
| `caravanTraderKinds` | `PMM_InsectorTribeTrader` | §4.6. |
| `modExtensions` | `KCSG.CustomGenOption` | The village base. See §4.3. |

**Neutral, but still able to raid.** The user asked for neutral. `TribeBase`
sets `canStageAttacks true`, so leave it: the tribe starts neutral and can raid
once relations sour. That is how vanilla gentle tribes behave. Dragonia does not
inherit it, which is why Dragonia never raids. Clearing the field gives you the
Dragonia behaviour exactly, but then the momos never raid at all, and §4.2 has
nowhere to happen.

Raids are melee-only. The momos carry no weapons and no apparel, so there is no
ranged group, and no attack ability is added for one. The two spew abilities the
mod already has stay (§5.3).

### 4.2 Pawn groups

`pawnGroupMakers` decides the whole raid texture. Groups to declare:

| Group kind | Who goes in it | Notes |
|---|---|---|
| `Combat` | The five fighters, weighted by caste | The main raid. Melee only. |
| `Combat-melee` | Devil Bug, Soldier Beetle, Giant Ant | The only combat group we need, since nobody is armed. |
| `Peaceful` | Giant Ant, Devil Bug, and the six child kinds | Village civilians and children. |
| `Settlement` | The five fighters, queen weighted low | **Required.** Without this group the base spawns empty. |
| `Miners` / `Loggers` / `Farmers` | Giant Ant, Devil Bug | Ideology-gated in vanilla; copy the vanilla tribe pattern. |
| `Trader` | the trader kind plus guards | §4.6. |
| `leaders` (inside a group) | Abaddon, or a soldier beetle variant | `PawnGroupMaker.leaders` is a real field. Vanilla tribes put their chiefs there. |

Do **not** declare `Combat-ranged` or `Settlement_RangedOnly`. Nobody carries a
weapon, so those groups would field unarmed pawns at range. Melee only.

Two rules learned from the vanilla numbers:

- **The queen is not a raid filler.** At `combatPower 500` she eats a whole raid
  budget. Put her in `Settlement` and `leaders` only, or give the faction a
  `maxPawnCostPerTotalPointsCurve` so she cannot roll into a small raid.
- **The greenworm is not a soldier.** At `combatPower 25` and health scale 0.25
  she is brood. Keep her in `Peaceful` and in the queen's brood spawn (§5.4).

### 4.3 The village base

This is the part the user asked about, and it works. See §12.1 for the proof.

**This is the plan.** Attach the KCSG extension and point it at VFEI2's own
layouts. The field takes a **list**, and KCSG picks one at random per village, so
listing several gives villages of different sizes for free:

```xml
<modExtensions Inherit="False">
  <li Class="KCSG.CustomGenOption">
    <preventBridgeable>true</preventBridgeable>
    <tryFindFreeArea>true</tryFindFreeArea>
    <chooseFromSettlements>
      <li>VFEI2_InsectoidSettlement</li>
      <li>VFEI2_InsectoidSettlementRatingOne</li>
      <li>VFEI2_InsectoidSettlementRatingTwo</li>
      <li>VFEI2_InsectoidSettlementRatingThree</li>
    </chooseFromSettlements>
  </li>
</modExtensions>
```

- `VFEI2_InsectoidSettlement` is the big one: 80x80, 10-20 formations.
- `VFEI2_InsectoidSettlementRatingOne/Two/Three` are the smaller ones. VFEI2
defines them in `Defs/QuestScriptDefs/Script_EmergingHive.xml` under the
abstract parent `VFEI_LayoutInsectoidHiveBase`, so they cost us nothing.
- All four draw formations by the tag `VFEI2_InsectoidFormation`. Tags are
global, so we inherit all 35 of VFEI2's formation layouts.
- The result: villages built from `VFEI_HiveWall`, `VFEI2_InsectJellyWall` and
  `VFEI2_RoyalJellyWall`, standing on `VFEI2_Creep`, with vanilla `Hive` and
  `GlowPod` inside.

If a momo layout is ever wanted on top of these, it is added to the same list,
so the four VFEI2 layouts keep rolling. That is where `stockpileOptions`
(`fillWithDefs` = `InsectJelly`, `VFEI2_Chitin`, `VFEI2_RoyalInsectJelly`) and
`roadOptions` (`mainRoadDef` = `VFEI2_Creep`) would go. Reel's does both. Not
planned now.

Usable `CustomGenOption` fields: `chooseFromSettlements`, `chooseFromlayouts`,
`tiledStructures`, `symbolResolver(s)`, `scatterThings`, `filthTypes`,
`scatterChance`, `tryFindFreeArea`, `preGenClear`, `fullClear`,
`preventBridgeable`, `clearFogInRect`, `scaleWithQuest`.

### 4.4 Art

- Hostile tribe: its own art, made by the user on 2026-09-20, at
  `Textures/World/WorldObjects/Expanding/PMM_InsectorSwarm.png` (128x128 RGBA, a
  spiked hive crest). Both `factionIconPath` and `settlementTexturePath` point at
  it, the Dragonia pattern. Two fields, two jobs: the icon shows in the faction
  list and world creation, the settlement texture draws each village on the world
  map. Split them if a separate village shape is wanted later.
- Neutral tribe: still placeholders - VFEI2's `UI/InsectoidHive` for its villages
  and the vanilla `World/WorldObjects/Expanding/Insects` for its icon.
- The art convention is single-colour line art on transparent, 128x128, drawn
  tinted by the faction colour. The user's crest follows it. Reference copies of
  the vanilla shapes live in `Tools/icon-refs/` (excluded from the game copy).
- `Tools/mktex.py` can generate placeholder art if needed; the Reptiles mod uses
  it for its faction art.
- Culture: with Ideology on, allow the vanilla `Corunan` culture, the house
  pattern. A custom `CultureDef` and VIE memes are optional flavour, not part of
  this plan.

### 4.5 Names

The user asked: can there be several names to choose from? Yes, and this is how
it works.

- The `label` and `defName` are fixed. They are what the defs and the faction
  list call the faction.
- The name a world shows, such as "The Copper Rats", comes from
  `factionNameMaker`. Settlement names come from `settlementNameMaker`. Both roll
  a fresh name per world and per settlement.
- Reusing `NamerFactionTribal` and `NamerSettlementTribal` costs nothing and
  already gives many possible names per world.

**Ruled: one small pack with the five names.** A `RulePackDef` lists literal
strings, so a pack holding those five names makes the game roll one of them per
tribe.

The five, sorted by tone so the two tribes never share a name:

| Neutral tribe | Hostile tribe |
|---|---|
| `PMM_NamerFactionInsectorHive` | `PMM_NamerFactionInsectorSwarm` |
| The Abaddon Hive | Abaddon Swarm |
| Abaddon Nest | Abaddon Brood |
| Tribe of the Abaddon | |

Two packs, split three and two. A world then never holds two tribes with the same
name, and the hostile tribe always sounds like a threat. One pack holding all
five also works, at a one in five chance of a duplicate name.

**Settlement names stay vanilla.** `settlementNameMaker` = `NamerSettlementTribal`,
so villages get ordinary tribal names.

`leaderTitle` is "hive queen", confirmed.

Vanilla is the template for the pack shape: `Data/Core/Defs/RulePackDefs/
RulePacks_Namers_Factions.xml`, line 117 (`NamerFactionTribal`) and line 137
(`NamerSettlementTribal`).

### 4.6 Caravans

The user said yes, the tribes should trade, "just like Dragonia". One
correction: **Dragonia does not send traders.** There is no `caravanTraderKinds`
and no `TraderKindDef` anywhere in the Reptiles mod, and its own comment says
trade is for later. So "like Dragonia" is the neutral, befriendable part, and a
trader def is what turns it into real caravans.

It is cheap. One `TraderKindDef`, plus one line on the faction:

```xml
<caravanTraderKinds>
  <li>PMM_InsectorTribeTrader</li>
</caravanTraderKinds>
```

The trader kind needs `stockGenerators`. A fitting list, from what the tribe
lives on and from Reel's trader:

| Stock | Notes |
|---|---|
| `InsectJelly`, 50-500 | The tribe's food. |
| `VFEI2_RoyalInsectJelly`, 10-200 | Late game. |
| `VFEI2_Chitin`, 100-900 | The tribe's material. |
| `Silver`, `Jade`, `MedicineHerbal` | Normal tribal trade goods. |
| `VFEI2_Pherocore*` | **Rare.** A low count range and an expensive price, so they turn up only now and then. |

Buying: reuse the vanilla gens, for example `StockGenerator_BuyExpensiveSimple`,
`StockGenerator_BuySlaves` and `StockGenerator_BuyTradeTag`. Reel's trader def is
a ready template, at `Mods/Reel's Insector Faction/1.6/Defs/FactionDefs/
BugTribeFaction.xml`.

Only the neutral tribe trades. A permanently hostile faction does not send
caravans.

### 4.7 Two tribes: one neutral, one hostile

The user asked for two tribe bases. Vanilla already does this three ways:
`TribeCivil` (gentle), `TribeRough` (fierce) and `TribeSavage` (savage). The
Reptiles mod does it twice, from one shared house base
(`PMM_ReptileFactionBase` in `Defs/FactionDefs/Factions_Reptile.xml`): a neutral
realm and a hostile brood. Follow Reptiles.

Proposed: two `FactionDef`s under one new abstract base, `PMM_InsectorTribeBase`.

| | Neutral tribe | Hostile tribe |
|---|---|---|
| Parent | `TribeBase` | `TribeSavageBase` |
| `permanentEnemy` | no | `true`, from the parent |
| `requiredMemes` | none | `Supremacist`, Ideology only, from the parent |
| `canStageAttacks` | `true`, inherited | `true`, inherited |
| Relations | starts neutral, can befriend, raids only if relations sour | always at war, cannot be allied |
| Trades (§4.6) | yes | no |
| Name pack (§4.5) | the hive names | the swarm names |
| Village layouts | the four VFEI2 layouts | the four VFEI2 layouts |
| Castes, child kinds, trader kind | shared | shared |
| Icon and settlement texture | its own | its own, different |

What `TribeSavageBase` brings, so nobody is surprised: `permanentEnemy true`, a
`requiredMemes` entry for `Supremacist` (Ideology only, the line carries
`MayRequire`), `disallowedMemes` for Nudism and Blindsight, a red
`colorSpectrum` that we override, and `maxConfigurableAtWorldCreation 9999`. It
also means the hostile tribe can never be allied, which is the point of it.

The two need different `factionIconPath`, different `settlementTexturePath` and
different `colorSpectrum`, or the player cannot tell the villages apart on the
world map. That is the whole point of two factions.

One small decision the split creates: `PawnKindDef.defaultFactionDef` can name
only one faction. Name the neutral one, and let `pawnGroupMakers` supply the
faction everywhere else, which is how generation normally works anyway. Leave a
comment on the kind so nobody "fixes" it later.

**Ruled: both tribes fight the same way.** Same castes, same weights, same queen
weight. Only relations, trade and looks differ.

The `defName`s shipped: `PMM_InsectorHive` (label "insector hive") and
`PMM_InsectorSwarm` (label "insector swarm").

**Deviation to know about.** The hostile tribe copies the savage fields
(`permanentEnemy`, the `Supremacist` meme) instead of parenting off
`TribeSavageBase`. A second parent would have meant a second copy of the whole
shared block, and two copies drift apart. The behaviour is the same: always at
war, never an ally. Say if you want the literal parent instead.

**The queen needed a cost curve of her own.** Measured in 1.6: a settlement group
gets 1150 to 1600 points (`SymbolResolver_Settlement.DefaultPawnsPoints`), and
`MaxPawnCost` reads the faction's `maxPawnCostPerTotalPointsCurve`, which in the
vanilla tribal faction caps a single pawn at roughly 135 to 151 there. The
abaddon costs 500, so on the vanilla curve she could never have stood in a
village. The factions carry their own curve that opens the cap from 1150 points
up. Phase 3 tunes it, and no `Combat` group lists her, so raids are unaffected.

**Leader wiring, found by the field test.** `Faction.TryGenerateNewLeader` scans
only kinds that are listed in a **`Combat`** group and carry
`<factionLeader>true</factionLeader>`, plus whatever sits in
`def.fixedLeaderKinds`. A queen who lives in the `Settlement` group alone is
invisible to that scan - which is exactly why both tribes started with no leader.
The abstract base now names her in `fixedLeaderKinds`, so she leads a tribe
without ever joining a raid group. The `MayRequire` on that entry means a world
without Odyssey gets a leaderless tribe, the same way vanilla treats a faction
with no leader kind.

**Two things the field test confirmed.** Villages generate with hive walls, creep,
hives and defenders, and the real insects that hatch from a village's hives belong
to the village's faction, so they defend it instead of fighting the momos beside
them. And a 10000-point raid produced no queen, which is the intended behaviour.

### 4.8 Where the villages go

Ruling 23, 2026-09-20: the two tribes settle **polluted ground only**, so their
villages cluster on the polluted regions of the world, and a colony that pollutes
its own neighbourhood is asking for neighbours.

Implementation: `Source/Insects/FactionPlacement.cs`, a postfix on both
`TileFinder.RandomSettlementTileFor` overloads, modelled on the Reptiles
mountain-only placement. It re-rolls up to 300 times for a tile with
`Tile.pollution >= 0.1`, chaining the caller's own validator, with a reentrancy
guard so its own re-roll calls cannot recurse. `Tile.pollution` is Biotech's
world-tile pollution, 0 to 1, generated at world creation and grown during play by
polluting maps - `WorldPollutionUtility.PolluteWorldAtTile` spreads it to
neighbours out to radius 4.

Fallback, and why: if a world holds no polluted tile at all, the vanilla tile is
kept and a dev warning is logged, following the Reptiles precedent. A tribe with no
village still exists and still raids, so nothing breaks; dropping it on clean
ground would be the worse lie. To make it strict, return `PlanetTile.Invalid`
instead of keeping the result.

Open: the threshold is `0.1`. Raise it if the villages should hug only the heavily
polluted hearts, lower it if they should spread further.

---

## 5. The six castes

### 5.1 What a caste is here

VRE genelines cannot be used on NPCs (§6.1), so a caste is five layers:

1. **Race** — `Races_InsectMomo_BS.xml`. Body size, health scale, hunger, melee
   tools, temperature range. Genes cannot change these.
2. **Xenotype** — the gene package. This is our stand-in for a geneline.
3. **PawnKindDef** — race, `combatPower`, xenotype, gender, backstories.
4. **Group slot** — where the kind is listed in `pawnGroupMakers`.
5. **Role** — what she does in a village, a raid, or a caravan.

Layer 3 is what makes a caste feel different, because raid points buy pawns at
`combatPower`.

### 5.2 The six, with the numbers already fixed

| Caste | Vanilla bug | CP | Health scale | Body size | Role after the overhaul |
|---|---|---|---|---|---|
| Devil Bug | Megascarab | 40 | 0.4 | 0.2 | Swarm filler. High weight in `Combat`. Also a village civilian. |
| Giant Ant | Spelopede | 75 | 1.7 | 0.8 | Line trooper and worker. The tribe's builder. |
| Soldier Beetle | Megaspider | 150 | 2.5 | 1.2 | Heavy melee. Caste leader. |
| Greenworm | Larva | 25 | 0.25 | 0.2 | Brood only. Comes out of the queen's egg spew. |
| Vamp Mosquito | Locust | 55 | 0.7 | 0.6 | Fast flyer. Ranged slot only if she has an ability. |
| Abaddon | Hive Queen | 500 | 9.8 | 4.5 | The queen. `Settlement` and `leaders`. The reason to raid a village. |

These numbers come from `PLAN.md` §3 and are unchanged. Keep `combatPower` at
the vanilla bug's value: infestation and raid point budgets were tuned for it.

### 5.3 The VRE gene package per caste (write-up for review, 2026-09-20)

Each caste gets a small, themed set of VRE Insector genes, added straight to her
xenotype. These are `VanillaRacesExpandedInsector.GenelineGeneDef` defs, which
are plain `GeneDef`s, so a xenotype can list them like any other gene.

Written up on 2026-09-20 from the 1.6 files rather than from memory, and **not yet
written to XML**. It replaces the first draft's table, which gave the mosquito wings
she already has, the beetle claws her race already has, and the bug sizes that B&S
already owns.

**Built 2026-09-20, in `Defs/XenotypeDefs/Xenotypes_Insect.xml`.** A `*` marks a
pherocore-locked gene: the caste is born with it, and recruiting her is what
unlocks it for the player. Nothing duplicates a race def.

| Caste | Evolutions | Degrades | Reads as |
|---|---|---|---|
| Devil Bug (40) | `VRE_AntimicrobialPeptides`, `VRE_InfraredSensors`* | `VRE_Heatstress`, `VRE_RapidLifeCycle` | Mends fast and hunts by heat in the dark, but burns in the sun and burns out young |
| Giant Ant (75) | `VRE_Hiveglands`, `VRE_SpelopedeHorn` | `VRE_Dormant`, `VRE_LowGreyMatter` | Builds and digs, sleeps hard, does not think |
| Soldier Beetle (150) | `VRE_HardenedChitin`, `VRE_CuticleShell` | `VRE_InefficientMidgut`, `VRE_Stenothermic` | Armoured and unplagued, but she eats like a horse and only thrives at home |
| Greenworm (25) | `VRE_Hiveglands`, `VRE_RobustMidgut` | `VRE_Immunodeficiency`, `VRE_HypothermicHibernation` | Builds, eats anything, catches everything, and sleeps through the cold |
| Vamp Mosquito (55) | `VRE_HighGreyMatter`, `VRE_AntimicrobialPeptides` | `VRE_AcidBlood`, `VRE_WeakenedChitin` | Cunning and fast-mending, thin-shelled, and her blood smokes when cut |
| Abaddon (500) | `VRE_SwarmSynapse`*, `VRE_SpawningSack`, `VRE_SlowedLifeCycle` | `VRE_ProteinDenaturation`, `VRE_HypothermicHibernation`, `VRE_HardLockedJoints` | A queen who never ages or stops breeding, cannot face the sun, sleeps through cold, and waddles |

**What the user changed in the vanilla genes** (2026-09-20): hands and heads instead
of muscle. The Giant Ant and the Greenworm both lost a vanilla gene for
`AptitudeRemarkable_Construction` (the ant lost `MeleeDamage_Strong`, the greenworm
`MoveSpeed_Slow`), and the Abaddon traded `Robust` and `MeleeDamage_Strong` for
`AptitudeRemarkable_Social` and `AptitudeStrong_Intellectual`.

Aptitude genetics are a `GeneTemplateDef`
(`Biotech/Defs/GeneDefs/GeneTemplateDefs.xml`), so the def names are generated per
skill and **the tier names are not the labels**:

| defName | label | effect |
|---|---|---|
| `AptitudeTerrible_X` | awful X | -8, drops all passion |
| `AptitudePoor_X` | poor X | -4 |
| `AptitudeStrong_X` | strong X | +4 |
| `AptitudeRemarkable_X` | great X | +8 and a passion level |

So "great" is `AptitudeRemarkable_*` and "strong" is `AptitudeStrong_*`, one tier
apart. The Abaddon's intellect was first written as `Remarkable` by mistake; it is
`Strong` now, and the two construction genes were right from the start. An aptitude
is neither an evolution nor a degrade, so it sits outside the balance count, and
each carries an `Aptitude<skill>` exclusion tag, so one pawn can never hold two
aptitudes for the same skill.

**Balance, the Insector way.** Every caste holds as many degrades as evolutions -
two and two, and four and four for the queen. That is VRE's own rule for a
geneline, in its language file (`Languages/English/Keyed/Keys.xml`):
`VRE_NeedEqualAmount`, "Needs equal amount of evolutions and degrades." Nothing in
the engine enforces it on a xenotype, so it is a house rule we keep by hand, and
it is why each caste above has its cost listed next to its gift.
`VRE_InsectJellyDependency` is neither an evolution nor a degrade, so it sits
outside the count.

On top of those, the fighters carry the jelly dependency, and everything else a momo
is comes free with the insectoid gene they all already carry:

| Added to all six | Why |
|---|---|
| `VRE_InsectJellyDependency`, on the five fighters only | The jelly loop. Never on the greenworm (§6.2). |
| The pheromone effect, as no gene at all | `PMM_Gene_Insect` carries it now. See below. |

**The pheromones moved into our gene** (user, 2026-09-20). VRE's
`VRE_InsectPheromones` is only a flag: the behaviour lives in two Harmony patches
in their assembly, which check `HasActiveGene(VRE_InsectPheromones)` and then tell
the insect that the pawn is not a valid target - a postfix on the compiler
generated validator nested inside `AttackTargetFinder.BestAttackTarget`, and one on
`Pawn.ThreatDisabledBecauseNonAggressiveRoamer`. So a momo used to need two genes to
say one thing. Our own copy lives in `Source/Insects/InsectPheromones.cs`, gated on
`PMM_Gene_Insect`, and VRE's gene is gone from all six castes. The price of owning
it: if VRE ever changes how their pheromones work, our copy will not follow. Both
copies can run at once safely - they act on disjoint gene sets.

**Insect flesh, ruled 2026-09-20.** `PMM_Gene_Insect` now also carries a VEF
`GeneExtension`:

```xml
<modExtensions>
    <li Class="VEF.Genes.GeneExtension">
        <customMeatThingDef>Meat_Megaspider</customMeatThingDef>
        <defsTreatedAsHumanMeat>
            <li>Meat_Megaspider</li>
        </defsTreatedAsHumanMeat>
    </li>
</modExtensions>
```

So her butchered meat is insect meat - which the races already said with
`useMeatFrom Megaspider` - and for **her** that meat counts as human meat. Eating
insect meat, her own kind included, is cannibalism to a momo and ordinary food to
everyone else. This is the eating half of the human-leather analogy. There is no
wearing half: nothing in vanilla, VFEI2 or VRE reacts to chitin apparel
(`ThoughtWorker_HumanLeatherApparel` compares worn stuff against
`ThingDefOf.Human.race.leatherDef` and nothing else), and real insectoids are
animals with no mood to react with.

VRE ships the same effect as `VRE_InsectFlesh`, which also carries VFEI2 terrain
speed factors (Insect 2, Creep 4.75, JellyFloor 5, RoyalJellyFloor 10). We took the
flesh part only, because the races already set the Insect 2 factor and a second copy
could stack. One oddity worth knowing: `Meat_Megaspider` is defined in **no XML at
all** - vanilla's own recipes reference it, and so does VRE's gene - so 1.6 must
generate per-race meats from `race.meatLabel`, naming them after the race. The name
is therefore borrowed, not invented, and it is the same name the vanilla recipes
list.

Two effects are worth seeing before agreeing to them. `VRE_HardenedChitin` is
`IncomingDamageFactor 0.75` on **all** incoming damage, and it sits on top of the
armour the B&S race tracker already gives the beetle - test her before keeping it.
`VRE_Hiveglands` is what lets a momo raise VFEI2 hives at all (§7.1), so the ant
and the greenworm become the tribe's two builders.

**The catalogue.** Read out of the 1.6 load folder on 2026-09-20: 22 free
evolutions (`1.6/Defs/GeneDefs/GeneDefs_Evolutions.xml`), 18 free degrades
(`GeneDefs_Mutations.xml`), 22 free cosmetics (`GeneDefs_GenelineCosmetics.xml`
plus one in the `Mods/AlphaAnimals` folder), and 20 pherocore-locked genes - 10
evolutions and 10 degrades in `Mods/VFEInsectoids/Defs/GeneDefs/*_VFEInsectoids.xml`.
The rest of the folder is the base xenotype's own parts: body, the four skin
tones, blood, flesh, antennae, pheromones, jelly dependency, chestburst and the
geneline gizmo gene.

**Four rules cut the list down.**

1. **One gene per organ tag.** VRE uses `exclusionTags` so two versions of the
   same organ cannot stack: `InsectorEye`, `InsectorVocal`, `InsectorImmunity`,
   `InsectorMovement`, `InsectorMouth`, `InsectorStomach`, `InsectorBrain`,
   `InsectorArmor`, `InsectorToxic`, `InsectorGlands`, `InsectorSize`,
   `InsectorLifespan`, `InsectorHeatstroke`, `InsectorHypothermia`,
   `InsectorAggression`, `Hands`, `Headbone`, `Tail`, `Fur`/`Body`, `BloodType`,
   `Fertility`, `Immunity`, plus the word pairs `MinTemperature`/`MaxTemperature`,
   `UVSensitivity` and `KillThirst`. Two genes sharing a tag cannot sit on one
   pawn, so a package may hold only one of each pair - and this is what makes the
   pairs useful to pick against: `VRE_RobustMidgut` and `VRE_InefficientMidgut`
   both hold `InsectorStomach`, so a caste can never have both.
2. **Nothing the race def already does.** The races carry chitin armour through
   the B&S tracker, teeth and claws, insect flesh, meat and blood, temperature
   range, body size, and wings on three of the six castes. Out for that reason:
   `VRE_InsectSkin`, `VRE_InsectFlesh`, `VRE_BugBlood`, `VRE_MineralRichInsectskin`
   (armour would stack again - the bug that was already burned once at 1.44 sharp),
   `VRE_Microsized` and `VRE_Colossal` (a `VEF_BodySize_Offset` on top of B&S
   sizing), and the melee-weapon genes `VRE_RipperBlades`, `VRE_ChargerClaws`,
   `VRE_MegaspiderHorns`, `VRE_InsectMandibles` and `VRE_InsectRostrum` (an extra
   attack verb, and the blades and claws also cost work speed, -0.3 and -0.4).
   `VRE_SpelopedeHorn` is the one exception, kept on the Giant Ant for its +0.2
   mining.
   **The wing genes are the sharp edge of this rule.** Our own `PMM_Gene_Flight`
   and `PMM_Gene_FlightWeak` (core `Defs/GeneDefs.xml`) grant a leap ability *and*
   swap the body to B&S's winged body. `VRE_InsectWings` and `VRE_LocustWings`
   would sit on top of that: two wing systems on one body. Our chitin skin genes
   carry no exclusion tags at all, so VRE's `SkinColorOverride` cosmetics would not
   even be blocked - they would simply fight ours for the final colour.
3. **Melee only.** No aimed ranged ability on a fighting caste, so
   `VRE_FlameGlands` and `VRE_AcidGlands` are out even though they are two genes.
   A reactive burst is not an aimed attack, so the queen could still take
   `VRE_AcidBurstSack` if it is ever unlocked for her.
4. **Locked genes are allowed on castes** (user, 2026-09-20). Those 20 genes are
   VFEI2's boss reward, and a caste carrying one hands the player that reward on
   recruitment. The user is fine with that, so two are in the packages:
   `VRE_InfraredSensors` (the devil bug's heat sense) and `VRE_SwarmSynapse` (the
   queen's crown).

**The pollution immunity: built, on the gene we already had.** The ruling is full
immunity for all six castes. Checked in the 1.6 assembly: `immuneToToxGasExposure`
is a field on `ApparelProperties` and on `GeneDef`, **not on `ThingDef`** - so a
race def cannot grant it; only a gene can. The user's answer (2026-09-20) was to
put it on `PMM_Gene_Insect`, the insectoid gene every momo already carries
(`Defs/GeneDefs/Genes_Insect.xml`), instead of adding a second gene:

```xml
<immuneToToxGasExposure>true</immuneToToxGasExposure>
<statOffsets>
        <ToxicEnvironmentResistance>1</ToxicEnvironmentResistance>
</statOffsets>
```

One edit covers all six castes, and it also covers a woman transformed into a momo
at runtime, since that path grants the same gene. Vanilla's own
`ToxicEnvironmentResistance_Total` ("total antitoxic lungs",
`Data/Biotech/Defs/GeneDefs/GeneDefs_Health.xml` line 172) was the alternative: the
same two fields plus rot-stink immunity, `biostatCpx 2`, `biostatMet -3`. It would
have been a second gene saying the same thing. The races keep their own
`ToxicEnvironmentResistance 0.8`, because buildup is severity x (1 - resistance),
so 0.8 plus 1 is already past the point where buildup stops.

Knock-on: `VRE_VestigialTubules` (the toxic degrade) and `VRE_DetoxifierTubules` are
now no-ops on a momo, so neither is in any package, and the Devil Bug no longer
needs `VRE_DetoxifierTubules` as "her extra". `biostatMet` on the insectoid gene is
still 1; raise it if the immunity should ever cost her hunger.

**Decided by the user, 2026-09-20.**

2. **Immunity mechanism: done, on `PMM_Gene_Insect`** (above).
4. **`VRE_Hiveglands` goes on the Giant Ant *and* the Greenworm**, so two castes
   can raise VFEI2 hives (§7.1). The other four cannot.
5. **No `VRE_AcidGlands` on the greenworm.** She keeps `SludgeSpew` alone.
6. **No `VRE_PollutionDependency` anywhere.** Struck from the queen's package.

**Decided too, 2026-09-20.**

1. **Locked pherocore genes on the castes: allowed.** The castes carry two of
   them - see rule 4 above. Detail of what the flag actually does:
   evolutions and 10 degrades, four per VFEI2 hive - Sorne (`VRE_SwarmSynapse`,
   `VRE_RoyalJellyInjector`), Nuchadus (`VRE_PyroResistantChitin`,
   `VRE_FlameGlands`), Chelis (`VRE_LocustWings`, `VRE_InsectRostrum`), Kemia
   (`VRE_AcidGlands`, `VRE_InfraredSensors`), Xanides
   (`VRE_MineralRichInsectskin`, `VRE_ChargerClaws`) - plus the Alpha Animals and
   Vanilla Genetics Expanded groups the mod does not use. They are marked
   `unlockable`, and `Utils.GenelineGenesInOrder` filters them out of the geneline
   picker until `WorldComponent_UnlockedGenes` has them, which the player does by
   consuming pherocores that VFEI2 bosses drop. The flag only hides them from the
   picker, so a caste carrying one has it from birth and the player collects it free
   on recruitment - one fewer reason to hunt that hive's boss. Flavour agrees with
   the mechanic: a pherocore gene belongs to one hive's bloodline, and the momos are
   a people of their own, not Sorne or Xanides.
   Three options: keep all 20 for the player (recommended); let the *queen* carry a
   couple, since she is the 500-point boss of her own hive; or let the castes use
   the 10 locked *degrades* only, since their downsides (`VRE_Colossal`,
   `VRE_EcdysoneOverdrive`, `VRE_InsectVolatile`...) are not rewards the player
   would miss.
2. **`VRE_GenelineEvolution`: left off** (user, 2026-09-20). The packages are
   permanent unless it is reconsidered later. Detail: the gene is `biostatCpx 3`,
   `biostatMet -5`, and does *nothing* on an NPC, because
   `Gene_GenelineEvolution.GetGizmos` returns early unless
   `pawn.IsColonistPlayerControlled`. On a recruited momo it opens
   `Window_ManageGenelines`: choose up to four genes from the unlocked pool, she
   sits in a `VRE_Metapod` for 24 hours, then the new loadout lands. The catch is
   `Geneline.AddPawnDirectly`, which removes **every** gene on the pawn whose def
   `is GenelineGeneDef` and which is not in the new loadout - and the whole caste
   package is made of `GenelineGeneDef`s. So her first switch deletes the package
   she was born with (her `PMM_Gene_Insect`, `ProjectMomo_Momo`, skin genes and
   vanilla genes survive, because those are plain `GeneDef`s). That is either the
   point of VRE's system or a trap, depending on taste. Without the gene she can
   still be customised the vanilla way, with gene packs and an assembler, but never
   through a geneline. Left off, and it is one line to add if that is ever wanted.

Two picks depend on rule 4: `VRE_InfraredSensors` (the devil bug's heat sense) and
`VRE_SwarmSynapse` (the queen's crown). If locked genes are ever taken back off the
castes, the free stand-ins that pass the tag check are `VRE_CompoundEyes`,
`VRE_HighGreyMatter` and `VRE_OcelliEyes`.

**What the packages add up to.** The fighters carry `VRE_InsectJellyDependency`
(`biostatMet 3`) on top of the momo gene's 3 and the insectoid gene's 1. Metabolism
raises food need, so the jelly gene makes the fighters eat more than they used to.
If the tribes turn out to eat the map, lower the insectoid gene or the jelly gene
before trimming the caste genes.

**Still binding, from the first draft.** Packages stay balanced between evolutes and
degrades, so the Health tab reads cleanly. VRE's `disableGeneExtraction` stays as
shipped, so a caste gene cannot be pulled out of a momo. The xenotypes stay
`inheritable`, so daughters inherit the caste. Jelly on the fighters only, never
`VRE_JellySacks` (§6.2). Pheromones come from `PMM_Gene_Insect` rather than from a
VRE gene, and that copy is ours to maintain. The two spew abilities the mod already
has stay as they are: `SludgeSpew` on the greenworm and the `EggSpew` clone on the
abaddon (§5.4).

### 5.4 The brood (egg spew) — built 2026-09-20

**Built, awaiting its field test. The brood is a VFEI2 swarmling, not a momo** (the
user's call, 2026-09-20). The clone is `PMM_Ability_EggSpew`, `PMM_Proj_EggSac`
and `PMM_EggSac` (`Defs/AbilityDefs/Abilities_EggSpew.xml` and
`Defs/ThingDefs/Things_EggSpew.xml`), all `MayRequire` Odyssey, and the abaddon
race comp and the queen pawn kind now point at it. All XML. What shipped against
this plan:

- The hatch is Core's own `CompSpawnPawnOnDestroyed` with `CompSpawnLarva` - which
  only sets `JoinLord => parent.Faction != Faction.OfPlayer` - exactly as vanilla's
  sac does: `KillFinalize` only, generated at age 0, a five-cell hop on a
  `PawnFlyer_Stun`, then `lordJob LordJob_WanderNest`.
- `VFEI2_Swarmling` is VFEI2's own juvenile kind, the pawn their queen and empress
  spawn. Its own `CompProperties_SwarmlingToCocoon` turns her into a cocoon, and a
  random adult insectoid of the hive crawls out. So the sac feeds the hive back
  into itself, and the brood's species stays a surprise.
- Why not a momo: Core cannot generate a HUMANLIKE pawn there at all. It asks for
  `fixedBiologicalAge 0` while refusing downed pawns, and a newborn humanlike is
  always downed, so the generator gave up after 120 tries, returned null, and the
  next line threw a NullReferenceException through every other mod's
  `GeneratePawn` postfix (in `Player.log` for 2026-09-20). A temporary
  `Source/Insects/EggSacHatch.cs` generated her as an adult instead; it was deleted
  when the brood became a swarmling, which is not humanlike.
- The faction question is answered without code. `Projectile_SpawnsThing.Impact`
  ends with `if (thing.def.CanHaveFaction) thing.SetFaction(base.Launcher.Faction)`
  and `ThingDef.CanHaveFaction` is true for buildings, so the sac takes the
  caster's faction and `CompSpawnPawnOnDestroyed` spawns the pawn with the sac's
  faction. `category` must stay `Building` for that.
- Odyssey's egg-sac art is not a loose file, so it cannot be reused. Ours is
  generated by `Tools/mktex.py` (building, projectile and ability icon) and is a
  placeholder like the rest of the mod's art.

Work item, and it must not be missed. Pencilled in before the build: the Abaddon
carried
`CompProperties_SpreadSludge` with `abilityDef EggSpew`, and the egg sac it
throws spawns a vanilla `Larva`. The old swap turned that larva into a
greenworm. With the swap gone, it spawns a real larva again.

The Odyssey defs are the template, and this is pure XML:

- `Data/Odyssey/Defs/AbilityDefs/Abilities.xml` — `AbilityDef EggSpew` launches
  `Proj_EggSac`; `ThingDef Proj_EggSac` is a `Projectile_SpawnsThing` with
  `spawnsThingDef EggSac`.
- `Data/Odyssey/Defs/ThingDefs_Buildings/Buildings_Misc.xml` — `ThingDef
  EggSac` has `CompProperties_SpawnPawnOnDestroyed` with `pawnKind Larva`.

So: clone the chain as `PMM_Ability_EggSpew`, `PMM_Proj_EggSac` and
`PMM_EggSac`, with `pawnKind VFEI2_Swarmling`, and give the clone to the
Abaddon's race comp. Check two things while cloning:

- `compClass CompSpawnLarva` is an Odyssey class for larvae. Drop the override
  and let `CompProperties_SpawnPawnOnDestroyed` use its default comp.
- `lordJob LordJob_WanderNest` may not suit a humanlike pawn. Either drop it, or
  test it, or point it at a job the brood can follow.

Decide which faction the brood belongs to. A brood spawned next to the queen
should belong to the queen's faction, so the village can use her. Test this in a
dev-spawned village before shipping.

### 5.5 Children — dropped by the user (2026-09-20)

Built as this section planned, then removed the same day: the user does not want
children in these villages. What was built, for the record: six child kinds
(`PMM_InsectChildDevilBug` and on) in `Defs/PawnKindDefs/PawnKinds_InsectChild.xml`,
listed in the `Settlement` and `Peaceful` groups, plus
`Source/Insects/ChildStagePatch.cs` to stop the engine re-ageing them. All of it,
and its changelog lines, is gone - a village is adults only again.

The research is kept because it cost an afternoon and is all still true: §12.8
explains how a village gets its people, why `backstoryFilters` do nothing for a
child, and the one vanilla rule that turns children into adults in a faction at
war. Read that before starting this again.

If children ever come back: Biotech's shape is `Villager_Child` / `Tribal_Child` -
`pawnGroupDevelopmentStage` Child, `isFighter` false, no gear - plus
`fixedChildBackstories` so the child gets a hive story instead of a human one.

---

## 6. VRE Insector wiring — what works, what does not

### 6.1 Genelines cannot be put on NPC pawns

`Geneline` is not a def. It is a save-game object in `GameComponent_Genelines`.
Nothing ships in XML. Switching geneline runs through a gizmo, and the gizmo
returns early unless `pawn.IsColonistPlayerControlled`:

```csharp
// Gene_GenelineEvolution.GetGizmos()
if (!pawn.IsColonistPlayerControlled) { yield break; }
```

So a raider, a villager or a caravan guard can never wear a geneline. §5.3 is the
workaround: put the same genes on the xenotype directly.

Also verified: `Pawn_GeneTracker_CheckForOverrides_Patch` only re-checks
exclusion tags between `GenelineGeneDef`s. It does not require the pawn to carry
`VRE_GenelineEvolution`. So a flat package is safe.

### 6.2 Jelly dependency

`VRE_InsectJellyDependency` is a gene resource bar, not a vanilla need.

- Empty bar drains 0.2 per day.
- One `InsectJelly` eaten restores 0.5.
- A hidden hediff adds 1 severity per day while the bar is empty. Deficiency at
  5, coma at 30, death at 60.
- The job that makes pawns eat jelly is patched into the **colonist** think tree
  only. An NPC momo never seeks jelly. Raids are short, so this only matters for
  long-lived non-player pawns. Off-map villages are not simulated.

**Ruled: the combat castes only.** Carry it on Devil Bug, Giant Ant, Soldier
Beetle, Vamp Mosquito and Abaddon. Leave it off the greenworm, who is brood, not
a fighter.

**Ruled: no `VRE_JellySacks` on any momo.** Pawns never make jelly. Every drop
comes from the world or from buildings the player raises, which is what makes the
VFEI2 jelly economy matter.

**Ruled: keep.** The Abaddon carries it. She is a combat caste at 500 points, and
royal jelly is her flavour.

### 6.3 What we do not take

- `VRE_ChestburstPregnancy` — ruled out. It is player-gizmo only, but the victim
  hediff kills in 15 days, and the user said no.
- `VRE_SpawningSack` is fine and stays in the queen's package. It only interacts
  with chestburst when chestburst is present.

---

## 7. VFEI2 wiring — hives and jelly

### 7.1 The build permission, and why it matters

VFEI2 lets a pawn build its hive structures only if the def carries
`VFEInsectoids.InsectBuilding` and either the pawn is a real insectoid or she
carries an active gene named exactly `VRE_Hiveglands`:

```csharp
// VFEInsectoids.GenConstruct_CanConstruct_Patch
if (!modExtension.nonInsectCanBuildIt && !p.IsColonyInsect() &&
    (p.genes == null || !p.genes.GenesListForReading.Any(g => g.def.defName == "VRE_Hiveglands" && g.Active)))
{ __result = false; }
```

And `Utils.IsColonyInsect()` needs `pawn.RaceProps.Insect`, which a humanlike
momo can never pass.

**So `VRE_Hiveglands` is the only key that lets a momo build a hive.** It is a
string compare on the gene name, not a race or faction check. That is why the
Giant Ant carries it in §5.3. With it she can raise `VFEI_HiveWall`,
`VFEI2_InsectJellyWall`, `VFEI2_JellyFarm`, `VFEI2_Creeper` and the artificial
hives. Without it she cannot build any of them.

The same gene gives +50% ConstructionSpeed and drops filth while she builds.
Both fit the species.

### 7.2 The jelly loop

Research: `VFEI2_BasicHivetech` → `VFEI2_StandardHivetech` → `VFEI2_ExoticHivetech`.

Producers a momo colony can build, once the `VRE_Hiveglands` gate is passed:

| Building | Output |
|---|---|
| `VFEI2_JellyFarm` | `InsectJelly` 6 per 45000-65000 ticks (settings raise it) |
| `VFEI2_Creeper` | `InsectJelly` 10 per 150000 ticks, and it spreads `VFEI2_Creep` |
| `VFEI2_ArtificialBasicHive` | `InsectJelly` 5, and it spawns insects |
| `VFEI2_Artificial*Hive` (geneline) | `InsectJelly` 20 |
| `VFEI2_JellyMorpher` | `VFEI2_RoyalInsectJelly` 10, fuelled by jelly |

Loot: NPC hives leave 30 jelly, the walls leave 2 each.

Nothing on the momo side produces jelly, by ruling 10. These buildings are the
whole supply for a colony that keeps insect momos.

### 7.3 Two small code changes

- `HiveNourishment.cs` — **done 2026-09-20.** It now resolves a list of hive-like
  defs once and looks each one up, instead of caching `ThingDef.Named("Hive")`.
  The list takes every def whose `thingClass` is a `Hive` (the same test VFEI2's
  own `Utils.allHiveDefs` uses), plus the `ArtificialHive` class family, which is
  a `ThingWithComps` and therefore not a `Hive`. Both halves avoid a compile-time
  reference on VFEI2.

- `HiveNourishment.cs` looks for `ThingDef.Named("Hive")` only. That misses
  `VFEI2_KemianHive`, `VFEI2_ChelisHive`, `VFEI2_XanidesHive`,
  `VFEI2_NuchadusHive` and every artificial hive. Widen it to any def whose
  `thingClass` is `Hive` (the same test VFEI2's own `Utils.allHiveDefs` uses).
  Two lines, and it makes hive nourishment work in every VFEI2 hive.
- The pawn kinds carry `moveSpeedFactorByTerrainTag` with `Insect 2.0`. VRE's own
  insect skin gene adds `VFEI2_Creep 4.75`, `VFEI2_JellyFloor 5` and
  `VFEI2_RoyalJellyFloor 10`. Our villages stand on creep, so adding those three
  entries to the kind gives momos their home-field speed. Optional, but cheap.

---

## 8. Kidnapping and the bond rule

Two things stay separate. Do not merge them.

**1. Vanilla raid kidnapping.** Their raids are humanlike raids, so raiders can
kidnap downed colonists. The man is then an ordinary prisoner of the tribe, and
the village he is held in can be raided to free him. That is the whole reason the
user allowed kidnapping again. So:

- Keep `LordJob_Kidnap` and the vanilla job.
- Delete the vanish behaviour in `HiveKidnapVanish.cs`.
- Delete the custom letters. Vanilla letters already say he is a prisoner.
- Leave `IncidentParms.canKidnap` and `LordJob_AssaultColony.canKidnap` alone.

**2. The bond rule.** When a hive momo wins a tsugai bond, she never joins the
colony. She carries the man home. This stays exactly as it is today, with two
edits:

- The faction test in `HiveInsectMomo.IsHiveInsectMomo` moves from
  `Faction.OfInsects` to the new tribe. Today it silently stops matching the
  moment the momos change faction.
- The custom "lost for good, no ransom, no rescue" text goes. He is a prisoner
  in a village, which the player can attack.

---

## 9. Dependencies, load order, build

`About/About.xml`, hard dependencies. **Shipped 2026-09-20** - they were overdue,
because the faction defs already referenced `KCSG.*` types (inside VEF) and
VFEI2's settlement layouts:

| packageId | Display name |
|---|---|
| `vanillaracesexpanded.insector` | Vanilla Races Expanded - Insector |
| `oskarpotocki.vfe.insectoid2` | Vanilla Factions Expanded - Insectoids 2 |
| `OskarPotocki.VanillaFactionsExpanded.Core` | Vanilla Expanded Framework |

VEF must be listed even though VFEI2 pulls it in, because our XML uses `KCSG.*`
types (shipped inside VEF) and `VEF.Genes.*` extensions. Listing it makes the
load order explicit.

`loadAfter` order: `brrainz.harmony`, `Ludeon.RimWorld.Biotech`, `PMM.Core`,
`RedMattis.BetterPrerequisites`, `OskarPotocki.VanillaFactionsExpanded.Core`,
`vanillaracesexpanded.insector`, then `oskarpotocki.vfe.insectoid2`. VRE Insector
loads after VFEI2, so mirror that.

The description needs a rewrite. It is player-facing prose, so simple English.
It currently promises that every spawn source is affected and that wild
encounters exist. Both are now false.

Build steps are unchanged: `./build.sh`, then `./sync.sh` before any in-game
test. The installed copy under `Mods` is a separate tree.

---

## 10. Work order and test gates

Build in this order. Each phase ends with a boot test, zero red errors, and a
changelog line.

**Phase 1 — delete the old spawn layer. (done 2026-09-20)**
Removed the swap, the wild kinds and the wild think trees. Expect real bugs
everywhere.
Gate: an infestation, a hive, an ancient danger and a VFEI2 geneline raid all
produce real insects. No momo spawns anywhere. The age workaround is gone with
the swap, so check that no "generated downed pawn" spam remains.

**Phase 2 — the two factions, their names and their villages. (done 2026-09-20)**
Shipped as `Defs/FactionDefs/Factions_InsectorTribes.xml` (one abstract base plus
the two factions), `Defs/RulePackDefs/Namers_Insector.xml` (two name packs) and
the renamed `Defs/PawnKindDefs/PawnKinds_InsectorTribe.xml`. The hostile tribe
copies the savage fields rather than parenting off `TribeSavageBase`, and the
cost curve was raised so the queen can stand in a village at all (§4.7).

Field-tested 2026-09-20: names, villages, defenders and the friendly hive insects
all pass, and a big raid correctly produced no queen. The one gap the test found
was the missing faction leader, now fixed (§4.7).

To inspect a village garrison: a settlement map only exists once you attack the
village (DevMode plus god mode makes that cheap). There is no dev tool for a
`Settlement` group - the "pawn groups made" debug menu previews `Combat` groups
only - so the only place to see the queen is a real village map.
Gate: a new world has at least one insector village, the two tribes carry
different Abaddon names, and they are tellable apart on the world map. Each
village map has hive walls, creep and hives, with defenders standing in it. Check
the log for `Skipping AddHostilePawnGroup` — that means a `Settlement` group is
missing.

**Phase 3 — raids, children and the caste table. (done 2026-09-20; the children
were dropped, and the rest is not yet field-tested)**
Done: `pawnGroupMakers` filled with melee groups only - Combat, a second Combat
for the Odyssey roster, Combat-melee, Peaceful, Settlement - plus the four work
groups vanilla tribes carry. The children were built and then removed by the user
(§5.5). The queen is still `Settlement`-only and still weight 1, which §11 item 1
asks about.
Gate: a large raid fields swarms of devil bugs, a few giant ants and at least one
soldier beetle, and no ranged group is ever used. A small raid never fields the
queen. A village raid fields her.

**Phase 4 — gene packages. (built 2026-09-20; not yet field-tested)**
Done: the six caste packages are in the six xenotypes (§5.3),
`VRE_InsectJellyDependency` on the five fighters only, no `VRE_JellySacks`
anywhere, and no `VRE_GenelineEvolution`. The pheromone effect is ours now, carried
by `PMM_Gene_Insect` instead of by VRE's gene.
Full environmental pollution immunity was ruled 2026-09-20 and is **done** too: it
lives on `PMM_Gene_Insect` (`Defs/GeneDefs/Genes_Insect.xml`), the gene all six
castes already carry, because `immuneToToxGasExposure` is a `GeneDef` field and no
race def can hold it.
Gate: each caste shows the expected genes in the Health tab. No gene conflict
errors. Real insects ignore a momo while she is among them. A momo with an empty
jelly bar picks up the deficiency hediff, and only the fighters do. No toxic
buildup builds up in tox gas, on polluted ground, or in toxic fallout.

**Extras, decided in the same conversation and built 2026-09-20.** Three things came
out of the Phase 4 discussion. None was in the original phase list:

- **Chitin, not human leather.** Every momo race is `ParentName="Human"`, which
  carries `leatherDef Leather_Human` and a `LeatherAmount` of 75, so a butchered momo
  used to yield human leather. All six races now set `VFEI2_Chitin` with a
  `LeatherAmount` of 30, matching VFEI2's own insect races
  (`Races_Fuelmite.xml`, `Races_Megawasp.xml`, `Races_RoyalMegascarab.xml`).
  Meat and blood were already right through `useMeatFrom Megaspider` and
  `bloodDef Filth_BloodInsect`.
- **Insect flesh.** `PMM_Gene_Insect` carries a VEF `GeneExtension`
  (`customMeatThingDef` + `defsTreatedAsHumanMeat`) so insect meat counts as human
  meat to a momo: eating it is cannibalism to her and ordinary food to everyone
  else. The eating half of the human-leather analogy; the wearing half does not
  exist in any mod (§5.3).
- **One corpse line.** Six momo corpses now sit under a single "momo corpses" line
  in the butcher menu and every item filter, instead of six entries mixed into
  humanlike corpses. Three caches, one snapshot and Big and Small had to be dealt
  with, all of it written up in §12.9.

Gate: butchering a momo yields 30 chitin; feeding insect meat to a momo colonist
raises the cannibalism thoughts and doing the same to a baseliner does not; the item
filters show one "momo corpses" line holding all six races, and a default butcher
bill still accepts a momo corpse.

**Phase 5 — brood and nourishment. (built 2026-09-20, awaiting its field test)**
The egg spew clone, and the widened hive lookup.
Gate: the Abaddon's egg spew produces a swarmling, in the queen's faction, in a
dev-spawned village. A momo standing near a `VFEI2_KemianHive` gets nourishment.

**Phase 6 — jelly and hives for the player.**
Confirm the build gate from §7.1 in both directions.
Gate: a recruited momo with `VRE_Hiveglands` can build `VFEI2_JellyFarm` and
`VFEI2_HiveWall`. A momo without the gene cannot.

**Phase 7 — kidnapping and polish.**
Bond rule on the new faction, letters removed, vanish removed, faction icon,
description, no new namers yet.
Gate: a bond win ends with the man as a prisoner in a village, and the player can
see him there. No "lost forever" letter anywhere.

**Phase 8 — trade and optional extras.**
The caravan trader kind (§4.6) on the neutral tribe only, and a custom Ideology
culture if it is ever wanted. An own settlement layout only if VFEI2's four ever
feel samey.

**Test every phase with a fresh world.** Save compatibility is already broken by
decision, so do not spend time on migration.

---

## 11. Open decisions

Every design question is answered. They are all in §2, and the naming items from
the first draft shipped: `PMM_InsectorHive` and `PMM_InsectorSwarm`, three hive
names to the neutral tribe and two swarm names to the hostile one.

Left:

1. **Queen presence in villages.** She is affordable at village point levels and
   listed in the `Settlement` group at weight 1 of 33, so she is a chance per
   village rather than a guarantee. Phase 3 kept her at 1 - raise the weight if
   the user wants a queen more often. She cannot be seen without attacking a
   village (§10, Phase 2).
2. **Custom art.** The hostile tribe has its own crest now, used for both its icon
   and its villages (§4.4). The neutral tribe still uses VFEI2's hive art and the
   vanilla insect icon.
3. **Phase 3 tuning:** the raid weights, the queen's weight (§11 item 1), and
   whether the raised cost curve changes how raids feel.
4. **The Phase 4 gene write-up** (§5.3) is fully answered and built: immunity on
   the insectoid gene, the six caste packages, locked genes allowed (two in use),
   and `VRE_GenelineEvolution` left off. What is left is the field test.
5. **One momo-corpse line across the whole family: done 2026-09-20.** The category
   def and the mover live in `Project Momo` (`Source/ProjectMomo/MomoCorpses.cs`) and
   `Defs/ThingCategoryDefs/ThingCategories_MomoCorpses.xml`, and each species mod
   registers its own races: insects 6, slimes 6, elementals 8 (§12.9). Reptiles cannot
   join as they are - their pawns are vanilla Human with a xenotype, so their corpses
   are shared human corpses - and giving the 11 reptile species their own race defs is
   planned as a separate project (`Project Momo Reptiles/RACES-PLAN.md`).

Worth asking later, not now:

6. Do we want our own settlement layout on top of VFEI2's four?
7. Do we want a custom Ideology culture, and VIE memes with it?
8. A settlement namer of our own, instead of `NamerSettlementTribal`.

---

## 12. Evidence appendix

Everything here was read from the installed mods or decompiled from 1.6
assemblies on 2026-09-20. Copy of the notes: repo memory
`insector-faction-research.md`.

### 12.1 KCSG, and why a faction base can be made of VFEI2 walls

`KCSG.dll` ships inside Vanilla Expanded Framework, at
`Vanilla Expanded Framework/1.6/Assemblies/KCSG.dll`. There is no separate KCSG
mod to install.

The chain, all in `KCSG` code:

1. `KCSG.Postfix_Settlement_MapGeneratorDef` patches `Settlement.MapGeneratorDef`.
   If `faction.def.HasModExtension<CustomGenOption>()` it swaps the result to
   `MapGeneratorDef KCSG_Base_Faction`.
2. `KCSG_Base_Faction` is defined in
   `Vanilla Expanded Framework/Defs/CustomStructureGeneration/MapGeneration/
   MapGenerators.xml` and contains `RocksFromGrid` plus `KCSG_Settlement`.
3. `KCSG.GenStep_Settlement` calls
   `map.ParentFaction.def.GetModExtension<CustomGenOption>().Generate(loc, map)`.
4. `CustomGenOption.Generate` picks a `SettlementLayoutDef` or a
   `StructureLayoutDef`, cleans the rect, and pushes the `kcsg_settlement`
   symbol.
5. `KCSG.SymbolResolver_Settlement` adds the defenders and then builds the
   layout. Defenders come from `pawnGroupMakers`. If `pawnGroupMakers` is null it
   logs `Skipping AddHostilePawnGroup` and the base is empty. It picks
   `LordJob_DefendBase` when the group eats food, `LordJob_DefendBaseNoEat` when
   it does not. Momos eat, so they get the normal lord.

Two working examples in the user's Mods folder:

- `Reel's Insector Faction` — `FactionDef TribeInsector`, parent
  `TribeRoughBase`, `KCSG.CustomGenOption` with
  `chooseFromSettlements Reel_BugSettlement`, and its layouts are made of
  `VFEI_HiveWall`, `VFEI2_InsectJellyWall`, `VFEI2_RoyalJellyWall`,
  `VFEI2_Creep`, `Hive`, `VFEI2_Creeper`, `VFEI2_JellyFarm`, `GlowPod`.
- VFEI2 itself patches the vanilla `Insect` faction the same way, pointing at
  `VFEI2_InsectoidSettlement`, and ships 35 formation layouts tagged
  `VFEI2_InsectoidFormation`.

### 12.2 VFEI2 owns the vanilla Insect faction's spawning

`VFEInsectoids.PawnGroupKindWorker_GeneratePawns_Patch` prefixes
`PawnGroupKindWorker.GeneratePawns`. For `parms.faction == Faction.OfInsects` it
ignores `pawnGroupMakers` completely and builds the group from genelines: 70% of
points from `VFEI_Sorne`, 30% from a random other geneline.

Consequences: our old faction-side swap had only one consumer left (that
patch), and our new faction is unaffected because the patch checks the faction
identity, not the group kind.

### 12.3 VRE Insector facts

- `Gene_GenelineEvolution.GetGizmos()` returns early unless
  `pawn.IsColonistPlayerControlled`. Genelines are player-only.
- `Pawn_GeneTracker_CheckForOverrides_Patch` only re-applies exclusion rules
  between `GenelineGeneDef`s. No requirement to carry `VRE_GenelineEvolution`.
- The geneline balance rule (equal evolutions and degrades) lives in
  `Window_EditGeneline.CanAccept()`, a UI check. A fixed package is not affected.
- `VRE_InsectJellyDependency`: 0.2 per day drain, 0.5 per jelly, hediff severity
  +1 per day, deficiency 5, coma 30, death 60. `VRE_JellySacks` = 6 jelly per
  day.
- `VRE_ChestburstPregnancy` is a player gizmo. The mod has no raid, kidnap or
  abduction patches at all.
- `VRE_InsectPheromones` is Harmony-side and makes wild insectoids ignore the
  carrier. PMM Insects does not use the gene: `Source/Insects/InsectPheromones.cs`
  carries a copy of those two patches, gated on `PMM_Gene_Insect` (§5.3).

### 12.4 The Odyssey egg spew

`AbilityDef EggSpew` launches `Proj_EggSac`. `Proj_EggSac` uses
`Projectile_SpawnsThing` with `spawnsThingDef EggSac`. `EggSac` carries
`CompProperties_SpawnPawnOnDestroyed` with `pawnKind Larva` and
`lordJob LordJob_WanderNest`. This is what the old swap converted into a
greenworm.

### 12.5 Why the wild layer was the cheaper alternative, and why it is out

Recorded so nobody re-derives it. A momo can be made real wildlife with no
Harmony at all:

- `BiomeDef.AllWildAnimals` iterates every `PawnKindDef` and yields any with
  `CommonalityOfAnimal > 0`.
- `CommonalityOfAnimal` reads `BiomeDef.wildAnimals` plus every pawnkind whose
  race declares `wildBiomes`. No `RaceProps.Animal` filter.
- `WildAnimalSpawner.SpawnRandomWildAnimalAt` spawns with
  `PawnGenerator.GeneratePawn(kind)`, so faction null.
- `AggressiveAnimalIncidentUtility.CanArriveManhunter` requires
  `RaceProps.Animal`, so manhunter packs could never have picked a momo.

The user chose option C instead: no wild momos at all. If that ever changes, the
Reptiles mod already has a working pattern for factionless wild momos: a
pawn kind file plus an `IncidentWorker` that drops one in, driven by
`ReptileWandersIn.cs`.

### 12.6 Vanilla references used

- Parents: `TribeBase`, `TribeRoughBase`, `TribeSavageBase`, plus concrete
  `TribeCivil`, `TribeRough`, `TribeSavage` in
  `Data/Core/Defs/FactionDefs/Factions_Misc.xml`.
- Namers: `NamerFactionTribal`, `NamerSettlementTribal` in
  `Data/Core/Defs/RulePackDefs/RulePacks_Namers_Factions.xml`.
- Neutral tribal parent `TribeBase` at `Factions_Misc.xml` line 219, and the
  gentle tribe `TribeCivil` at line 442.
- Child kinds: `Villager_Child` and `Tribal_Child` in
  `Biotech/Defs/PawnKindDefs_Humanlikes/PawnKinds_Special.xml`. The field that
  marks a kind as a child is `pawnGroupDevelopmentStage`. Both sit in their
  faction's `Peaceful` group - `Tribal_Child` at `Factions_Misc.xml` line 346,
  `Villager_Child` at line 98 - which is the vanilla answer to where a child goes.
  `fixedChildBackstories` is vanilla too, used in `Anomaly/Defs/CreepjoinerDefs/
  Forms.xml` line 20.
- VFEI2 village layouts: `VFEI2_InsectoidSettlementRatingOne/Two/Three` in
  `Mods/Vanilla Factions Expanded - Insectoids 2/1.6/Defs/QuestScriptDefs/
  Script_EmergingHive.xml`.
- Raid loot: `TribeRaidLootMaker`.
- Vanilla insect faction: hidden, in
  `Data/Core/Defs/FactionDefs/Factions_Hidden.xml`.
- `PawnKindDef.ecoSystemWeight` is only counted for factionless pawns, in
  `WildAnimalSpawner.CurrentTotalAnimalWeight`. On faction pawns it does nothing.
- Leather: vanilla `Human` sets `<leatherDef>Leather_Human</leatherDef>` plus a
  `LeatherAmount` stat of 75, and every momo race is `ParentName="Human"` - so
  until 2026-09-20 a butchered momo yielded human leather. The six races now set
  `<leatherDef>VFEI2_Chitin</leatherDef>` with `LeatherAmount 30`, which is exactly
  what VFEI2's own insect races carry (30 chitin each, in `Races_Fuelmite.xml`,
  `Races_Megawasp.xml`, `Races_RoyalMegascarab.xml`). Meat and blood were already
  right: all six set `<useMeatFrom>Megaspider</useMeatFrom>` and
  `<bloodDef>Filth_BloodInsect</bloodDef>`, and `useMeatFrom` is a vanilla
  `RaceProperties` field whose leather twin is `useLeatherFrom`.
- Corpse categories are a story of their own, in four failed attempts and three
  caches: see §12.9.

### 12.7 VFEI2's insect territory system

Read from the 1.6 dll on 2026-09-20. This is what makes a VFEI2 insect base an
infested region, and what our tribes do *not* get today.

- `VFEInsectoids.WorldObjectsHolder_Add_Patch` postfixes `WorldObjectsHolder.Add`.
  For `settlement.Faction?.def == FactionDefOf.Insect` it calls
  `GameComponent_Insectoids.AddInsectHive(settlement)` and dirties
  `WorldDrawLayer_Insects`. The test is a hard reference to the vanilla Insect
  def, so our two tribes never create territory.
- `AddInsectHive` flood-fills the neighbourhood: radius 30 tiles, at most 50
  tiles, distance-weighted. The tiles are stored in
  `GameComponent_Insectoids.insectTiles`, keyed by the settlement.
- Three readers of that set:
  * `MapComponentUtility_GenerateMap_Patch` - for any non-pocket map whose tile
    `IsInfestedTile()`, pick a random `InsectMapGenDef` and run `DoMapGen`. That
    spawns 20-30 geneline hives, floods `VFEI2_Creep` out to
    `maxSpawnCreepRadius` 100, scatters hive surroundings, and spawns the geneline
    **boss** - but only `if (map.Parent is Settlement s && s.Faction ==
    Faction.OfInsects)`. It also pushes wandering factionless animals into the
    hive lord and extends `distToHiveToAttack` to 30.
  * `GameComponent_Insectoids.InfestationMtbDays(tile)` - inside a territory,
    infestation MTB becomes distance-to-hive x 5 days.
  * `WorldDrawLayer_Insects` - the world-map overlay.
- VFEI2 uses **no** `TileMutatorDef`. The "infested tile" is its own territory
  object, not a tile mutator, so there is no mutator to copy.
- Settlement count comes from `settlementGenerationWeight`, not from a min or max
  field. Vanilla's hidden `Insect` faction sets none, so it never settled; VFEI2
  patches it to 1. `requiredCountAtGameStart` and `startingCountAtWorldCreation`
  count faction **instances**, not settlements.

Consequences before hooking our tribes in: an infested tile applies the whole
`DoMapGen` storm to *every* non-pocket map generated there, our own village map
and any player colony in the region included. A killed village's territory entry
is pruned only in `ExposeData`, so it clears on save or load, not instantly.

**What we decided.** Ruling 22: the feral faction keeps no bases.
`Patches/InsectFaction_NoSettlements.xml` replaces its `settlementGenerationWeight`
back to 0, the value vanilla's hidden faction uses. With no Insect settlements
there are no territories, so no infested tiles, no overlay, no map-gen storm and
no boss-map path. Bosses remain reachable through the thumper buildings, and the
Emerging Hive quest can still turn its site into one real hive settlement, which
is treated as a feature. Raids and infestations are untouched: they come from the
faction, not from its settlements. Our mod loads after VFEI2, so our replace runs
after their add - that is why §9's dependency list had to ship in the same step.

### 12.8 How a child reaches a village, and the one engine rule that undoes it

**Not shipped.** The children this was written for were dropped by the user on
2026-09-20 (§5.5). All of it was verified in the 1.6 assembly and vanilla XML, so
it is kept here for the next attempt rather than thrown away.

Read 2026-09-20 from `Assembly-CSharp` (1.6) and from vanilla XML. Written down
because three separate rules here are easy to get wrong.

**1. A village's population is one group, and it is `Settlement`.**
In `SymbolResolver_Settlement.Resolve` the map gets exactly one pawn push:

```csharp
resolveParams2.pawnGroupKindDef = rp.pawnGroupKindDef ?? PawnGroupKindDefOf.Settlement;
resolveParams2.pawnGroupMakerParams.points = rp.settlementPawnGroupPoints ?? DefaultPawnsPoints.RandomInRange;
resolveParams2.pawnGroupMakerParams.inhabitants = true;
BaseGen.symbolStack.Push("pawnGroup", resolveParams2);
```

There is no second civilian pass, and the lord made just above it is
`LordJob_DefendBase`. So the `Settlement` group *is* the village, which is why the
children had to go in it rather than in `Peaceful` alone. Vanilla's own children do
sit in `Peaceful` (`Tribal_Child` at `Factions_Misc.xml` line 346, `Villager_Child`
at line 98), the group its peaceful encounters draw on, so ours are listed in both.
Default village points are 1150 to 1600, from `DefaultPawnsPoints`.

**2. `backstoryFilters` do nothing for a child.**
In `PawnBioAndNameGenerator.GetBackstoryCategoryFiltersFor` the child branch
returns before the kind and faction filters are ever read - it hands back a
hardcoded category group, `{"Child"}` - so a child of one of our kinds would roll
a vanilla human childhood. The field that is read, earlier in
`PawnBioAndNameGenerator`:

```csharp
if (pawn.kindDef.fixedChildBackstories.Any())
    pawn.story.Childhood = pawn.kindDef.fixedChildBackstories.RandomElement();
```

`fixedChildBackstories` is a `PawnKindDef` field (vanilla uses it in
`Anomaly/Defs/CreepjoinerDefs/Forms.xml`), so each child kind pins her species'
childhood with it and needs no filters at all.

**3. The engine re-ages every pawn of a hostile faction.**
In `PawnGroupKindWorker_Normal`:

```csharp
if (item.Option.kind.pawnGroupDevelopmentStage.HasValue)
    request.AllowedDevelopmentalStages = item.Option.kind.pawnGroupDevelopmentStage.Value;
if (!Find.Storyteller.difficulty.ChildRaidersAllowed
    && parms.faction != null && parms.faction.HostileTo(Faction.OfPlayer))
{
    request.AllowedDevelopmentalStages = DevelopmentalStage.Adult;
}
```

That second block wins, and `inhabitants` (read into a local a few lines above)
exempts nothing. Child raiders are off on the default difficulties, so a child of a
faction at war generates as an adult - a child's story and body type on an adult's
age and stats. Both tribes hit this: the swarm is `<permanentEnemy>`, and a
village's map is generated when it is attacked, and attacking is what makes the two
sides hostile in the first place. `Source/Insects/ChildStagePatch.cs` restores the
kind's own stage in a prefix on `PawnGenerator.GeneratePawn(PawnGenerationRequest)`
- the one overload the group workers call - and the request is a struct, so the
prefix takes it by `ref`. The prefix is gated on the `PMM_InsectChild` defName
prefix, so the difficulty setting, vanilla children and every other mod are left
alone.

**4. Role groups are free.** Vanilla `TribeBase` carries `Miners`, `Hunters`,
`Loggers` and `Farmers` at `commonality` 1, each `MayRequire="Ludeon.RimWorld.
Ideology"`, each holding a single faction-specific kind. Ours name the giant ant,
the devil bug and the soldier beetle. Only Ideology code paths ask for those group
kinds, so they cost nothing, and nothing else in the mod depends on them.

### 12.9 One line for momo corpses: a snapshot, three caches, and Big and Small

Read 2026-09-20 from `Assembly-CSharp`, `BigAndSmall.dll` and vanilla XML. It took
four attempts to make this stick, so the whole chain is written down - every step of
it looked like a complete answer at the time.

**1. A humanlike race's corpse ignores its flesh type.** In
`ThingDefGenerator_Corpses.GenerateCorpseDef`:

```csharp
thingCategories.Add(pawnDef.race.Humanlike
    ? ThingCategoryDefOf.CorpsesHumanlike
    : pawnDef.race.FleshType.corpseCategory);
```

The humanlike test comes first, so `fleshType Insectoid` cannot move a momo corpse,
and there is no XML field for the category at all.

**2. Corpses are generated defs, so no patch can reach them.** They are implied
defs, created during def loading after XML patching, and addressed through
`DirectXmlCrossRefLoader.RegisterListWantsCrossRef` rather than XML.

**3. Big and Small files corpses too.** `BigAndSmall.RaceFuser.GenerateCorpse`
does the same job for fused races, into `BS_CorpsesHumanlikeHybrids` ("Hybrids") or
`BS_CorpsesHumanlikeAnimals` ("Sapient animals"), both defined in
`Big and Small - Framework/1.6/Base/Defs/ThingCategoryDefs.xml` - and both children
of `CorpsesHumanlike`, which is the pattern ours copies. The order of mods'
`[StaticConstructorOnStartup]` constructors is not ours to choose, so B&S could
simply run after us and put the corpses back. It did.

**4. Menus read a snapshot, not the def.**
`ThingCategoryNodeDatabase.FinalizeInit` runs once, before any mod's static
constructor, and does `thingCategory.childThingDefs.Add(thingDef)` for every def in
the database. Every filter menu draws from that, so moving a corpse's
`thingCategories` afterwards changes nothing that is drawn. Ticking the category
still appears to work, because `ThingFilter.SetAllow` walks the *live*
`DescendantThingDefs`, and `Allows(ThingDef)` is plain membership of an allowed set.

**5. And the rows come from a second, cached list.**
`ThingCategoryDef.ResolveReferences` (load time) also builds
`sortedChildThingDefsCached` - the rows `Listing_TreeThingFilter` draws - plus
`allChildThingDefsCached` for membership tests. Both are private and already built,
so after any move they must be rebuilt by calling `ResolveReferences()` again on
every category def, parents included, because a parent's cached set covers its
descendants.

**What the code does.** `Project Momo` owns both halves now (2026-09-20):
`Defs/ThingCategoryDefs/ThingCategories_MomoCorpses.xml`
defines `PMM_MomoCorpses` ("momo corpses"), a child of `CorpsesHumanlike` the way
vanilla's `CorpsesInsect` is a child of `CorpsesAnimal` - that nesting is what keeps
every filter that allows humanlike corpses accepting a momo corpse, and what makes
the menu show one line instead of one entry per race - and
`Source/ProjectMomo/MomoCorpses.cs` moves the corpses out of `CorpsesHumanlike` and
B&S's two categories, keeps `childThingDefs` in step on both sides, and calls
`ResolveReferences()` on every category to rebuild the caches. `MomoCorpses.Register(...)`
is called once per species mod from its own `[StaticConstructorOnStartup]` (insects:
`Source/Insects/InsectMomoCorpses.cs`), and the move runs again from a postfix on
`StaticConstructorOnStartupUtility.CallAll()` - that postfix runs after every mod's
startup constructor, so ours is the last word whatever other mods do.

**The symptom at each attempt**, for the next person who sees one of them:

| Attempt | Change | What the user saw |
|---|---|---|
| 1 | move the defs in a startup constructor | nothing moved; B&S won the ordering |
| 2 | + `CallAll` postfix, + strip B&S's categories | line appeared, ticking it worked, **no rows inside** |
| 3 | + `childThingDefs` bookkeeping | still no rows: the drawn list is `SortedChildThingDefs` |
| 4 | + `ResolveReferences()` on every category | correct |

---

## 13. Changelog plan

`CHANGELOG.md` must change whenever defs, source, patches, About or textures
change, or `changelog-check.sh` warns and `build-all.sh --strict` fails.

Shape: flat dated lines, newest first, two sections, no version headings. Verbs
only from Added / Changed / Fixed / Removed / Rebalanced. About 25 words. No file
names, class names or tick counts.

The entries this work will need, written when each phase lands:

Every entry is one line. Do not wrap an entry across two lines, or the checker
fails it.

```markdown
## Player-facing

- 2026-09-20: Added Vanilla Races Expanded - Insector and Vanilla Factions Expanded - Insectoids 2 as required mods.
- 2026-09-20: Added insector tribes. Their villages are built from insect hive walls and hives.
- 2026-09-20: Changed the insect momos to live only in insector tribes. Vanilla insects spawn normally again.
- 2026-09-20: Removed wild insect momos.
- 2026-09-20: Changed each insect momo caste to fight with insect genes, so her strengths match her species.
- 2026-09-20: Added insect jelly as food for the insect momos.
- 2026-09-20: Changed a kidnapped man to be held prisoner in the tribe's village, instead of being lost forever.
```

```markdown
## Internal

- 2026-09-20: Removed the pawn generation swap and the wild momo layer.
```

One change per line. Do not batch them into one entry.
