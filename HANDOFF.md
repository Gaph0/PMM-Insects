# PMM Insects - Insector Tribes hand-off

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
mamono, and one "mamono corpses" line in the item filters (§10, §12.9). That line moved
into core on 2026-09-20, so the slime and elemental mamonos share it (§11 item 5).
Phases 6 to 8: the hive build gate is still open, phase 7 was closed on 2026-09-24 by
deleting the hive bond rule and its vanish rather than repointing them (§8), and
phase 8's trade landed on 2026-09-22. What is left is the field tests in §10 and the
art and tuning in §11.

`archive/PLAN.md` holds the old design: a total spawn swap, plus tameable wild
mamonos. It was archived on 2026-10-01, once the mod had shipped as two tribes and its
one live section - the papillon maturation - moved into `CASTES-PLAN.md` §7. Where the
two files disagree, this one wins. What is left there is the design reference, with §2
(the swap), §5 (the wild-man recipe) and §6 (their risk list) deleted on 2026-09-24.

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
| Vanilla insects | Removed from the game. Every spawn becomes a mamono. | Normal. Real bugs spawn everywhere again. |
| Wild mamonos | Exist. Spawn from factionless bug spawns, tameable. | Gone. The wild layer is deleted. |
| Insect mamonos | Two jobs: the vanilla Insect faction, or a wild woman. | One job: citizens, raiders and brood of the new insector tribes. |
| Their home | None. They use vanilla and VFEI2 hives. | Their own villages, built from VFEI2 hive walls and hives. |
| Their character | Six species with their own genes. | Six castes, each with a VRE Insector gene package. |
| Mods required | Harmony, Biotech, PMM.Core, B&S. | Plus VRE Insector, VFEI2 and VEF. All hard. |
| Code | A global prefix on pawn generation. | No pawn-generation hook at all. |

The mod becomes **purely additive**. It stops touching vanilla pawn generation,
so vanilla insects, VFEI2 genelines and other insect mods all work normally.

---

## 2. Locked rulings (user, 2026-09-20)

1. **Wild layer: option C.** Delete the swap and the wild mamonos. Mamonos live only
   in the tribes.
2. **Castes: option 1.** All six species stay, all six spawn in the tribes and
   villages.
3. **Kidnapping is allowed**, vanilla style. The tribes now have bases, so a
   prisoner can be rescued from one.
4. **VFEI2 is a hard dependency.**
5. **No mamonos in the vanilla Insect faction.** The tribes own every faction-side
   mamono kind.
6. **The hive-abduction wrapper is gone.** No vanish, no special letter. Vanilla
   kidnapping only.
7. **The bond rule is gone too (changed 2026-09-24).** A hive mamono uses the core's
   tsugai behaviour like any other mamono, so she can join the colony on a bond win.
   §8 lists what was deleted and why.
8. **No chestburst pregnancy on mamonos.** Hard no.
9. **Jelly dependency: combat castes only.** Only the castes that read as fighters
   carry `VRE_InsectJellyDependency`. The greenworm does not.
10. **No `VRE_JellySacks` on any mamono.** Pawns never make jelly. All of it comes
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

- `Source/Insects/InsectSwap.cs` - the whole `Patch_InsectGenerationSwap`.
  Nothing needs it once the mamonos are a faction.
- `Source/Insects/WildInsects.cs` - `Patch_InsectIsWildMan` and
  `Patch_InsectShouldNotReachOutside`. No factionless mamonos means no wild-man
  behaviour to fake.
- `Defs/PawnKindDefs/PawnKinds_InsectWild.xml` - all six `PMM_Wild*` kinds.
- `Defs/ThinkTreeDefs/ThinkTrees_InsectWild.xml` - the wild think trees.
- `Source/Insects/HiveBondKidnap.cs`, `Source/Insects/HiveKidnapVanish.cs` and
  `Source/Insects/FactionJoinQuiet.cs` - deleted 2026-09-24. The hive bond rule,
  the vanish and the swap-era log patch were all unreachable; §8 has the detail.

### Keep

- `Source/Insects/InsectsMod.cs` - Harmony bootstrap. Add the new patch classes
  here or beside them.
- `Source/Insects/HealthScalePatch.cs` - still needed for the two fragile
  species.
- `Source/Insects/HiveNourishment.cs` - keep, but widen the hive lookup (§7).
- `Defs/ThingDefs/Races_InsectMamono_BS.xml` - the six races. Still the home of
  every stat.
- `Defs/GeneDefs/*` - the shared insect gene and the chitin skin genes.
- `Defs/BackstoryDefs/Backstories_Insect.xml` - keep, but re-read the stories.
  Any line that talks about being found in the wild now points at the wrong
  place.

### Rewrite

- `Defs/PawnKindDefs/PawnKinds_InsectFaction.xml` - becomes the tribe's kind
  file. Change `defaultFactionDef` from `Insect` to the new faction. Drop the
  `ecoSystemWeight` lines: they only count for factionless pawns, so they do
  nothing here. Rename the file to `PawnKinds_InsectorTribe.xml` if you like.
### Add

Landed already:

- `Defs/FactionDefs/Factions_InsectorTribes.xml` - one shared abstract base and
  two factions, neutral and hostile (§4, §4.7).
- `Defs/RulePackDefs/Namers_Insector.xml` - the five faction names (§4.5).
- `Source/Insects/InsectMamonoCorpses.cs` - registers the six insect races on the
  family's shared `PMM_MamonoCorpses` line. The category def and the mover moved into
  `Project Mamono` on 2026-09-20 (`Defs/ThingCategoryDefs/ThingCategories_MamonoCorpses.xml`
  and `Source/ProjectMamono/MamonoCorpses.cs`), so the insects, the slimes and the
  elementals share one line (§11 item 5, §12.9).
- `Source/Insects/InsectPheromones.cs` - the pheromone patches VRE keeps inside
  their gene, moved onto `PMM_Gene_Insect` (§5.3).
- `Defs/GeneDefs/Genes_Insect.xml` - extended, not new: chitin skin, feelers, full
  toxin immunity, pheromones and insect flesh all live on this one gene (§5.3).
- `Patches/InsectFaction_NoSettlements.xml` - the feral faction's bases off the map
  (§12.7).
- `Defs/AbilityDefs/Abilities_EggSpew.xml` and `Defs/ThingDefs/Things_EggSpew.xml`
  - the brood, landed 2026-09-20 (§5.4). Ungated, and it hatches a
  `VFEI2_Swarmling`.

Still to come:

