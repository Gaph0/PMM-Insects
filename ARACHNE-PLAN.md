# Arachne plan (Project Mamono Insects)

Agreed with the user 2026-09-26. Spider mamono of the hostile abaddon tribe.

## Verified facts this plan rests on

- **Body: Big & Small's own spider, no new art.**
  - `Big and Small - Framework/1.6/SimplyRaces/Defs/Races/Spider/`: `BodyDef_Spider.xml`
    (`BS_SpiderHybrid`, with `BS_SpiderAbdomen` as a real, damageable part),
    `GraphicSetDef_Spider.xml` (`BS_SpooderGraphicSetUpper`, the upper-body art with its colour
    channels), `ThingDef_Spider.xml` (`BS_SpiderPersonRace`) and `RaceTracker_Spider.xml`
    (`BS_SpiderPersonRace`: MaxNutrition x2, CarryingCapacity x1.5, Manipulation +0.05, plus the
    render nodes that draw the abdomen).
  - **Race route, not the gene route** (user's call): a B&S `thingDefSwap` gene silently refuses a
    race def that is not literally Human - recorded in `PMM_Gene_Flight`'s own comment - and she has
    her own race.
  - B&S's tracker is added through **`raceHediffList`**, never merged into ours: B&S declares its
    trackers with `<defName>` only, so XML inheritance cannot find them as a parent (file header of
    `Races_InsectMamono_BS.xml`, 2026-09-20).
- **The look variant is settled: neither exists.** Confirmed in game 2026-09-26: there is no
  Synthread gene and no Devilstrand one. `BS_SpiderBody_Synthread` and `BS_SpiderBody_Devil` appear
  only inside B&S's own gene-page settings and have no def behind them in this install. The stock
  spider body renders clean, so she wears that and there is nothing to add or mimic.
- **Genes that exist** in this load order: `VRE_HighGreyMatter`, `VRE_VocalGlands`,
  `VRE_InsectVolatile`, `VRE_ProteinDenaturation`, `VRE_InsectJellyDependency` (VRE Insector), and
  the vanilla aptitude genes, which are Biotech `GeneTemplateDef`s used by name.
- **User vocabulary for tiers (2026-09-26):** "good" = `AptitudeStrong_<Skill>`, "great" =
  `AptitudeRemarkable_<Skill>`.
- **Silk:** Medieval Overhaul's silk is `DankPyon_Silk`.
- **Coffee:** `VBE_HotCoffee` in `Vanilla Brewing Expanded/1.6/Defs/ThingDefs_Items/Drinks_Coffees.xml`,
  `ParentName="DrugBase"`, `foodType` already includes `Liquor`, `joyKind Chemical`; the caffeine
  effect rides its `ingestible` outcome doers.
- **Webs already in the load order:** `AA_Web` and the `AA_*Web` family (Alpha Animals), `AG_Web`
  (Alpha Genes), DankPyon's `Webknecht`.

## Decisions (user, 2026-09-26)

| Question | Answer |
|---|---|
| Body route | race route |
| Look variant | none needed - neither `BS_SpiderBody_Synthread` nor `BS_SpiderBody_Devil` has a def behind it |
| Web trap | **thrown ability** |
| Silk | gathered from her by **right click**, and she can gather it herself |
| Placement | hostile abaddon tribe **only**, and she can be laid by an abaddon |
| Crafting | good, not great -> `AptitudeStrong_Crafting` |

## Done (phase 1, 2026-09-26)

- `Defs/ThingDefs/Races_InsectMamono_BS.xml`: `PMM_Race_Arachne` (`<body>BS_SpiderHybrid</body>`,
  insect flesh/ichor/meat, `VFEI2_Chitin` leather, SM/base body size pair 1.3 -> 1.69,
  `baseHealthScale` 0.4 -> 1.0 after HealthScalePatch, four melee tools, hive nourishment) and
  `PMM_RaceTracker_Arachne` (Manipulation +0.10, sharp 0.15, race comp, romance tags), with B&S's
  `BS_SpiderPersonRace` attached through `raceHediffList`.
- `Defs/XenotypeDefs/Xenotypes_Insect.xml`: `PMM_InsectArachne`, seven identity genes plus the five
  chitin tones, `setRace`/`forceRace` to `PMM_Race_Arachne`, `factionlessGenerationWeight 0`.
  Plus core's `PMM_Gene_LargeFrame` (user's call, same day): 20% for any caravan she joins and
  caravans count her as a riding animal, with no size change, so her race's 1.3 pair stands.
  Deliberately without `VRE_InsectSkin`/`VRE_InsectAntennae`: her upper body is the spider's own
  art and an insect fur body or head would fight it.
- `Defs/PawnKindDefs/PawnKinds_InsectorTribe.xml`: `PMM_InsectArachne`, combat power 110,
  `factionlessGenerationWeight 0`.
- **Wired into the tribe (same day):** weight 2 in the village garrison and 3 in the queen's own
  garrison in `Factions_InsectorTribes.xml` (settlements only, never a raid group), and an entry in
  the abaddon's `CompProperties_BroodOrder` list, so a queen can lay a weaver on purpose. Note that
  the same list feeds "a random mamono", so she can also come out of a random draw.
- `CHANGELOG.md`: one line.

## Next

1. **Verify her in game**: done 2026-09-26 - she spawns clean, her graphics are right, and the
   Synthread/Devilstrand variants turned out not to exist.
2. **Phase 2, flavour: CLOSED 2026-09-27.** `Masochist` stays, applied through
   `PawnKindDef.forcedTraits` (`List<TraitRequirement>`, so `<li><def>Masochist</def></li>`) on her
   pawn kind, which covers dev spawns AND daughters laid from an egg sac because both generate from
   the kind. Everything else in this phase was scrapped by the user the same day - no `Bloodlust`, no
   Alpha Memes `AM_Sadist`, no backstory, no description polish - so none of it is pending work.
   Kept because it has cost time before: do not reach for B&S to force a trait. `PawnExtension` has a
   `forcedTraits : List<TraitDef>`, but it is applied by `GeneEffectManager.ApplyForcedTraits(bool,
   Gene)` - from genes - so a race tracker is the wrong home for it and would silently do nothing.