- `Defs/TraderKindDefs/Trader_InsectorTribe.xml` - the caravan trader (§4.6).
- `Textures/` - art for the neutral tribe (§4.4).
- `About/About.xml` - already carries the dependency block (§9).
- `CHANGELOG.md` - entries as the work lands (§13).

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
`Project Mamono Reptiles/Defs/FactionDefs/Factions_Reptile.xml` shows the project
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
| `factionNameMaker` | `PMM_NamerFactionInsectorHive` (neutral), `PMM_NamerFactionInsectorSwarm` (hostile) - §4.5 | |
| `settlementNameMaker` | `NamerSettlementTribal` | Vanilla tribal namer, ruled. Inherited from `TribeBase`; the def no longer repeats it. |
| `factionIconPath` | own art for both: `World/WorldObjects/Expanding/PMM_InsectorHive` (neutral, the user's crest, 2026-10-01), `.../PMM_InsectorSwarm` (hostile, 2026-09-20) | The neutral one used to be vanilla's - the hidden insect faction declares that path, `Core/Defs/FactionDefs/Factions_Hidden.xml`. Vanilla Textures Expanded only re-ships a retouched copy at that path; nothing points there now. |
| `settlementTexturePath` | own art for both: `World/WorldObjects/Expanding/PMM_InsectorHive` (neutral), `.../PMM_InsectorSwarm` (hostile) | Mandatory: leaving it unset throws on every settlement draw. It is what draws the village on the world map, not `factionIconPath`. The neutral one was VFEI2's `UI/InsectoidHive` until 2026-10-01. |
| `canStageAttacks` | `true` | Inherited. See the note below. |
| `raidLootMaker` | `TribeRaidLootMaker` | Vanilla def, inherited from `TribeBase` (the def no longer repeats it). The Broods use it too. |
| `requiredCountAtGameStart` | `1` | House pattern. The world always gets one tribe. |
| `startingCountAtWorldCreation` | `1` | **This is faction instances, not settlements.** In Reptiles, 4 produced four duplicate Dragonias. |
| `maxConfigurableAtWorldCreation` | `9999` | **Changed 2026-10-01.** Vanilla's own number for every tribe faction (`TribeCivil` carries it, and both tribal bases do too) - an uncapped slider. It read `10` before: a house number, not a decision. |
| `allowedCultures` | `Corunan` | House pattern. |
| `backstoryFilters` | `Tribal` | House pattern. |
| `xenotypeSet` | `Inherit="False"`, the six mamono xenotypes | Keeps random pawns inside the mod. |
| `autoFlee` | `false` | Bugs do not run. |
| `colorSpectrum` | chitin tones | World map and letters. |
| `maxPawnCostPerTotalPointsCurve` | own curve, see §4.7 | The vanilla tribal curve cannot allow a 500-point pawn into a village at all. |
| `caravanTraderKinds` | `PMM_InsectorTribeTrader` | §4.6. |
| `modExtensions` | `KCSG.CustomGenOption` | The village base. See §4.3. |

**Neutral, but still able to raid.** The user asked for neutral. `TribeBase`
sets `canStageAttacks true`, so leave it: the tribe starts neutral and can raid
once relations sour. That is how vanilla gentle tribes behave. Dragonia does not
inherit it, which is why Dragonia never raids. Clearing the field gives you the
Dragonia behaviour exactly, but then the mamonos never raid at all, and §4.2 has
nowhere to happen.

Raids are melee-only. The mamonos carry no weapons and no apparel, so there is no
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

If a mamono layout is ever wanted on top of these, it is added to the same list,
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
- Neutral tribe: own art as of 2026-10-01, one crest for both fields
  (`PMM_InsectorHive`, from the user's `insectsvillage.png`). It used to borrow
  VFEI2's `UI/InsectoidHive` for its villages and vanilla's
  `World/WorldObjects/Expanding/Insects` for its icon, which is the hidden insect
  faction's own. (Corrected 2026-10-01: an earlier revision of this note called the
  icon missing without Vanilla Textures Expanded. That mod ships a retouched copy at
  the same path, so with it loaded the game draws that version, but the icon is
  vanilla's and needed no dependency. Both borrowings are gone.)
- The art convention is 128x128 on a transparent background, white fill, black
  outline, drawn tinted by the faction colour - which is exactly how vanilla builds
  its own village markers (checked 2026-10-01 against `Village.png`: the same white
  and black pixel counts on a transparent field). Both crests follow it, so no art
  needed redoing. Two things to know before the next count of borrowed art: the game
  ships no loose vanilla textures, so a texture path with no file on disk anywhere is
  vanilla's, from the Unity bundle; and Vanilla Textures Expanded re-ships the vanilla
  icon set at vanilla paths, so "our copy matched that mod's file" never proves a path
  belongs to that mod.
- No borrowed art, audited 2026-10-01: not one file we ship matches a file in
  vanilla or in any workshop mod, byte for byte. Two references still point outside
  the mod - both deliberate, both explained where they appear, and both left alone by
  the user's call of 2026-10-01 ("referencing other mods is OK"): the arachne silk
  item wears vanilla's `Things/Item/Resource/Cloth` with its own tint
  (`Things_ArachneSilk.xml`), and the web trap wears Alpha Animals' `UI/Abilities/AA_Web`
  in a def that is entirely `MayRequire="sarg.alphaanimals"`, so that icon's owner
  is always loaded whenever the def exists.
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
without a leader kind gets a leaderless tribe, the same way vanilla treats a faction
with no leader kind.

**Two things the field test confirmed.** Villages generate with hive walls, creep,
hives and defenders, and the real insects that hatch from a village's hives belong
to the village's faction, so they defend it instead of fighting the mamonos beside
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

## 5. The castes

### 5.1 What a caste is here

VRE genelines cannot be used on NPCs (§6.1), so a caste is five layers:

1. **Race** - `Races_InsectMamono_BS.xml`. Body size, health scale, hunger, melee
   tools, temperature range. Genes cannot change these.
2. **Xenotype** - the gene package. This is our stand-in for a geneline.
3. **PawnKindDef** - race, `combatPower`, xenotype, gender, backstories.
4. **Group slot** - where the kind is listed in `pawnGroupMakers`.
5. **Role** - what she does in a village, a raid, or a caravan.

Layer 3 is what makes a caste feel different, because raid points buy pawns at
`combatPower`.

### 5.2 The six, with the numbers already fixed

| Caste | Vanilla bug | CP | Health scale | Body size | Role after the overhaul |
|---|---|---|---|---|---|
| Devil Bug | Megascarab | 40 | 0.4 | 1.0 | Swarm filler and hive builder. High weight in `Combat`. Also a village civilian. |
| Giant Ant | Spelopede | 75 | 1.7 | 1.0 | Line trooper and worker. The tribe's digger and hauler. |
| Soldier Beetle | Megaspider | 150 | 2.5 | 1.2 | Heavy melee. Caste leader. |
| Greenworm | Larva | 25 | 0.25 | 1.0 | Brood only. Comes out of the queen's egg spew. |
| Vamp Mosquito | Locust | 55 | 0.7 | 0.6 | Fast flyer. Ranged slot only if she has an ability. |
| Abaddon | Hive Queen | 500 | 9.8 | 4.5 | The queen. `Settlement` and `leaders`. The reason to raid a village. |

These numbers come from `archive/PLAN.md` §3, with one change: four castes now start
full-grown so that `VRE_Microsized` shrinks them from there rather than shrinking
them twice over - the Devil Bug (0.2 -> 0.6 -> 1.0), the mosquito, the Giant Ant and
the Greenworm (0.8 and 0.6 -> 1.0), all on 2026-09-27.

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
| Devil Bug (40) | `VRE_Hiveglands`, `VRE_SwarmSynapse`*, `VRE_InfraredSensors`* | `VRE_LowGreyMatter`, `VRE_RapidLifeCycle`, `VRE_Microsized`* | Raises hives, sees heat in the dark and answers the swarm's call - small, dim, short-lived and bad with people, but a good builder. Package replaced 2026-09-27 |
| Giant Ant (75) | `VRE_SpelopedeHorn`, `VRE_SwarmSynapse`*, `VRE_RobustMidgut` | `VRE_LowGreyMatter`, `VRE_RapidLifeCycle`, `VRE_Microsized`* | Digs and hauls, eats anything raw, sleeps little, needs no jelly - strong, dim, short-lived and sterile. Package replaced 2026-09-27 |
| Soldier Beetle (150) | `VRE_HardenedChitin`, `VRE_CuticleShell` | `VRE_InefficientMidgut`, `VRE_Stenothermic` | Armoured and unplagued, but she eats like a horse and only thrives at home |
| Greenworm (25) | none | `VRE_Immunodeficiency`, `VRE_PorousSkin`, `VRE_WeakenedChitin`, `VRE_PassiveInsect`*, `VRE_Microsized`*, `VRE_InefficientMidgut`, `VRE_LowGreyMatter` | A soft, passive, dim grub: catches everything, fights nothing, always hungry, and needs jelly. Degrade-heavy on purpose, with no evolution at all - the balance rule is broken for her by design. Package replaced 2026-09-27 |
| Vamp Mosquito (55) | `VRE_HighGreyMatter`, `VRE_AntimicrobialPeptides` | `VRE_AcidBlood`, `VRE_WeakenedChitin` | Cunning and fast-mending, thin-shelled, and her blood smokes when cut |
| Abaddon (500) | `VRE_SwarmSynapse`*, `VRE_RobustMidgut`, `VRE_SlowedLifeCycle`, `VRE_VocalGlands` | `VRE_Colossal`, `VRE_Dormant`, `VRE_HardLockedJoints`, `VRE_HypothermicHibernation` | A queen who never ages, eats raw food without harm and speaks with a beautiful human voice, paid for with hunger, dormancy, stiff joints and cold. Reworked 2026-09-26 (§5.10) |

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
two and two (four and four for the queen, after her three passes on 2026-09-26: the
Fertility clash first left her at three evolutions against four mutations, vocal glands
swapped the inefficient midgut for hard-locked joints and put her level again, and the
later parthenogenesis-for-robust-midgut swap was evolution for evolution - see §5.10). That is VRE's own
rule for a
geneline, in its language file (`Languages/English/Keyed/Keys.xml`):
`VRE_NeedEqualAmount`, "Needs equal amount of evolutions and degrades." Nothing in
the engine enforces it on a xenotype, so it is a house rule we keep by hand, and
it is why each caste above has its cost listed next to its gift.
`VRE_InsectJellyDependency` is neither an evolution nor a degrade, so it sits
outside the count.

On top of those, the fighters carry the jelly dependency, and everything else a mamono
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
`Pawn.ThreatDisabledBecauseNonAggressiveRoamer`. So a mamono used to need two genes to
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
insect meat, her own kind included, is cannibalism to a mamono and ordinary food to
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
`VRE_Hiveglands` is what lets a mamono raise VFEI2 hives at all (§7.1), so the ant
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
   recruitment. The user is fine with that, and several are in the packages now,
   among others: `VRE_InfraredSensors` and `VRE_Microsized` (the devil bug),
   `VRE_Microsized` (the mosquito), `VRE_SwarmSynapse` (the queen's crown and the
   devil bug), `VRE_ChargerClaws` (the girtablilu), and `VRE_HardLockedJoints` and
   `VRE_Colossal` (the queen).

**The pollution immunity: built, on the gene we already had.** The ruling is full
immunity for all six castes. Checked in the 1.6 assembly: `immuneToToxGasExposure`
is a field on `ApparelProperties` and on `GeneDef`, **not on `ThingDef`** - so a
race def cannot grant it; only a gene can. The user's answer (2026-09-20) was to
put it on `PMM_Gene_Insect`, the insectoid gene every mamono already carries
(`Defs/GeneDefs/Genes_Insect.xml`), instead of adding a second gene:

```xml
<immuneToToxGasExposure>true</immuneToToxGasExposure>
<statOffsets>
        <ToxicEnvironmentResistance>1</ToxicEnvironmentResistance>
</statOffsets>
```

One edit covers all six castes, and it also covers a woman transformed into a mamono
at runtime, since that path grants the same gene. Vanilla's own
`ToxicEnvironmentResistance_Total` ("total antitoxic lungs",
`Data/Biotech/Defs/GeneDefs/GeneDefs_Health.xml` line 172) was the alternative: the
same two fields plus rot-stink immunity, `biostatCpx 2`, `biostatMet -3`. It would
have been a second gene saying the same thing. The races keep their own
`ToxicEnvironmentResistance 0.8`, because buildup is severity x (1 - resistance),
so 0.8 plus 1 is already past the point where buildup stops.

Knock-on: `VRE_VestigialTubules` (the toxic degrade) and `VRE_DetoxifierTubules` are
now no-ops on a mamono, so neither is in any package, and the Devil Bug no longer
needs `VRE_DetoxifierTubules` as "her extra". `biostatMet` on the insectoid gene is
still 1; raise it if the immunity should ever cost her hunger.

**Decided by the user, 2026-09-20.**

2. **Immunity mechanism: done, on `PMM_Gene_Insect`** (above).
4. **`VRE_Hiveglands` goes on the Ant Arachne and the Devil Bug**, so two castes
   raise VFEI2 hives (§7.1). The Giant Ant and the Greenworm lost it with their
   packages of 2026-09-27; the rest never had it.
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
   the mechanic: a pherocore gene belongs to one hive's bloodline, and the mamonos are
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
   `pawn.IsColonistPlayerControlled`. On a recruited mamono it opens
   `Window_ManageGenelines`: choose up to four genes from the unlocked pool, she
   sits in a `VRE_Metapod` for 24 hours, then the new loadout lands. The catch is
   `Geneline.AddPawnDirectly`, which removes **every** gene on the pawn whose def
   `is GenelineGeneDef` and which is not in the new loadout - and the whole caste
   package is made of `GenelineGeneDef`s. So her first switch deletes the package
   she was born with (her `PMM_Gene_Insect`, `ProjectMamono_Mamono`, skin genes and
   vanilla genes survive, because those are plain `GeneDef`s). That is either the
   point of VRE's system or a trap, depending on taste. Without the gene she can
   still be customised the vanilla way, with gene packs and an assembler, but never
   through a geneline. Left off, and it is one line to add if that is ever wanted.

Two picks depend on rule 4: `VRE_InfraredSensors` (the devil bug's heat sense) and
`VRE_SwarmSynapse` (the queen's crown). If locked genes are ever taken back off the
castes, the free stand-ins that pass the tag check are `VRE_CompoundEyes`,
`VRE_HighGreyMatter` and `VRE_OcelliEyes`.

**What the packages add up to.** The fighters carry `VRE_InsectJellyDependency`
(`biostatMet 3`) on top of the mamono gene's 3 and the insectoid gene's 1. Metabolism
raises food need, so the jelly gene makes the fighters eat more than they used to.
If the tribes turn out to eat the map, lower the insectoid gene or the jelly gene
before trimming the caste genes.

**Still binding, from the first draft.** Packages stay balanced between evolutes and
degrades, so the Health tab reads cleanly. VRE's `disableGeneExtraction` stays as
shipped, so a caste gene cannot be pulled out of a mamono. The xenotypes stay
`inheritable`, so daughters inherit the caste. Jelly on the fighters only, never
`VRE_JellySacks` (§6.2). Pheromones come from `PMM_Gene_Insect` rather than from a
VRE gene, and that copy is ours to maintain. The two spew abilities the mod already
has stay as they are: `SludgeSpew` on the greenworm and the `EggSpew` clone on the
abaddon (§5.4).

### 5.4 The brood (egg spew) - built 2026-09-20

**Built, awaiting its field test. The brood is a VFEI2 swarmling, not a mamono** (the
user's call, 2026-09-20). The clone is `PMM_Ability_EggSpew`, `PMM_Proj_EggSac`
and `PMM_EggSac` (`Defs/AbilityDefs/Abilities_EggSpew.xml` and
`Defs/ThingDefs/Things_EggSpew.xml`), all ungated Core defs, and the abaddon
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
- Why not a mamono: Core cannot generate a HUMANLIKE pawn there at all. It asks for
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
- the vanilla egg-sac art is not a loose file, so it cannot be reused. Ours (the
  building, its projectile, and the ability icon) is placeholder art like the rest
  of the mod's.

Work item, and it must not be missed. Pencilled in before the build: the Abaddon
carried
`CompProperties_SpreadSludge` with `abilityDef EggSpew`, and the egg sac it
throws spawns a vanilla `Larva`. The old swap turned that larva into a
greenworm. With the swap gone, it spawns a real larva again.

That comp has since been deleted from her race (2026-09-26): nothing casts the egg spew
for her any more, and the sac carries a swarmling. See §5.11.

The vanilla defs are the template, and this is pure XML:

- the DLC's `Defs/AbilityDefs/Abilities.xml` - `AbilityDef EggSpew` launches
  `Proj_EggSac`; `ThingDef Proj_EggSac` is a `Projectile_SpawnsThing` with
  `spawnsThingDef EggSac`.
- the DLC's `Defs/ThingDefs_Buildings/Buildings_Misc.xml` - `ThingDef
  EggSac` has `CompProperties_SpawnPawnOnDestroyed` with `pawnKind Larva`.

So: clone the chain as `PMM_Ability_EggSpew`, `PMM_Proj_EggSac` and
`PMM_EggSac`, with `pawnKind VFEI2_Swarmling`, and give the clone to the
Abaddon's race comp. Check two things while cloning:

- `compClass CompSpawnLarva` is the vanilla class for larvae. Drop the override
  and let `CompProperties_SpawnPawnOnDestroyed` use its default comp.
- `lordJob LordJob_WanderNest` may not suit a humanlike pawn. Either drop it, or
  test it, or point it at a job the brood can follow.

Decide which faction the brood belongs to. A brood spawned next to the queen
should belong to the queen's faction, so the village can use her. Test this in a
dev-spawned village before shipping.

### 5.5 Children - dropped by the user (2026-09-20)

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

### 5.6 Abaddon Folk - the seventh species (2026-09-25)

Added at the user's request from the wiki entry: the soldier class of an abaddon's
swarm, the "soldier bugs" that make up the bulk of any swarm an abaddon leads. She
is the first species here with **no vanilla bug behind her** - each of the six above
replaces one insect - so nothing about her is copied 1:1 and her numbers are set for
the role instead.

**She deliberately does not spawn** (user ruling 2026-09-25, "doesn't spawn
normally"). That is a concrete shape, not a tone of voice:

| Layer | What she has |
|---|---|
| Race | `PMM_Race_AbaddonFolk` + `PMM_RaceTracker_AbaddonFolk` in `Defs/ThingDefs/Races_InsectMamono_BS.xml` |
| Xenotype | `PMM_InsectAbaddonFolk` in `Defs/XenotypeDefs/Xenotypes_Insect.xml` |
| PawnKindDef | `PMM_InsectAbaddonFolk`, in `Defs/PawnKindDefs/PawnKinds_AbaddonFolk.xml` - see the correction below |
| Pawn groups | none, in neither tribe |
| Faction `xenotypeSet` | no entry, so even a kindless pawn of either tribe never rolls her |

So she is reachable by hand: dev mode, a xenogerm, or a corruptor who is
already one. Nothing needed registering for that last path -
`MamonoTransformation.IsMonsterXenotype` tests a xenotype's gene list for
`ProjectMamono_Mamono`, not a list of defNames - so any mamono wearing her xenotype
imprints it on the women she takes. If she is ever given a spawn source (a village
caste, the queen's egg spew, a special raid), the pawn groups are the piece written
then.

**Correction, same day: she does have a pawn kind.** The table above first said "none,
on purpose", on the theory that a kind is the thing that makes a species spawnable.
That theory is wrong in one direction and costs a test round: **the dev-mode spawn
list is a list of pawn kinds**, so a species with no kind cannot be spawned for
testing at all. `PMM_InsectAbaddonFolk` therefore exists, and it stays out of every
faction's `pawnGroupMakers` and `xenotypeSet`, so the world still never spawns her -
a kind with no group listing is reachable only when something asks for it by name.

**And that is how the four arms were "missing".** The user was testing with
vanilla's dev *apply xenotype* action, and Big & Small reads a xenotype's declared
race (`XenotypeExtension.setRace`) when a pawn is **generated** - so applying her
xenotype to a living pawn leaves that pawn with her genes and the body it already
had: no four arms, no wings, no chitin, because none of that lives in genes.
`MamonoTransformation` calls `BigAndSmall.XenoTypeDefExtensions.
TrySwapToXenotypeThingDef` for exactly this reason; vanilla's dev action did not.

**Closed the same day, in core.** `Source/ProjectMamono/XenotypeRacePatch.cs` is a
postfix on `Pawn_GeneTracker.SetXenotype` that calls `ApplyXenotypeRace` whenever the
new xenotype declares a race the pawn is not already wearing - so dev "apply
xenotype" now gives the pawn the species' body, and so does anything else that sets a
xenotype at runtime. It is narrow on purpose: no `XenotypeExtension`/`setRace`, no
swap (every vanilla xenotype); pawn already that race, no swap (so ordinary generation
is not swapped twice); animals skipped. **Until 2026-09-25 the workaround for testing
was to spawn her kind instead**, and that is still the only way to get her as a
*faction-less* pawn rather than by converting someone.

**Her gene list** is the user's list of 2026-09-25, mapped to defNames: swarm
synapse `VRE_SwarmSynapse`, hardened chitin `VRE_HardenedChitin`, microsized
`VRE_Microsized`, inefficient midgut `VRE_InefficientMidgut`, jelly dependency
`VRE_InsectJellyDependency`, insect skin `VRE_InsectSkin`, insect antennae
`VRE_InsectAntennae`, weak flight `PMM_Gene_FlightWeak`, poor social
`AptitudePoor_Social`, mamono `ProjectMamono_Mamono`, plus the body colour rolled for her (§5.13)
- and **great melee** `AptitudeRemarkable_Melee`, asked for later the same day (vanilla's
`AptitudeRemarkable` template is the label "great {0}": +8 aptitude and a passion
level).

Three notes on how that list departs from §5.3's rules for the six:

1. **"Insect pheromones" needed no gene.** `PMM_Gene_Insect` already carries the
   behaviour in our own Harmony copy (§5.3), and VRE's `VRE_InsectPheromones` is
   only the flag those patches read. Listing it would be a second flag for one
   feature, so it is absent and the behaviour is identical either way.
2. **`VRE_InsectSkin` is in, against §5.3's exclusion** (user's call, caveat shown:
   "accepting the lost body colour / VRE heads"). The colour is not actually lost: its fur
   carries `useSkinColorForFur`, so the shell is drawn in the pawn's skin colour, which her
   chitin gene sets. Its exclusion tags are body and fur tags (`AG_Bodies`, `Bodies`, `Body`,
   `Fur`) while the tone carries vanilla's `SkinColorOverride`, so the two never conflict, and
   the antennae gene's `Antenna` tags conflict with nothing here. The armour-stacking worry
   behind §5.3's ban is answered differently here: **her race tracker carries no armour stage at
   all**, so `VRE_InsectSkin`'s sharp 0.27 / blunt 0.18 is the only armour on top of
   `VRE_HardenedChitin`'s quarter-off.
3. **`VRE_Microsized` is in, and it does sit on top of B&S sizing** - exactly the
   thing §5.3 excluded it for. Here it is wanted. The first guess at the numbers was
   wrong, though, and the user caught it in game: nothing takes a percentage off, so
   read §5.8 before touching her size.

Balance follows VRE's geneline rule, two evolutions (swarm synapse, hardened
chitin) against two degrades (microsized, inefficient midgut); the jelly dependency
and the cosmetic antennae sit outside the count.

Numbers, for the record: MoveSpeed 4.0, chosen to offset
`VRE_HardenedChitin`'s -0.15 Moving capacity; health scale 1.0, so no entry is owed
in `Source/Insects/HealthScalePatch.cs` (the roster comment there names her);
temperature -10 to 60 °C; market value 150; internal damage halved; wings on the
race plus the B&S winged tracker, because a weak-flight carrier needs a race that
already has the wing body. She also carries the hive-nourishment comp every caste
has, and her corpse is registered in `Source/Insects/InsectMamonoCorpses.cs`.

### 5.7 Four arms, and the arms' art (2026-09-25)

The wiki gives her two pairs of arms. Built, in three layers:

| Layer | Def |
|---|---|
| Body | `PMM_Body_FourArmedWinged` (`Defs/BodyDefs/Body_FourArmedWinged.xml`): B&S's four-armed humanlike with their two `BS_Wing` parts added, so she has four arms *and* keeps her wings. **Shared with the abaddon queen** since 2026-09-26 - one body def, both species |
| Groups | `PMM_LowerLeftHand` / `PMM_LowerRightHand` (`Defs/BodyPartGroupDefs/BodyPartGroups_LowerHands.xml`) |
| Tools | two more fist attacks, `lower left fist` / `lower right fist`, linked to those groups |

**Why the body is a generated copy.** B&S ships `BS_FourArmedHuman` and
`BS_HumanoidWithWings_Body` as two separate defs, ships no four-armed *winged* one,
and declares its bodies with `<defName>` only - so `ParentName` XML inheritance
cannot merge them (the same trap that forced our trackers to be split, see the
header of `Races_InsectMamono_BS.xml`). `Tools/make_four_armed_winged_body.py` therefore
copies their part tree and inserts the wings, and the result is committed as plain
XML. Re-run it if B&S ever changes their bodies; the copy will not follow on its
own. The script finds the framework's own folder under Steam (the workshop copy
first, then local `Mods/` folders) and prints which copy it read; set `PMM_BS_RACES`
to choose one by hand.

**Why the lower hands got groups of their own.** Big & Small leaves both pairs in
vanilla's `LeftHand` / `RightHand`, and vanilla decides whether a melee tool works
by the parts in its group - so with their layout, losing the upper arms would
silence the lower fists as well. Four arms should fail separately, so the lower
five fingers each point at our groups and the lower fists follow them. Nothing
else reads those groups; `PawnCapacityWorker_Manipulation` is limb *efficiency*
(best limb set), not a sum, so the second pair adds redundancy rather than
manipulation.

**The wings' coverage had to come down (same day).** B&S gives each wing coverage
0.08, which their two-armed body can afford: its torso children total about 93%. Four
arms already spend 93% by themselves, so copying the wings at 0.08 took her torso to
**109%** - and `BodyDef.ConfigErrors` warns about that in dev mode (harmless, warning
only, but it filled Player.log). `Tools/make_four_armed_winged_body.py` now writes 0.02 per
wing and *checks* every record's coverage sum, failing rather than letting a future
edit cross 100% again. Current worst: 97%, the torso.

**The art is a placeholder, and this is the part that still needs a human.**
Humanlike arms live in the body texture, so B&S's own four-armed race draws two
arms as well, and no installed mod ships a four-arm or abdomen sprite. What exists
now is placeholder art - six crude grey limbs in
`Textures/RaceDefaults/PMM_AbaddonFolk/PMM_LowerArms_{south,north,east}[m].png`,
512x512, drawn in near-white because the render node tints them with the pawn's
skin colour (`colorType Skin`), so her body colour applies for free. Replace those
six files with real art, names unchanged, and nothing in XML moves.

The node itself is on `PMM_RaceTracker_AbaddonFolk` and is a copy of B&S's own
tail node (`1.6/SimplyRaces/Defs/Races/TailedHuman/RaceTracker_TailedHuman.xml`) -
`PawnRenderingProps_Ultimate` + `PawnRenderNode_Ultimate`, `parentTagDef Body`,
`shader Cutout`, per-direction sprites (east serves west through the `m` mirror).
Two knobs will probably want tuning once it can be seen, because art cannot be
judged in XML: the layers (2 = over the body, under clothing; -1 for the north
view) and the `drawSize` (0.8,0.8) copied from the tail.

**One deliberate gap: no hide-when-destroyed alt.** B&S matches a part trigger by
`BodyPartDef` alone (`ConditionalGraphic.PartRecord` carries only bodyPartDef,
mirrored, partMissing, replacement/implant/hediff flags - no custom labels), and
this body has four parts of the def `Arm`. A `partMissing` alt on `Arm` would
blank the lower arms when an *upper* arm is lost. Always drawn beats wrong here.

**Open ends.** Her xenotype icon is the queen's crest as a placeholder
(`Defs/XenotypeDefs/Xenotypes_Insect.xml`) until she has art of her own, and the
arms are drawn with placeholder art. The abdomen the user asked about is **not
built** - the call on 2026-09-25 was "just do the arms for now", so no abdomen
body part, no gut inside it and no sprite exist. `BS_SpiderAbdomen` /
`BS_SpiderHybrid` are the references if it is ever wanted: a 60 HP skin-covered
part, placed in B&S's spider body as a child of the torso at `height Bottom`, and
in their version the stomach lives inside it. See §11 item 9.
The queen's own abdomen needs none of that: she takes hers from VRE's cosmetic
spawning-sack gene (§5.10), because hers is drawn art rather than a body part. A folk
version would still need the part, the gut inside it and the sprite.

**The lower arms had to move above the wings (2026-10-01).** She was drawn with the left lower arm
behind her front wing and the right one in front of it. The wings are B&S's own nodes, whose front
faces sit at layer 92 and whose inner ones sit at -1 and -2, while the arm node was at 2 - the value
copied from B&S's tail. It is at 93 now, above the wings, with the north view left at -1, where her
own back and the wings are meant to hide the middle of the pair. Anything above the wings is above
her clothes too: the two cannot be separated without copying B&S's winged tracker, the same
copy-or-patch trade `SpiderBody_BS.xml` records for the spider body. Her queen is not covered by
this: she wears the same body but has no lower-arm node of her own, so if her four arms ever read
wrong, that is a different look.

### 5.8 Her size: two frameworks, both ADDING (2026-09-25)

The first pass at her size was wrong and the user caught it in game: he read **0.36**
on the pawn where the design wanted 0.5. Where that number comes from, read out of the
two assemblies rather than guessed:

| Layer | Contribution | Their 1.6 code |
|---|---|---|
| Vanilla | `baseBodySize 0.9` x `CurLifeStage.bodySizeFactor 1` = 0.90 | `Pawn.BodySize` |
| Big & Small | `(0.9 x SM_BodySizeMultiplier) - 0.9` = -0.09 at 0.9 | `Pawn_BodySize` postfix adds `cache.totalSizeOffset`; `BSCache.CalculateSize` builds it as `(baseBodySize + offset) x SM x bodySizeFactor - baseBodySize` |
| VEF, from `VRE_Microsized` | `VEF_BodySize_Offset -0.45`, **added flat** | `VanillaExpandedFramework_Pawn_BodySize` postfix adds `cache.bodySizeOffset` |
| | **0.36** | |

So `VEF_BodySize_Offset` is an offset, not a fraction, and "microsized takes 45% off"
was my error - VEF's own stat description ("+1 roughly doubles size") reads that way,
the code does not. The fix was to make the B&S half ask for more:
`SM_BodySizeMultiplier` 0.9 -> **1.0556**, since `0.9 x 1.0556 = 0.95` and
`0.95 - 0.45 = 0.50`.

`SM_BodySizeMultiplier` is also the stat B&S's scaler reads for how large a humanlike is
**drawn** (`baseBodySize` is not), so she draws about 17% bigger than before as well as
measuring 0.5. That one line is the knob if the drawing still reads wrong.

### 5.9 The tracker row stays - hiding it was tried and reverted (2026-09-26)

The two blue rows in her Health tab - "Winged humanoid" (`BS_HumanoidWithWings_Race`) and
"Abaddon folk" (`PMM_RaceTracker_AbaddonFolk`) - were hidden for an hour. Core's
`RaceTrackerRowHidden.cs` patched `BigAndSmall.RaceTracker.Visible` (hardcoded `=> true`) to
return false for `PMM_Race_*` pawns, and the rows did vanish. So did the wings. B&S draws a
tracker's art through the hediff's own render nodes and only installs them while the hediff is
`Visible`, so the row and the art are one switch - the patch is deleted and its changelog line
with it, and the race defs were never touched.

That is the answer to "can we just hide those rows": not without moving every tracker's render
nodes onto some other, visible hediff, which would put a row straight back. §5.7 still stands -
the winged tracker's second row is what draws her wings.

### 5.10 The abaddon queen's rework: four arms and a new package (2026-09-26)

The user's second pass on the queen, in two parts.

**Four arms.** She now runs the same `PMM_Body_FourArmedWinged` body as her soldier
daughters, so the two abaddon species share one body def - renamed from
`PMM_Body_AbaddonFolk` (and `Tools/make_abaddon_folk_body.py` renamed with it) when the
queen joined, because a body named for the folk would have been a lie in her race def.
Her melee tools are unchanged: the user's call was four arms with the second pair
structural only, so no lower fists exist to dilute the weighted pick between her fists
and her 30-power claw.

**Her gene package**, straight from the user's list: `VRE_SwarmSynapse`,
`VRE_Parthenogenesis` and `VRE_SlowedLifeCycle` as the gifts, `VRE_Colossal`,
`VRE_Dormant`, `VRE_InefficientMidgut` and `VRE_HypothermicHibernation` as the costs,
plus `VRE_InsectSkin`, `VRE_InsectAntennae` and her jelly dependency. Out:
`VRE_ProteinDenaturation` (sunlight burned her) and `AptitudeRemarkable_Social` - the
user's call was that the list is the whole package, and her strong-intellect aptitude
stays.

**And the fertility question answered itself.** `VRE_Parthenogenesis` was on that list, and
it held the `Fertility` exclusion tag - which is why the real spawning sack could never
join it. The user first chose parthenogenesis over the sack, then on 2026-09-26 traded
parthenogenesis itself out for `VRE_RobustMidgut`, evolution for evolution so the 4/4 holds -
raw food feeds her at 1.8x and never sickens her.

**And then closed it.** With parthenogenesis gone she would have conceived the ordinary way,
and that is not what the user wants: more abaddons should mean more egg sacs, not more
pregnancies. So she took vanilla's **`Sterile`** gene (Fertility x0, `ParentName
FertilityBase`, 1 metabolism). One line in the xenotype, no race-level stat, and the gene list
says why she is barren. It costs the 4/4 nothing, because `Sterile` is a plain vanilla
`GeneDef` rather than one of VRE's pools - same as her jelly dependency and her skin genes.
Two consequences worth knowing: it explains itself to the player, and it takes the shared
`Fertility` tag back from parthenogenesis, so `VRE_SpawningSack` is tag-blocked again exactly
as it was before (see the abdomen note). None of this touches her egg sac, which is our own
ability on a button rather than a pregnancy.

**And the gene was not enough.** The gene alone did not do it in game: the stat panel listed
`Sterile` at "x0%" while her Fertility read 100%. Two of my answers to that were wrong and the
doc keeps the corrections rather than the guesses. Guess one - a stale gene list, since a pawn's
genes are saved with her - the user shot down: the pawn was spawned after the change, on a fresh
game. Guess two - the gene overridden by another gene sharing Biotech's `Fertility` exclusion tag,
which is how one gene knocks out another - fell to a scan of every def in the install: the only
genes holding that tag are the abstract `FertilityBase` that `Sterile` inherits from, VRE's
`VRE_SpawningSack` and `VRE_Parthenogenesis`, and one VRE mutation. None of them is in her set;
her cosmetic sack holds `Tail` alone. My third try, `<Fertility>0</Fertility>` on her race def's
`statBases`, is dropped too - and the reason I gave for it was wrong, corrected by the user the same
day: I read the info card header ("Human, Healer") as her race and wrote that down as fact. The
first token there is the pawn's **name**; "Human" is simply what she is called, and the pawn being
looked at may very well be the queen herself. Nothing in the fix rests on that reading: the hediff
reaches a queen from her gene **and** from her race comp.

**The real cause, which took four passes to find, was our own core mod.** `Project
Mamono/Source/ProjectMamono/MamonoFertilityPatch.cs` protects every Mamono on purpose: a postfix on
`StatPart_FertilityByGenderAge.TransformValue` restores her pre-age fertility, a postfix on
`StatExtension.GetStatValue` floors an adult Mamono's Fertility at 1.0 so that - in its own words -
"no source - sterilized, fertility-drained or removed ovaries, gene/trait offsets, or age - can push
her below 100%", and a postfix on `Pawn.Sterile()` reports adult Mamonos as not sterile. The abaddon is
a Mamono, so every x0 in her gene list and every hediff factor was being clipped by our own code,
exactly as designed. That is why the panel listed both "x0%" lines and still read 100%, and why
nothing the Insects mod could do was ever going to show. The habit worth keeping: when a vanilla
mechanism "does nothing", grep our own Source for a patch on it before theorising about the engine.

The fix is one exception in that file, `DeliberatelySterile`, built from vanilla's own other two
sterility sources - `HediffSet.HasHediffPreventsPregnancy()` and `GeneUtility.SterileGenes`, the pair
`Pawn.Sterile()` itself checks. She has that gene, so all three core
layers now skip her and vanilla answers on its own: Fertility 0%, `Sterile()` true, no pregnancy.

**What the 1.6 assembly says**, kept because it settles why both factors showed in the tooltip and
neither reached the number: `StatWorker.GetValueUnfinalized`
multiplies `def.statFactors` for every *active* gene, and `Fertility` has no post-process curve, no
max value, and exactly two parts, both of them multipliers - so a live x0 factor cannot leave the
value at 100%.

**And then the fix shrank to nothing, in the same pass.** The hidden hediff
(`Defs/HediffDefs/Hediffs_Insect.xml`, and that folder with it), the `Sterile` gene-class patch, the
gene class, and the two delivery routes behind it (the `AddGene`/`Tick` hooks and the race comp) are
all deleted, because the core exception above made every one of them pointless: a gene with
`sterilize` is enough by itself. Her lock today is one line of vanilla data - `Sterile` in her
xenotype - which holds the shared `Fertility` tag, pays 1 metabolism, and multiplies her Fertility
by 0 the moment the core protection steps aside. One consequence worth writing down, since that
hediff did ship for a few hours: the first load of a save that already has it logs a single "could
not find def" error for `PMM_InsectSterility` and drops it, and no later load sees it.

**Her third pass, same day: the voice and the swap.** Two changes, both in the
xenotype's gene list only. In: `VRE_VocalGlands`, the vocal organ that emulates a human
voice ("a melodic tone that for most people can be described as beautiful"), worth
Talking +0.25, SocialImpact +0.25, NegotiationAbility +0.4 and TradePriceImprovement
0.1 - a queen who commands her swarm by pheromone and by voice. Out:
`VRE_InefficientMidgut`, the permanent 2.5x hunger rate and its extra stomach room.
Back in: `VRE_HardLockedJoints`, which she carried before this rework and lost in the
first pass - MoveSpeed -0.4 against her race's 4.0. She now pays for her size with a
slow walk instead of a bottomless stomach. By VRE's pools `VRE_VocalGlands` is an
evolution and `VRE_HardLockedJoints` a mutation (pherocore-locked, read out of
`GeneDefs_Evolutions_VFEInsectoids.xml` / `_Mutations_VFEInsectoids.xml`; those live in
the nested submod folder inside the Insector mod, `1.6/Mods/VFEInsectoids/`), so the
package sits at §5.3's four and four - the balance needed one more evolution, and the
voice is it.

Size, for the record: `VRE_Colossal` adds a positive `VEF_BodySize_Offset` on top of
her `SM_BodySizeMultiplier` 2.0, which by the B&S maths in §5.8 already makes her Body
size 4.0.

**Measured, then retuned (2026-09-26).** The user read 4.65 off her in game, which
confirms the whole model end to end: B&S multiplies `SM_BodySizeMultiplier` by
`baseBodySize` (2.0 x 2.0 = 4.0), and VRE_Colossal's `VEF_BodySize_Offset` is a flat
**+0.65** added afterwards, giving 4.65 exactly. Her race pair is now
**1.1619 / 1.1619**, so 1.1619 x 1.1619 + 0.65 = **2.00**, which is what the user asked
for. Both numbers move together because they are kept equal across this whole file.
B&S draws a humanlike from `SM_BodySizeMultiplier` and not from `baseBodySize`, so she
also *looks* smaller now; if she ever reads too small for a queen, the dial is the
cosmetic size stat, and touching this pair would move the mechanic as well.

**Her abdomen came back as looks only.** The real spawning sack is out on a tag, as it
always was: `Sterile` holds the shared `Fertility` tag now that parthenogenesis has gone.
VRE ships the visual on its own anyway: `VRE_SpawningSack_Cosmetic` is the same
attachment node, the same texture (`Things/Pawn/Humanlike/BodyAttachments/SpawningSack`)
and the same `colorType Skin`, so it takes her body colour - with no stats and no
`Fertility` tag. So the answer to "can we get the abdomen without the gene" was that no
copying was needed: VRE had already split the look from the mechanic. Its one exclusion
tag is `Tail` (it would clash with a tail gene, and she has none). The node is a plain
vanilla `GeneDef.renderNodeProperties` attachment, tuned by VRE for a human-sized pawn;
if it reads wrong on a queen this size, the lever is a PatchOperation on their node or
our own copy of it (with `overrideMeshSize`/offsets of our own), not anything her race
def can reach.

### 5.11 The brood picker - the queen chooses what she lays (started 2026-09-26)

The ask: a UI element on the abaddon's egg spew that decides which insect mamono comes out of
her egg. Five phases; phase 1 is done.

**Phase 1, done - the order itself.** `Source/Insects/BroodOrder.cs` holds
`CompProperties_BroodOrder` + `CompBroodOrder`, attached to the abaddon's race def so it
reaches a queen however she arrived. The caste list lives in XML
(`<broodOptions>`), and the gizmo is a `Command_Action` opening a `FloatMenu`, each entry
carrying the caste's own **xenotype** icon - `PawnKindDef.xenotypeSet[0]`, which works because
`XenotypeSet` exposes its private `xenotypeChances` list through a public indexer and `Count`
(verified in Assembly-CSharp, not assumed). Two scribes, the def and the random flag, rather
than an option index: reordering `<broodOptions>` later cannot silently change what an
existing save's queen lays.

The gizmo is offered only to a queen of the player's faction who actually has the ability.
The ability is looked up by name (`DefDatabase<AbilityDef>.GetNamedSilentFail`) rather than
through a `[DefOf]` field, because a `[DefOf]` field pointing at a def that may not be loaded is a
load error when that DLC is absent, and the same guard is why the comp stays quiet instead of
throwing. The gate is `pawn.abilities.GetAbility(...)` - `Pawn_AbilityTracker` has no
`HasAbility`, which the first compile found for us.

**Phases 3 and 4, done 2026-09-26 - the hatch follows the order.** `Source/Insects/EggSacBrood.cs`
holds both halves. `Projectile_EggSac : Projectile_SpawnsThing` works out the landing cell first
with vanilla's own rule (impact cell, or the first free standable neighbour when the cell is
taken and `tryAdjacentFreeSpaces` is on), calls `base.Impact` - vanilla still does the spawning
and the damage - and then picks the sac up from that cell by its comp
(`loc.GetFirstThingWithComp<CompEggSacBrood>`) to write the order on it. `CompEggSacBrood :
CompSpawnPawnOnDestroyed` derives from vanilla so the def keeps `pawnKind`/`lordJob`, and
branches in `PostDestroy`: a **non-humanlike** brood (a swarmling, or a sac saved before the
picker) calls `base.PostDestroy` and gets vanilla's hatch untouched - age 0, flyer hop, nest
lord; a **mamono** is generated by us at age 3 with `allowDowned: true`, and stands up where the
sac was with no hop and no lord. Her age is a props field, not a constant
(`<biologicalAge>`, vanilla has no such field): the agreed value is 3, and it is set to 0 in the
XML right now because the user asked for a newborn on 2026-09-26 while testing the hatch - one
line, no rebuild. The order is scribed on the sac, so a save/reload in flight or
with a sac sitting on the map still hatches the right caste, and a sac nobody told anything
falls back to the def's `pawnKind`. She is also the queen's **daughter**: the sac carries the\nlauncher, and the hatch adds `PawnRelationDefOf.Parent` on the child only. `Child`\n("son"/"daughter") is an **implied** def, and `AddDirectRelation` refuses implied defs outright\n(`if (def.implied) -> Log.Warning and return`), so writing the child's side is the whole job -\nand it is what vanilla's own birth code does in `PregnancyUtility`, for the genetic mother and\nthe father both. The mother's side reads \"daughter\" on its own.\n\nHer **father** comes from the queen's own relations, since an egg has nobody but her mother (a\npregnancy records its father at conception; nothing here does). Order: the core mod's **tsugai\nbond** first - `ProjectMamono_DefOf.ProjectMamono_Tsugai`, a `PawnRelationDef` whose own `importance`\nis 210 against vanilla's spouse at 200, and the lore is that a bond is for life - then spouse,\nfiance, lover. All male: `Parent` is labelled by gender, so a woman partner would be written down\nas a second mother, and `ChildRelationUtility.ChanceOfBecomingChildOf` logs a warning when handed a\nnon-male \"father\". Ex-partners are off the list. The bond partner must be *alive*, which is the core\nmod's own rule for the bond (`TsugaiFormation.HasBondedPartner` asks for `!p.Dead`; a partner's\ndeath severs the bond hediff and leaves grief), while a dead husband stays a husband, because that\nis what vanilla leaves behind.\n\nThe naming window comes with her. Vanilla's factory `PawnNamingUtility.NamePawnDialog` decides\nwhat is editable by testing `Pawn.babyNamingDeadline`, so the dialog is built directly\n(`new Dialog_NamePawn(child, names, names, suggested)`) and that deadline - which vanilla's birth\ncode owns and other systems read - is left alone. First, nick and last name are all editable, the\nqueen's own family name is offered as a suggestion, and the window's description reads \"Mother:\n<queen>\" for free because the relation was added a line earlier. Only for a hatch on the map the\nplayer is looking at, and never for a nest queen's brood.

What the tooltip promised is now true, so the CHANGELOG line went in with this pass. What is
left of the plan is the optional cocoon (item 5).

**Revision the same day, after the first in-game test.** The gizmo did not appear for the
user, and the log was clean (no XML or class errors, no exception), so the comp was loaded
and parsed and one of the comp's own guards was hiding it. That guard was the ability: it
used to come from her pawn kind, so a queen who arrived by transformation had none. Fixed
by having the comp **grant the ability itself** in `PostSpawnSetup`, and the kind's
`<abilities>` entry is deleted with it so there is one source that covers both paths. The
faction check now leaves the gizmo **visible but disabled** with a reason instead of hiding
it: a picker that silently vanishes reads as a bug, and an order on somebody else's queen
is pointless while nothing casts the ability for her.

One thing that was *not* the cause, and is worth keeping: **ThingComps are not saved at
all.** `ThingWithComps.ExposeData` calls `InitializeComps()` itself when
`Scribe.mode == LoadingVars` and then only scribes each comp's own state - the save file has
no `<comps>` node anywhere. So comps are rebuilt from the def on every load, and adding a
comp to a race def reaches pawns that already exist as soon as the game is restarted.

**Two more egg-spew changes, same day (2026-09-26).** Both the user's call, and together they left it
a plain sac throw. The cooldown went from 5000 ticks - about two hours - to 900000, fifteen days,
because the sac is now the only throttle on abaddon numbers. And the acid half is gone: the DLC's
`EggSpew` sprays 18 cells of sludge for 18 AcidBurn and leaves `InsectSludge` terrain, and ours now
drops the `CompProperties_AbilitySprayLiquid` comp - along with the clone built a few minutes earlier
to redirect that terrain to creep. Nothing burns, nothing is slimed, no terrain is laid. The range
came in with it, 14.9 cells to 1.9: the ring immediately around her, which is what "domestic" meant
here. `Things_EggSpew.xml` is back to two ThingDefs, the sac projectile and the sac. The DLC's
`AcidSpray` warm-up and cast sounds stay, because vanilla's own EggSpew plays exactly that pair.

**And then the two buttons became one (same day, the user's call).** The brood picker was a second
gizmo beside the egg spew button, and both described one act, so the picker is gone. The ability def
now carries `<gizmoClass>PMM_Insects.Command_Ability_EggSpew</gizmoClass>` -
`AbilityDef.gizmoClass` is a real field, and vanilla's own `Command_AbilitySpeech` is the pattern
(same `(Ability, Pawn)` constructor, same `Tooltip` override) - and our command opens the caste menu
first, then hands over to `Command_Ability.ProcessInput`, which is where vanilla starts the targeting
cursor from `ability.verb`. Order of play: press, choose the daughter, pick the ground. Choosing and
then cancelling the cursor keeps the new order and spends no cooldown, because a cast is what starts
the cooldown. `CompBroodOrder` keeps the order and builds the menu; it no longer yields a gizmo, and
its `CurrentLabel` now feeds the button's tooltip - the old picker label doing the same job. The
disabled-with-a-reason case for somebody else's queen moved onto the button with it.

**Arachne, the spider mamono (from 2026-09-26):** her plan is finished and archived at
`archive/ARACHNE-PLAN.md` (2026-10-01) - a body from Big & Small's own spider, tribe spawns, the brood
menu, silk, web and coffee. Everything in it is built and tested; the file is kept for its evidence.

**Still to come, agreed with the user 2026-09-26:**

2. **Done 2026-09-26.** Her `CompProperties_SpreadSludge` block is deleted and the ability now
   carries `aiCanUse false`, so nothing casts it on its own. That comp was the *only* caster -
   `CompSpreadSludge.CompTick` goes straight to `ability.verb.TryStartCastOn` with no faction
   check and no `aiCanUse` check - so the deletion was the real fix and the `aiCanUse` flip is
   the belt to its braces. The greenworm's identically shaped comp points at `SludgeSpew` and
   stays: no brood in it. What the queen loses with it is the ambient acid she used to spread
   while laying - deliberate, and the user's call was "fully manual".
3. **Done 2026-09-26** with phase 4, in `Source/Insects/EggSacBrood.cs`. The ruling that was
   open here is settled too: the ability is now granted by her race comp (see the revision note
   above), so a woman transformed into a queen can breed and the pawn kind's `<abilities>`
   entry is deleted with it.
4. **Done 2026-09-26.** `CompEggSacBrood`: the chosen caste, age 3, `allowDowned: true`, no
   nest lord for a humanlike. Vanilla's `CompSpawnPawnOnDestroyed.PostDestroy` hardcodes
   `fixedBiologicalAge = 0f` and `allowDowned: false`, which is why this had to be our own comp
   and not a def tweak - that pair is the 2026-09-20 "Generated downed pawn" failure.
5. **Open, and the user's call.** Optional cocoon stage: the sac spawns our own cocoon building
   carrying the order, and a killed cocoon loses the brood (their ruling). Recommendation on the
   record: skip it. It buys the metamorphosis beat and costs a building def, a comp, art, a
   fourth hop for the order and a second killability rule - and the chain is at two hops today.

**The finding that shaped the plan.** Today nothing about the brood is choosable, and no mamono
comes out of the egg either: the sac hatches a `VFEI2_Swarmling`, and VFEI2's
`CompSwarmlingToCocoon` (internal) later swaps her for the `VFEI2_InsectoidCocoon` building,
whose `VFEInsectoids.Cocoon` class **hardcodes** its table in C# - a public `array` of
megascarab 0.5 / spelopede 0.25 / megaspider 0.15 / megapede 0.05 - and spawns one at
`AgeBiologicalTicks = 30000`. There is no XML hook for "what comes out", which is why the
choice has to be carried by our own comps and the default entry is left to VFEI2 untouched.

### 5.12 The honey bee, and honey - built 2026-09-27

**The caste.** `PMM_InsectHoneyBee` xenotype, `PMM_Race_HoneyBee` race plus tracker,
`PMM_InsectHoneyBee` pawn kind, and her own icon - hand-drawn art since 2026-09-29
(`Textures/UI/Icons/Xenotypes/PMM_HoneyBee.png`, 64x64 as the reptiles' icons are). She is
the neutral hive's alone: `Peaceful` weight 12, `Settlement` weight 3, the `Farmers` role group
at weight 2 (the heaviest of the three there - she is the caste that farms), and no Combat group
anywhere, so the swarm never fields her. The hive's `xenotypeSet` was rebalanced for her: devil
bug 0.28, giant ant 0.28, soldier beetle 0.20, ant arachne 0.12, honey bee 0.12.

Her genes are the user's list, every def checked against the installed 1.6 files: `VRE_Serotonin`
and `VRE_CompoundEyes` (both evolutions) against `VRE_RapidLifeCycle` and `VRE_Microsized` (both
degrades) - two and two, the balance VRE's picker demands - plus `VRE_InsectAntennae`, the core
mod's `PMM_Gene_FlightWeak`, `Sterile`, `AptitudeStrong_Plants`, `AptitudeStrong_Animals`,
`AptitudePoor_Intellectual`, `AptitudePoor_Social`, `PMM_Gene_Honey`, and the body colour she
rolls (§5.13).
The trade that comes with the list, on the record: serotonin keeps her blissful AND doubles her
social-fight chance (`socialFightChanceFactor 2`, and its own description promises as much), so a
hive of farmers is a hive of cheerful squabbles.

**She is the one caste with no bug to copy.** There is no bee in vanilla, in VFEI2 or in VRE
Insector (checked 2026-09-27), so her numbers are set by comparison with her sisters rather than
overlaid 1:1 from a bug as every older caste's were: body-size pair 1.0 x 1.0 (0.55 after
microsized), health scale 0.4 (x2.5 in `HealthScalePatch`, so 1.0 effective - the devil bug's
frame), hunger 0.15, market value 150, light armor at 0.10 sharp / 0.05 blunt, and the devil
bug's worker tool set with **no sting**, because nothing in the user's list adds a weapon.

**The honey gene.** `PMM_Gene_Honey` (`Defs/GeneDefs/Genes_Honey.xml`): a `GeneDef` with
`geneClass PMM_Insects.Gene_Honey` and a `PMM_Insects.HoneyGeneExtension` for the dials, because a
`GeneDef` has no comps field - the same reason core's age-ailment marker is an extension. **The
state lives on the gene, not on the race**, so a second honey-making caste is one gene in her
xenotype: the work giver, the right-click order and the gizmo all find her through
`Gene_Honey.Get(pawn)` and never through a race name, which is the rule the silk comp states for
itself.

**Accumulation is work-driven, the one place honey differs from silk.** Silk fills on the clock;
honey fills from what she does. `Gene_Honey.Tick` polls once an in-game hour
(`IsHashIntervalTick(2500)`, the core gene's own cadence) and reads three vanilla records:
`PlantsSown`, `PlantsHarvested`, `AnimalsTamed` - verified in
`Data/Core/Defs/Misc/RecordDefs/Records_Misc.xml` on 2026-09-27. The plan also named milking
animals, and **that record does not exist**: vanilla's list holds `AnimalsTamed` and
`AnimalsSlaughtered` and nothing finer, so animal work counts through taming alone. The records
are looked up BY NAME, so one missing from this load order is simply not tracked, and no Harmony
patch is needed anywhere in the accumulation. The first reading of a save-loaded pawn only
baselines (`lastWorkTotal` starts at -1 and is scribed): she already carries a lifetime of farm
work on her records, and paying for that would hand the player a full bar the moment she loads.

**The dials**, all on the extension so each is one line to move: `fullnessPerAction 0.024` (a full
bar is about 42 tracked actions), `honeyPerMilking 12`, `minFullness 0.75`. Fullness clamps at 1,
so honey caps as silk does, and milking empties her with the yield scaled to what had built up -
early is a shorter wait for less honey, never a loss. These are the plan's open tuning questions,
answered as defaults rather than by asking.

**The emptying point, raised 2026-09-29 (the user's call).** `minFullness` was 0.3. The user asked
for honey that drops by itself once the bar is full; shown how the chain works, they took the
smaller change instead - **no automatic drop**, and the emptying point raised to three quarters so
one milking takes more at once. The dial is shared, so the right-click order and her gizmo wait the
same three quarters, and `WorkGiver_MilkHoney` still empties her the moment she passes it wherever
Handling is on - which is what made the automatic drop unnecessary. The trade accepted with it is
the wait: fewer milkings, each one bigger than the early trickle it replaced.

**The yield halved and the fill slowed, same day (the user's call).** "Halve the amount of honey
made, and make the amount of work needed increase by 25%": `honeyPerMilking` 25 -> 12 (25 does not
halve into whole jars, so it is the rounded-down half) and `fullnessPerAction` 0.03 -> 0.024, which
is 0.03 over 1.25. The two multiply: honey per tracked action falls to about 38% of its old rate
(the fill rate 80% times the yield 48%), so a jar costs roughly 2.7 times the work it did. Nothing
else moved - the item, its stack of 25 and its value are MO's numbers and unchanged.

**The chain is silk's chain**: a `PMM_MilkHoney` job (`JobDriver_MilkHoney`), a matching
`PMM_MilkHoney` work giver (`WorkGiver_MilkHoney`, self-only like the silk giver, so she takes her
own and the player's right-click covers everyone else), a right-click order
(`HoneyFloatMenuPatch`) and a gizmo. **Since 2026-10-01 the two features share that whole chain**:
the order, the driver and the work giver are written once in `Source/Insects/PawnHarvest.cs`, and
all a feature supplies is an `IPawnHarvest` implementation - the comp for silk, the gene for honey
- plus which job to start. The shared work giver also hands the pawn to the scan itself through
`PotentialWorkThingsGlobal`, with a request group of `Undefined`, which is vanilla's own shape for a
self-only giver (`WorkGiver_TendSelf`, behind the `DoctorTendToSelf` def); the version before that
asked for `ThingRequestGroup.Pawn` instead, which worked but made every scan walk the pawns of the
map to find her. The gizmo is the one
thing silk does not have: `Verse.Gene.GetGizmos` is the engine's own hook, so the order sits on
her own bar with no patch - note that its base implementation returns **null** rather than an
empty list, so nothing calls base. Work type `Handling` rather than the silk giver's `Hauling`:
vanilla files the milking of animals under Handling, and this is the caste the user gave the
animal talent to. `InsectDefOf` moved to its own file (`Source/Insects/InsectDefOf.cs`) because it
now holds two job defs and stopped belonging in the silk file.

**Translation, both chains.** The C# strings live in `Languages/English/Keyed/Silk.xml` and
`Honey.xml` - three keys each: the order's label, the "none yet" suffix, and the fullness reading.
The job's report string and the work giver's label, verb and gerund are def fields, so they live
inline in the defs, as every other def string in this mod does: no mod in the family ships a
`DefInjected` folder. A translation therefore adds `Languages/<language>/DefInjected/JobDef/` and
`.../WorkGiverDef/` files for the def-side text, and Keyed files for the C# side.

**The item.** `PMM_Honey` (`Defs/ThingDefs/Things_Honey.xml`): vanilla insect jelly's parent and
shape, with market value 7, mass 0.025, nutrition 0.05, a stack of 25, the `AnimalProductRaw`
shelf, and **no `FoodPoisonChanceFixedHuman` line at all** - that stat's default is 0, so the
plan's "never poisons anyone" is reached by omission rather than by a field that says 0. Value,
nutrition and stack match Medieval Overhaul's own honey, so a colony running MO sees one honey
either way. The stack of 25 is MO's number rather than a match for one milking, which has been 12
since 2026-09-29. It is tradeable (vanilla's default), which
the MO patch's reasoning needs: that patch stops our jar being *generated* as trader stock, never
the player selling one.

**Medieval Overhaul compat** (`Patches/Honey_MedievalOverhaul.xml`, gated on MO's display name
the way the silk patch is). MO is workshop `3219596926` and its honey item is `DankPyon_Honey`, a
food on the same vanilla parent as ours. With MO loaded the gene's `honeyDef` is re-pointed to it
- remove plus add, because the element ships in our own gene def - and `PMM_Honey` is added to
the `excludedThingDefs` of every `StockGenerator_Category` that stocks `AnimalProductRaw`, with a
second op carrying the `[not(excludedThingDefs)]` predicate for the generators that never wrote
the list. `tradeability` is left alone, the arachne's reasoning. The name the player reads then is
MO's own: "smokey honey".

**Also in this pass:** the four castes built after `InsectMamonoCorpses.cs` was written - the
arachne, the beelzebub, the girtablilu and the ant arachne - were never registered in it, so their
corpses sat under vanilla's humanlike line while every older sister's moved to the mamono line. All
four are registered now, the bee with them.

### 5.13 Body colour is rolled from her caste's three (2026-09-30)

Every caste has three body colours and every mamono wears one of them, rolled for her. The colours
are 33 gene defs in `Defs/GeneDefs/Genes_InsectSkin.xml`, three per caste, with the arachne and the
ant arachne sharing a row; which three belong to whom is declared on the caste herself, as a
`PMM_Insects.InsectSkinColours` list inside her xenotype's `modExtensions`.

**Why a gene at all.** `Pawn_StoryTracker.SkinColor` is `skinColorOverride ?? SkinColorBase`, and
`skinColorOverride` is only ever written from a gene (`Pawn_GeneTracker.Notify_GenesChanged`), so a
colour has to be a gene to be a colour. That also means it is saved with her, and that it shows as
one row on her gene page - the row names her shade ("deep indigo chitin", "bright yellow chitin").

**Why a roll and not a list.** One `skinColorOverride` is one fixed RGB, so three colours need three
defs - and a xenotype listing all three would show two greyed rows, because vanilla keeps one active
and overrides the rest. Vanilla answers the same problem with per-pawn melanin genes, and so does
this, in `Source/Insects/InsectChitin.cs`:

- `Gene_InsectChitin` on `PMM_Gene_Insect` rolls when the gene lands, so ordinary generation, a
  woman transformed into a caste, and a daughter inheriting her mother's colour all pass through it.
- `PawnGenerator.GeneratePawn` rolls once more at the end of generation, so a daughter wears her own
  colour rather than her mother's.
- `Pawn_GeneTracker.ExposeData` does **not** call `PostAdd`, and the load hook only fills a gap, so a
  mamono loaded from a save keeps the colour she was saved with.
- `Pawn_GeneTracker.SetXenotype` sets `xenotype` *before* it adds that xenotype's genes through
  `AddGene` (checked in the 1.6 assembly), so the roll already knows which list to draw from - and a
  woman re-cast in game has her old colour replaced by one of her new caste's.

**The five shared tones are gone** (pale, olive, moss, umber, dark: 2026-09-29 to 2026-09-30, rolled
for every caste alike). A save made while they existed loses the gene reference once per mamono - the
removed defs mean the game logs it and drops it - and the load hook then hands her a colour from her
caste's palette, so she comes back in her caste's colours rather than staying untoned.

**A caste with no list keeps the shared base.** `PMM_Gene_Insect` still carries
`skinColorBase (120, 118, 89)`, which is what shows for the hornet, the mantis, the mothman and the
papillon until each is given her three, and for a woman carrying the insect gene without one of the
caste xenotypes. `CASTES-PLAN.md` §2 carries "her three colours" as a build step for each new caste.

**And the colour tints the icon.** `GeneDef.IconColor` (`Verse/GeneDef.cs:220`) falls back through
`iconColor` -> `skinColorBase` -> `skinColorOverride` -> `hairColorOverride`, and its only reader
(`GeneUIUtility.cs:431`) multiplies that colour into the gene's icon. That is how vanilla's cosmetic
genes work: greyscale art, and the tint supplies the colour - which is why the colour genes and every
hair or skin gene in the load order read correctly. `PMM_Gene_Insect` is the one gene here that
declares a skin tone without being a skin gene, so its hand-drawn art came out multiplied by the
chitin base `(120, 118, 89)`: the heart's pink `208,85,148` rendered as `97,39,51` (user's report,
2026-09-30). It carries an explicit `<iconColor>(255,255,255)</iconColor>` now to opt out.
**A new gameplay gene with coloured art needs that line; a skin or hair gene must not have it.**

---

### 5.14 Grief leaves the gene's tooltip (2026-09-29)

The 2026-09-27 grief pass (`Patches/InsectGrief.xml`) wrote `PMM_Gene_Insect` into
`ThoughtDef.nullifyingGenes` on twenty-two relation thoughts. The nullification was right, the
display was not. That field is read in exactly two places: `ThoughtUtility.NullifyingGene`, which
is the nullification itself, and `GeneDef.GetDescriptionFull`, which walks every thought naming
the gene and prints the whole set on the **gene's own tooltip** - a `Mood:` block reading
`Removes: My daughter died: -23`, once per relation. The user hovered the insectoid gene, found
twenty-two lines of dead relatives, and asked for them hidden.

The field could not stay, so the same question is answered in code instead:

- The patch marks the same twenty-two thoughts with a `PMM_Insects.InsectGriefThought` mod
  extension (`PatchOperationAddModExtension`) in place of the `nullifyingGenes` list. A mod
  extension is read by nothing the game ships, so the tooltip has nothing to print.
- `Source/Insects/InsectGrief.cs` postfixes `ThoughtUtility.NullifyingGene` and answers with the
  pawn's `PMM_Gene_Insect` for a marked thought. Only marked thoughts reach the gene lookup, so
  every other thought on every other pawn costs one null mod-extension check.

`Thought_Memory.MoodOffset` reaches the nullification through `ThoughtUtility.ThoughtNullified`,
which asks `NullifyingGene` last, and the memory's own explanatory line comes from
`ThoughtNullifiedMessage`, which asks it too - so a single postfix serves both and neither result
changes. The memory is still gained, still worth nothing, and still reads "(Disabled by gene:
insectoid)". What moved is only where the relationship is written down: our code, rather than the
thought defs' data, which is the one thing to remember before putting the gene back into a
`nullifyingGenes` list.

---

### 5.15 The new castes' first two genes: light and quarrel (2026-09-30)

Phase 1 of `CASTES-PLAN.md`, built and deployed, with **no caste carrying either gene yet.** The
plan and the full gene study are `CASTES-PLAN.md` and `GENES-FEASIBILITY.md`; this section is what
exists. No language keys were needed: every string of both genes is inline in its def, the way the
mod's other gene labels are.

**The light gene** (`PMM_Gene_DisorientatingLights`, `Defs/GeneDefs/Genes_DisorientatingLights.xml`,
`Source/Insects/DisorientatingLights.cs`), written for the mothman: -5% consciousness and +10 mood
while she stands in man-made light at exactly 50%, and not under a growing lamp, with a glowing cave
counting as light.

The rule is one engine number and no def names. `GlowGrid.GroundGlowAt(cell, ignoreSky: true)` skips
the sky term, which is what makes the light artificial; what is left is capped at 0.5 for an ordinary
light source (`MaxGameGlowFromNonOverlitGroundLights`) and reads a flat 1.0 on every cell inside a
source's `overlightRadius` (`AlphaOfOverlit`). A vanilla `StandingLamp` has no overlight radius, so it
reads 0.5 and passes; the vanilla `SunLamp` - the growing lamp - has `overlightRadius 7.0`, so its
bright core reads 1.0 and is the one place the gene does not fire. The lamp's outer ring reads 0.5
like any lamp and does count, which is the 2026-09-30 call: the core is where the crops stand, and
excluding the ring would mean scanning the map's light sources on every check.

**The two halves have two homes, and that is the only interesting thing about the gene.** A hediff
cannot add mood - `HediffStage` has `overrideMoodBase`, which *replaces* the mood base - so the
consciousness penalty is the hidden marker hediff `PMM_Hediff_DisorientatingLights` (one `capMods`
stage, added and removed by the gene class on the honey gene's hourly poll) and the mood is the
situational thought `PMM_Thought_DisorientatingLights`. The hediff is this mod's first hidden
`Visible => false` hediff; the family's precedent is Reptiles' `Hediff_Shedding`, and the gene's
`description` is where the player reads the two numbers, because the effect box stays empty.

**The quarrel gene** (`PMM_Gene_Quarrelsome` for the beetle, `PMM_Gene_HoneyHatred` for the hornet,
`Defs/GeneDefs/Genes_Quarrel.xml`, `Source/Insects/Quarrel.cs`): -50 opinion toward the carriers of
one named gene, mutual for the beetle and one way for the hornet. The dial is the gene def's own
`QuarrelGeneExtension.dislikedGene`, so a third grudge is a third def and no new code of any kind.

Two things about it are worth remembering. The fights need no code at all: `Pawn_InteractionsTracker`
already multiplies the social-fight chance by opinion (x2.5 at -50, x4 at -100), so the opinion
penalty *is* the aggression. And a social thought is only collected once per other pawn when the def
names `thoughtClass Thought_SituationalSocial` - the default for a def with a worker and no duration
is `Thought_Situational`, which has no other pawn at all, so the penalty would silently never apply.

**Still to come:** the four castes themselves, phases 2 to 5 of the plan. The genes are in the def
database and show on a gene page in dev mode; nothing spawns with them yet.

---

### 5.16 The castes' body colours (2026-09-30)

The user supplied three colour values for each caste, and every caste now rolls one of hers. They are
recorded here exactly as given; the defs that carry them are in `Genes_InsectSkin.xml` and the roll
is §5.13.

| caste | value 1 | value 2 | value 3 |
|---|---|---|---|
| Giant ant | 50 49 80 `#323150` | 97 112 143 `#61708F` | 126 171 210 `#7EABD2` |
| Soldier beetle | 87 68 61 `#57443D` | 140 118 105 `#8C7669` | 48 30 28 `#301E1C` |
| Greenworm | 137 171 120 `#89AB78` | 97 135 84 `#618754` | 74 109 67 `#4A6D43` |
| Honey bee | 247 218 65 `#F7DA41` | 255 215 45 `#FFD72D` | 217 175 15 `#D9AF0F` |
| Girtablilu | 196 140 95 `#C48C5F` | 233 185 123 `#E9B97B` | 167 93 78 `#A75D4E` |
| Devil bug | 42 16 3 `#2A1003` | 134 99 61 `#86633D` | 185 145 94 `#B9915E` |
| Vamp mosquito | 43 39 38 `#2B2726` | 192 195 202 `#C0C3CA` | 107 103 102 `#6B6766` |
| Abaddon | 96 80 87 `#605057` | 114 104 105 `#726869` | 50 44 46 `#322C2E` |
| Abaddon folk | 114 93 98 `#725D62` | 94 71 79 `#5E474F` | 130 100 110 `#82646E` |
| Arachne, and the ant arachne | 152 144 185 `#9890B9` | 117 116 150 `#757496` | 51 47 72 `#332F48` |
| Beelzebub | 64 71 77 `#40474D` | 84 92 103 `#545C67` | 110 120 132 `#6E7884` |

**Each row is a set of three, not a ramp.** One of the three is picked for each mamono at random
(the user's ruling, 2026-09-30), so no value is a shadow and none is a highlight - the order the
values are written in means nothing. The table keeps the order the user wrote them in.

**Where a caste's colour comes from today.** Each caste lists her three on her own xenotype, as a
`PMM_Insects.InsectSkinColours` extension, and one is rolled for her when she is generated - so her
colour is always one of her own three, and the row above is the palette herself, not a suggestion.
The five tones every caste used to share are gone with it; §5.13 has the mechanism, and what a save
made before this does on load.

**What is still missing.** The hornet, the mantis, the mothman and the papillon - the four mockup
castes of `CASTES-PLAN.md` - have no colours yet, and neither does the soldier beetle's rebuild
beyond the row above. Until a caste has a palette she wears the shared chitin base `(120, 118, 89)`,
which `PMM_Gene_Insect` keeps for exactly that case.

---

### 5.17 The spider body takes her skin colour (2026-10-01)

The arachne, the ant arachne and the girtablilu have their lower body - legs and abdomen - drawn in
their own chitin colour now, instead of their hair colour. Before this, changing a spider momo's
hair recoloured eight legs with it, and the spider half never matched the woman attached to it.

**What B&S does, and why this had to be a copy.** Big & Small draws that body with two graphic sets
of its own - `BS_SpooderGraphicSetUpper` (legs and body) and `BS_SpooderGraphicSetUnder` (the
abdomen, south and north only) - and both take their main tint from `hairColor` at brightness 0.95.
B&S already ships the alternative: the lamia's tail takes `skinColor` (brightness 1.03) while its
second channel takes hair. Patching their two defs would have recoloured the spider body of every
spider-person in a save, B&S's own race included, so the user's call on 2026-10-01 was **our three
castes only, by copy** - and the whole lower body, not just the abdomen.

`Defs/ThingDefs/SpiderBody_BS.xml` is what we own now:

- `PMM_SpiderGraphicSetUpper` and `PMM_SpiderGraphicSetUnder`: B&S's art paths
  (`RaceDefaults/SpooderNew/`) with `colorA` from `skinColor` (brightness 0.95 - B&S's dial for that
  art; 1.0 would match her skin exactly) and `colorB` still the favourite colour.
- `PMM_RaceTracker_SpiderBody`: B&S's `BS_SpiderPersonRace` re-pointed at those two sets, carrying
  everything else it brought - the numbers (MaxNutrition x2, CarryingCapacity x1.5, Manipulation
  +0.05), both render nodes with their draw sizes, offsets, layers and body-type scales, the blank art
  for a pawn whose abdomen is destroyed, and the pawn behaviour that comes with the body (the spider
  romance tags, creep walking, the VEF stride hediff, the body position offset).
- All three races list that tracker in `raceHediffList` where they used to list B&S's: two trackers
  both drawing the body would draw it twice.

**What came off in the copy.** The `HasCustomizableGraphics` extension with B&S's `AbdomenGraphics`
tag - that tag is the player-facing way to force a colour onto the abdomen, it is B&S's, and it was
declared on B&S's race. Ours takes her skin colour with no escape hatch. Core still ships the unused
`PMM_TailGraphics` tag, which is what to reach for if an editable colour is ever wanted here. The two
`taur` gene-tag alts came off too: they blank the spider art for a pawn carrying B&S's taur gene, and
nothing in this mod carries one, so they could never fire.

**The cost to remember:** this file is a copy of B&S's spider configuration, so a B&S update that
changes the spider art, the draw offsets or the body behaviour will not reach the three spider castes
on its own. Re-diff against `SimplyRaces/Defs/Races/Spider/` when B&S moves.

---

## 6. VRE Insector wiring - what works, what does not

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
  only. An NPC mamono never seeks jelly. Raids are short, so this only matters for
  long-lived non-player pawns. Off-map villages are not simulated.

**Ruled 2026-09-27: per caste, not per role.** The gene now follows the species and
not the fighting, because the greenworm carries it alone among the brood. Carry it on
Greenworm, Soldier Beetle, Vamp Mosquito and Abaddon. Leave it off the Devil Bug and
the Giant Ant, whose packages of 2026-09-27 dropped it.

**Ruled: no `VRE_JellySacks` on any mamono.** Pawns never make jelly. Every drop
comes from the world or from buildings the player raises, which is what makes the
VFEI2 jelly economy matter.

**Ruled: keep.** The Abaddon carries it. She is a combat caste at 500 points, and
royal jelly is her flavour.

### 6.3 What we do not take

- `VRE_ChestburstPregnancy` - ruled out. It is player-gizmo only, but the victim
  hediff kills in 15 days, and the user said no.
- `VRE_SpawningSack` only interacts with chestburst when chestburst is present, so it
  was never the problem - but it is **out of the queen's package** as of 2026-09-26: VRE tags
  it and `VRE_Parthenogenesis` with the same `Fertility` exclusion tag. She traded
  parthenogenesis for `VRE_RobustMidgut` that day and took vanilla `Sterile` in its place, so the
  tag simply changed hands - the sack is blocked by it, before and after (§5.10).

---

## 7. VFEI2 wiring - hives and jelly

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
mamono can never pass.

**So `VRE_Hiveglands` is the only key that lets a mamono build a hive.** It is a
string compare on the gene name, not a race or faction check. That is why the
Giant Ant carries it in §5.3. With it she can raise `VFEI_HiveWall`,
`VFEI2_InsectJellyWall`, `VFEI2_JellyFarm`, `VFEI2_Creeper` and the artificial
hives. Without it she cannot build any of them.

The same gene gives +50% ConstructionSpeed and drops filth while she builds.
Both fit the species.

### 7.2 The jelly loop

Research: `VFEI2_BasicHivetech` → `VFEI2_StandardHivetech` → `VFEI2_ExoticHivetech`.

Producers a mamono colony can build, once the `VRE_Hiveglands` gate is passed:

| Building | Output |
|---|---|
| `VFEI2_JellyFarm` | `InsectJelly` 6 per 45000-65000 ticks (settings raise it) |
| `VFEI2_Creeper` | `InsectJelly` 10 per 150000 ticks, and it spreads `VFEI2_Creep` |
| `VFEI2_ArtificialBasicHive` | `InsectJelly` 5, and it spawns insects |
| `VFEI2_Artificial*Hive` (geneline) | `InsectJelly` 20 |
| `VFEI2_JellyMorpher` | `VFEI2_RoyalInsectJelly` 10, fuelled by jelly |

Loot: NPC hives leave 30 jelly, the walls leave 2 each.

Nothing on the mamono side produces jelly, by ruling 10. These buildings are the
whole supply for a colony that keeps insect mamonos.

### 7.3 Two small code changes

- `HiveNourishment.cs` - **done 2026-09-20.** It now resolves a list of hive-like
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
  entries to the kind gives mamonos their home-field speed. Optional, but cheap.

---

## 8. Kidnapping

**Vanilla kidnapping only (user ruling, 2026-09-24).** Their raids are humanlike,
so raiders can kidnap downed colonists. The man is then an ordinary prisoner of the
tribe, and the village he is held in can be raided to free him. Keep
`LordJob_Kidnap` and the vanilla job, and leave `IncidentParms.canKidnap` and
`LordJob_AssaultColony.canKidnap` alone.

**No hive special casing is left.** A tribe mamono uses the core's tsugai behaviour
unchanged: when she wins a bond the core rolls its join chance, and if it wins she
joins the colony like any other mamono. Three files were deleted on 2026-09-24:

- `HiveBondKidnap.cs` - the `HiveInsectMamono` gate, the forced "never joins" prefix
  on `TsugaiFormation.ExecuteJoin`, the abduct job and the "gone for good" letter.
  The gate tested `Faction.OfInsects`, which the tribes had not used since Phase 1,
  so none of it was reachable: the bond rule was dead code.
- `HiveKidnapVanish.cs` - the vanish at the map edge, which existed only to make
  that forced kidnap final.
- `FactionJoinQuiet.cs` - it silenced the "humanlike pawn added to a non-humanlike
  faction" error, which only the deleted generation swap could raise.

**Trap worth keeping:** the gate that died here was written against
`Faction.OfInsects`. A tribe faction test must go through
`FactionPlacement.IsInsectTribe`, which compares defNames.

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

**Phase 1 - delete the old spawn layer. (done 2026-09-20)**
Removed the swap, the wild kinds and the wild think trees. Expect real bugs
everywhere.
Gate: an infestation, a hive, an ancient danger and a VFEI2 geneline raid all
produce real insects. No mamono spawns anywhere. The age workaround is gone with
the swap, so check that no "generated downed pawn" spam remains.

**Phase 2 - the two factions, their names and their villages. (done 2026-09-20)**
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
the log for `Skipping AddHostilePawnGroup` - that means a `Settlement` group is
missing.

**Phase 3 - raids, children and the caste table. (done 2026-09-20; the children
were dropped, and the rest is not yet field-tested)**
Done: `pawnGroupMakers` filled with melee groups only - Combat, a second Combat
for the vanilla roster, Combat-melee, Peaceful, Settlement - plus the four work
groups vanilla tribes carry. The children were built and then removed by the user
(§5.5). The queen is still `Settlement`-only and still weight 1, which §11 item 1
asks about.
Gate: a large raid fields swarms of devil bugs, a few giant ants and at least one
soldier beetle, and no ranged group is ever used. A small raid never fields the
queen. A village raid fields her.

**Phase 4 - gene packages. (built 2026-09-20; not yet field-tested)**
Done: the six caste packages are in the six xenotypes (§5.3),
`VRE_InsectJellyDependency` on the five fighters only, no `VRE_JellySacks`
anywhere, and no `VRE_GenelineEvolution`. The pheromone effect is ours now, carried
by `PMM_Gene_Insect` instead of by VRE's gene.
Full environmental pollution immunity was ruled 2026-09-20 and is **done** too: it
lives on `PMM_Gene_Insect` (`Defs/GeneDefs/Genes_Insect.xml`), the gene all six
castes already carry, because `immuneToToxGasExposure` is a `GeneDef` field and no
race def can hold it.
Gate: each caste shows the expected genes in the Health tab. No gene conflict
errors. Real insects ignore a mamono while she is among them. A mamono with an empty
jelly bar picks up the deficiency hediff, and only the fighters do. No toxic
buildup builds up in tox gas, on polluted ground, or in toxic fallout.

**Extras, decided in the same conversation and built 2026-09-20.** Three things came
out of the Phase 4 discussion. None was in the original phase list:

- **Chitin, not human leather.** Every mamono race is `ParentName="Human"`, which
  carries `leatherDef Leather_Human` and a `LeatherAmount` of 75, so a butchered mamono
  used to yield human leather. All six races now set `VFEI2_Chitin` with a
  `LeatherAmount` of 30, matching VFEI2's own insect races
  (`Races_Fuelmite.xml`, `Races_Megawasp.xml`, `Races_RoyalMegascarab.xml`).
  Meat and blood were already right through `useMeatFrom Megaspider` and
  `bloodDef Filth_BloodInsect`.
- **Insect flesh.** `PMM_Gene_Insect` carries a VEF `GeneExtension`
  (`customMeatThingDef` + `defsTreatedAsHumanMeat`) so insect meat counts as human
  meat to a mamono: eating it is cannibalism to her and ordinary food to everyone
  else. The eating half of the human-leather analogy; the wearing half does not
  exist in any mod (§5.3).
- **One corpse line.** Six mamono corpses now sit under a single "mamono corpses" line
  in the butcher menu and every item filter, instead of six entries mixed into
  humanlike corpses. Three caches, one snapshot and Big and Small had to be dealt
  with, all of it written up in §12.9.

Gate: butchering a mamono yields 30 chitin; feeding insect meat to a mamono colonist
raises the cannibalism thoughts and doing the same to a baseliner does not; the item
filters show one "mamono corpses" line holding all six races, and a default butcher
bill still accepts a mamono corpse.

**Phase 5 - brood and nourishment. (built 2026-09-20, awaiting its field test)**
The egg spew clone, and the widened hive lookup.
Gate: the Abaddon's egg spew produces a swarmling, in the queen's faction, in a
dev-spawned village. A mamono standing near a `VFEI2_KemianHive` gets nourishment.

**Phase 6 - jelly and hives for the player.**
Confirm the build gate from §7.1 in both directions.
Gate: a recruited mamono with `VRE_Hiveglands` can build `VFEI2_JellyFarm` and
`VFEI2_HiveWall`. A mamono without the gene cannot.

**Phase 7 - kidnapping and polish. (done 2026-09-24)**
The hive bond rule, the vanish and the custom letter were deleted rather than
repointed at the tribes (user ruling 2026-09-24), so the tribes behave like every
other mamono. The faction icon and description were already in place; no new namers.
Gate: a tribe raid leaves the man a prisoner in a village he can be freed from, and
a bond win follows the core's roll.

**Phase 8 - trade and optional extras.**
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
5. **One mamono-corpse line across the whole family: done 2026-09-20.** The category
   def and the mover live in `Project Mamono` (`Source/ProjectMamono/MamonoCorpses.cs`) and
   `Defs/ThingCategoryDefs/ThingCategories_MamonoCorpses.xml`, and each species mod
   registers its own races: insects 6, slimes 6, elementals 8 (§12.9). Reptiles cannot
   join as they are - their pawns are vanilla Human with a xenotype, so their corpses
   are shared human corpses - and giving the 11 reptile species their own race defs is
   planned as a separate project (`Project Mamono Reptiles/RACES-PLAN.md`).

Worth asking later, not now:

6. Do we want our own settlement layout on top of VFEI2's four?
7. Do we want a custom Ideology culture, and VIE memes with it?
8. A settlement namer of our own, instead of `NamerSettlementTribal`.
9. **The abaddon folk's own art (2026-09-25).** Two pieces, both now the only
   things left on her: the **xenotype icon** (currently the queen's crest) and the
   **lower-arm sprite** (currently a placeholder, six files in
   `Textures/RaceDefaults/PMM_AbaddonFolk/`). Replacements go in at the
   same paths with the same names - no XML changes. §5.7 has the details, including
   the node settings likely to need tuning in dev mode.
10. **The insect abdomen - asked about and shelved (2026-09-25).** B&S's spider is
   the reference if it comes up again: `BS_SpiderAbdomen` is a 60 HP skin-covered
   body part childed to the torso at `height Bottom`, and in their body the stomach
   sits inside it. Nothing was built - "just do the arms for now" (§5.7).
   **Resolved for the queen, not for the folk (2026-09-26):** the abaddon's abdomen is
   `VRE_SpawningSack_Cosmetic`, VRE's look-only half of the spawning sack (§5.10), so no
   body part, no gut and no art of ours were needed. The folk still has none, and the
   spider stays the reference if she ever gets one.
11. **Should the abaddon be able to lay a honey bee?** Her brood list (§5.11) was not
    touched by the honey pass (2026-09-27), so the bee is not among the castes the
    queen can choose. One entry in that list if she should be - no code change.

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
   it does not. Mamonos eat, so they get the normal lord.

Two working examples in the user's Mods folder:

- `Reel's Insector Faction` - `FactionDef TribeInsector`, parent
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

### 12.4 The egg spew chain

`AbilityDef EggSpew` launches `Proj_EggSac`. `Proj_EggSac` uses
`Projectile_SpawnsThing` with `spawnsThingDef EggSac`. `EggSac` carries
`CompProperties_SpawnPawnOnDestroyed` with `pawnKind Larva` and
`lordJob LordJob_WanderNest`. This is what the old swap converted into a
greenworm.

### 12.5 Why the wild layer was the cheaper alternative, and why it is out

Recorded so nobody re-derives it. A mamono can be made real wildlife with no
Harmony at all:

- `BiomeDef.AllWildAnimals` iterates every `PawnKindDef` and yields any with
  `CommonalityOfAnimal > 0`.
- `CommonalityOfAnimal` reads `BiomeDef.wildAnimals` plus every pawnkind whose
  race declares `wildBiomes`. No `RaceProps.Animal` filter.
- `WildAnimalSpawner.SpawnRandomWildAnimalAt` spawns with
  `PawnGenerator.GeneratePawn(kind)`, so faction null.
- `AggressiveAnimalIncidentUtility.CanArriveManhunter` requires
  `RaceProps.Animal`, so manhunter packs could never have picked a mamono.

The user chose option C instead: no wild mamonos at all. If that ever changes, the
Reptiles mod already has a working pattern for factionless wild mamonos: a
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
  `LeatherAmount` stat of 75, and every mamono race is `ParentName="Human"` - so
  until 2026-09-20 a butchered mamono yielded human leather. The six races now set
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
alone. That patch and those kinds were deleted on 2026-09-20 with the rest of the
child work, so read this as the trap any future child kind walks into rather than as
something the mod still does.

**4. Role groups are free.** Vanilla `TribeBase` carries `Miners`, `Hunters`,
`Loggers` and `Farmers` at `commonality` 1, each `MayRequire="Ludeon.RimWorld.
Ideology"`, each holding a single faction-specific kind. Ours name the giant ant,
the devil bug and the soldier beetle. Only Ideology code paths ask for those group
kinds, so they cost nothing, and nothing else in the mod depends on them.

### 12.9 One line for mamono corpses: a snapshot, three caches, and Big and Small

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

The humanlike test comes first, so `fleshType Insectoid` cannot move a mamono corpse,
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

**What the code does.** `Project Mamono` owns both halves now (2026-09-20):
`Defs/ThingCategoryDefs/ThingCategories_MamonoCorpses.xml`
defines `PMM_MamonoCorpses` ("mamono corpses"), a child of `CorpsesHumanlike` the way
vanilla's `CorpsesInsect` is a child of `CorpsesAnimal` - that nesting is what keeps
every filter that allows humanlike corpses accepting a mamono corpse, and what makes
the menu show one line instead of one entry per race - and
`Source/ProjectMamono/MamonoCorpses.cs` moves the corpses out of `CorpsesHumanlike` and
B&S's two categories, keeps `childThingDefs` in step on both sides, and calls
`ResolveReferences()` on every category to rebuild the caches. `MamonoCorpses.Register(...)`
is called once per species mod from its own `[StaticConstructorOnStartup]` (insects:
`Source/Insects/InsectMamonoCorpses.cs`), and the move runs again from a postfix on
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
- 2026-09-20: Changed the insect mamonos to live only in insector tribes. Vanilla insects spawn normally again.
- 2026-09-20: Removed wild insect mamonos.
- 2026-09-20: Changed each insect mamono caste to fight with insect genes, so her strengths match her species.
- 2026-09-20: Added insect jelly as food for the insect mamonos.
- 2026-09-20: Changed a kidnapped man to be held prisoner in the tribe's village, instead of being lost forever.
```

```markdown
## Internal

- 2026-09-20: Removed the pawn generation swap and the wild mamono layer.
```

One change per line. Do not batch them into one entry.