3. **Phase 3, silk: DONE 2026-09-26.** `PMM_ArachneSilk` (a plain vanilla-style `Fabric` textile,
   which covers ordinary recipes in either direction - MO's `DankPyon_Silk` is a `Fabric`/`Textiles`
   def too), `CompArachneSilk` for the growth state, a right-click order through a Harmony patch on
   `Pawn.GetFloatMenuOptions` (core's own pattern), and a self-gather pair: `PMM_GatherArachneSilk`
   job plus `WorkGiver_ArachneSilk`, a self-targeting scanner on the model of vanilla's `SelfTend`,
   filed under **Hauling** (user's call 2026-09-26, moving it off Handling).
   `Patches/ArachneSilk_MedievalOverhaul.xml` handles **Medieval Overhaul compat** and is gated on MO
   being loaded: it re-points the race's `silkDef` from our def to `DankPyon_Silk`, because MO takes
   its silk as a *named* ingredient in its rugs and royal furniture (`<DankPyon_Silk>150</DankPyon_Silk>`
   in a costList) and no fabric category can substitute; it also adds our def to the
   `excludedThingDefs` of every trader generator stocking `Textiles`, so the def nothing produces any
   more cannot appear from thin air in trade. `tradeability` is deliberately untouched, so the player
   can still sell a spare bundle.
   The art is vanilla cloth tinted, and that is final: the art pass was scrapped on 2026-09-27.
4. **Phase 4, web trap: DONE 2026-09-26, guard added 2026-09-27.** The effect is Alpha Animals', and
   it is not a thing class: their `AA_Web` projectile is a plain `BaseBullet` whose `projectile` block
   is `damageDef Stun`, 8 damage - the stun *is* the trap, there is no lingering web on the ground.
   (The custom `AlphaBehavioursAndEvents.Web_Projectile` class belongs to AA's *other* webs - Frost,
   Fire, Acidic, Psy - not to this one.) `Defs/AbilityDefs/Abilities_ArachneWeb.xml` defines
   `PMM_Ability_WebTrap`: their verb on their projectile, their throwing sound and icon, our label and
   wording, one web per cast instead of their burst of three, range 12 rather than their 25, and a
   600-tick cooldown. The `<abilities>` list sits on `PMM_RaceTracker_Arachne`, not on her pawn kind,
   so every arachne has it however she was generated; the def and the reference carry the same
   `MayRequire="sarg.alphaanimals"` gate.
   **The bug (user reports 2026-09-27): webs kept firing every few seconds while the gizmo still showed
   seven seconds of cooldown; then, after the shot was refused, the same rhythm of attempts with no web
   leaving.** Cause, read out of Assembly-CSharp rather than guessed: nothing on the cast path asks the
   ability about its cooldown. `Ability.Activate` only applies effects, `Toils_Combat.CastVerb` hands
   the cast straight to `Verb.TryStartCastOn`, and that checks the caster, the verb's own state and the
   shot line - `Ability.CanCast`, the one property that reports the cooldown, is consulted by the gizmo
   and by `JobGiver_AICastAbility` and by nothing else.
   `Source/Insects/ArachneWeb.cs` adds `Verb_WebTrap : Verb_AbilityShoot` with two guards, and the order
   they were needed in is the interesting part. The first attempt refused at `TryCastShot`, which stopped
   the web but not the attempt: the user's own log then showed ten refusals with 508/416/324/232 ticks
   left, about 92 ticks apart - our 1.5s warmup - on the stack
   `Stance_Warmup.Expire -> Verb.WarmupComplete -> Verb.TryCastNextBurstShot -> TryCastShot`. So a fresh
   cast is being *started* every warmup, and the refusal has to happen there: `TryStartCastOn` now refuses
   first, so an attempt during cooldown costs nothing at all (no warmup stance, no sound, no stance lock),
   and the shot guard stays as a second line for the Harmony-patched `Verb.TryCastNextBurstShot` (`_Patch3`
   in that stack - another mod patches it) seen in the trace. (`Available()` would have been the wrong hook:
   `TryStartCastOn` never asks it, only `TryCastNextBurstShot` does.)
   `aiCanUse` is false, as on the abaddon's egg spew, but that alone did not stop it - so whatever starts
   the cast either ignores the field or never reads it. Ruled out while looking: VRE Insector's humanlike
   think-tree patch (it adds only `JobGiver_ConsumeInsectJelly`), VFEI2's `JobGiver_AICastAbilityOnPosition`
   nodes (animal subtree only, each hard-coding a burrow ability), and the newest save (it predates the
   ability, so it held no job to inspect). The caller is therefore still unnamed, and the guard logs pawn,
   current job and a stack trace on every refusal until it is. Delete that diagnostic, not the guards, once
   a session logs no lines.** That is now settled and the diagnostic is gone: it was removed on
   2026-09-27, in the same pass as the AI giver below, after the log showed no refusals at all.
   **Enemy use, 2026-09-27 (user: "give enemy arachnes the ability too").** Until that day only the
   player could fire it. `Source/Insects/WebTrapAI.cs` adds `JobGiver_WebTrapTarget :
   JobGiver_AICastAbility` - vanilla's giver is abstract, and the one concrete version in the base
   game, `...OnSelf`, has nothing to do with throwing a web at someone - and
   `Patches/ArachneWeb_EnemyUse.xml` prepends it to the `Humanlike` tree (which `Human` sets as its
   `thinkTreeMain`, so her race inherits it). Prepended, not appended: in a `ThinkNode_Priority` the
   first node that returns a job wins, so appending would park it behind the ordinary fight giver and
   she would never web. The giver only fires for pawns hostile to the player, which is what keeps a
   player's own arachne a button the player presses, and it honours the cooldown through the base
   giver's `CanCast` check - one web, then fists while it recharges. `aiCanUse` is true again to match.
   It wears Alpha Animals' own icon, final for the same reason: the art pass was scrapped on
   2026-09-27.
5. **Phase 5, coffee: DONE 2026-09-27.** Her caffeine is alcohol, at one coffee per beer.
   The ask: with Vanilla Brewing Expanded loaded, an arachne who drinks coffee should get drunk
   instead of caffeinated.
   **What the two sides actually do** (read out of their XML and Assembly-CSharp, not assumed):
   - VBE's coffee is `VBE_HotCoffee` (workshop folder `2186560858`,
     `1.6/Defs/ThingDefs_Items/Drinks_Coffees.xml`). Its effects are all `ingestible.outcomeDoers`:
     one `IngestionOutcomeDoer_GiveHediff` giving `VBE_HotCoffeeHigh` (severity 1,
     `toleranceChemical VBE_Caffeine`), a second giving `VBE_CaffeineTolerance` (0.016). Its
     `CompProperties_Drug` sits on chemical `VBE_Caffeine` (addictiveness 0.005), which is where
     `VBE_CaffeineAddiction` and `VBE_CaffeineWithdrawal` come from.
   - Vanilla's own drunk is beer: the same doer class giving `AlcoholHigh` at 0.15 with
     `toleranceChemical Alcohol`, plus an `AlcoholTolerance` doer. So the target shape is one we
     already know works.
   - Vanilla's `IngestionOutcomeDoer_GiveHediff` exposes exactly `hediffDef`, `severity`,
     `toleranceChemical`, `divideByBodySize` and `multiplyByGeneToleranceFactors` - **there is no race
     filter**, on it or on the base class. "For an arachne only" therefore cannot be done with
     vanilla's classes in pure XML. The way round it is the one VFE Insectoids 2 already uses: ship
     our own `IngestionOutcomeDoer` subclass, since a doer in XML can name any class.
   - `CompDrug.PostIngested` (decompiled) only deals with overdose, drug history and the
     last-took-a-drug ticks. The high and the tolerance come from the doers, and doers run in list
     order - so a doer of ours **appended last** can undo what theirs just did.
   **The plan:**
   1. `Source/Insects/CoffeeDrunk.cs`: one `IngestionOutcomeDoer` subclass, no Harmony. For a pawn of a
      named race it gives a named drunk hediff and removes a named list of caffeine hediffs. Every
      defName lives in the XML patch, never in the C#, so nothing dangles in a load order without VBE.
   2. `Patches/ArachneCoffee_VBE.xml`: gated on VBE being loaded, appends that doer to
      `VBE_HotCoffee/ingestible/outcomeDoers` with `<race>PMM_Race_Arachne</race>`,
      `<drunkHediff>AlcoholHigh</drunkHediff>`, a severity, and the suppression list
      (`VBE_HotCoffeeHigh`, `VBE_CaffeineTolerance`, and `VBE_CaffeineAddiction` if it proves to be
      doer-applied).
   3. One thing left to check while building it: the addiction rides the chemical rather than the doer
      list, and *where* that lands relative to ingestion is not yet verified. If it lands after our
      doer, a small postfix on `Thing.Ingested` covers it. Either way the strip is a list of names, so
      extending it later is one line.
   4. Deliberately **no** `toleranceChemical` on our doer: coffee should not raise her tolerance to
      *alcohol*, which is what that field would do.
   **Decisions the user still owes me:** which pawns (just the arachne, or every insect mamono), which
   drinks (coffee alone, or the rest of the VBE caffeine family - teas and energy drinks ship in the
   same mod), how drunk (beer is 0.15; coffee is the weaker drink), and whether the caffeine she has
   already absorbed should be forgiven if this lands mid-addiction.
   **Effort:** one C# file plus one gated patch, testable in a single sitting - spawn her, dev-spawn
   coffee, watch the hediff list.

   **Built and deployed 2026-09-27.** The user's answers: just the arachne; every caffeinated drink;
   drunk severity mirrors the caffeine severity; an existing addiction is not to be forgiven.
   `Source/Insects/CoffeeDrunk.cs` holds `IngestionOutcomeDoer_CoffeeDrunk`, and
   `Patches/ArachneCoffee_VBE.xml` appends it with one xpath keyed on
   `toleranceChemical="VBE_Caffeine"` (with a `comps/li/chemical` branch for a drink whose caffeine
   rides a drug comp instead) - so there is no drink list to maintain and any caffeinated drink added
   later is swept up too. Checked against the real defs with a real XPath engine: the xpath reaches
   **13 drinks** - hot, iced, latte, mocha, mint, lemon, pumpkin, dalgona, cannibal and Irish coffees,
   Yuenyeung, the energy drink, and one coffee liquor from another mod - and each mirrors a caffeine
   severity of 1.000. Two consequences to know: vanilla beer's drunk is 0.15, so at `severityFactor 1`
   one coffee is a heavy drunk (that field is the one-line dial), and the energy drink's
   `ToxicBuildup` doer is also keyed to `VBE_Caffeine`, so she skips that side effect too.
   Also checked, because the user asked for teas by name: the teas in that family (`VBE_BlackTea`,
   `VBE_GreenTea`, `VBE_EarlGreyTea`, `VBE_LemonTea`, `VBE_MintTea`, kombucha) carry **no caffeine at
   all** - `VBE_BlackTea` has no `outcomeDoers` whatsoever and its drug comp is not on the caffeine
   chemical; its "wakes one up" effect is a tannin hediff giving immunity gain and rest-fall stats.
   The user's ruling on 2026-09-27 was to ignore teas, so they stay tannin drinks and nothing more.
   **The deploy was briefly blocked, for a reason that had nothing to do with coffee:** the user pruned
   the local Mods folder when their mods updated, which took the local `Big and Small - Framework` copy
   with it, and both build files pointed at that copy - so core stopped compiling and `sync.sh`
   correctly refused to ship XML that expects an assembly never built. Both files now reference the
   workshop copy (`$(SteamModContentFolder)/2925432336/1.6/Base/Assemblies`); those were the only three
   stale local-path references in the tree, so a Steam update can no longer break the build this way.
   Rebuilt, synced, gate passes.

   **Playtest 2026-09-27: one black coffee blacked her out.** The straight mirror was the problem -
   every caffeine high in the load order is severity 1, and `AlcoholHigh`'s top stage caps
   Consciousness at 0.1, which is the blackout, so a single coffee was maximum drunk. `severityFactor`
   is now 0.15: one coffee is one beer (vanilla's stages are tipsy at 0.25, drunk at 0.4, hammered at
   0.7, blackout above that), so about two coffees for tipsy and six to black out. Note the factor
   cannot tell drinks apart - every caffeine high here is severity 1, so a coffee and an energy drink
   weigh the same.

## Honest expectations

- The body is B&S's, but whether the taur silhouette, the abdomen render and the upper-body art all
  land on the first try is unverified: expect one or two in-game iterations on the look, the same way
  the winged castes needed them.
- `baseHungerRate` 0.15 and the size pair 1.3 are first guesses; both are one-line dials.
