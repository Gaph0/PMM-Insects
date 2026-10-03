# Project Mamono Insects

An insect sub-mod of Project Mamono. It turns the insectoids of the Rim into insect
mamonos - monster girls who keep the bug they came from - and gives them tribes of
their own.

## Details

The mod does four new things:

1. **Two insector tribes.** A neutral hive that trades and can be befriended, and a
   swarm that is always at war. Both live in villages raised from hive walls and creep,
   and both take men captive the way every other mamono does.
2. **Sixteen insect mamono castes.** Each is built on a Vanilla Races Expanded -
   Insector geneline, each lists her own three chitin colours and rolls one of them
   when she is born, and each works, fights or farms the way her bug did.
3. **The abaddon queen and her swarm.** She lays mamonos to order instead of eggs, and
   her soldier class, the abaddon folk, ships with her as her infantry.
4. **A use for every one of them.** A greenworm who has taken in enough mana spins a
   cocoon and comes out a papillon, jelly feeds the hive, honey bees make honey you can
   milk, and every mamono butchers into chitin.

Vanilla insects are untouched. They still spawn, raid and infest as they always did.

## Content

### The xenotypes

Sixteen castes, one per xenotype. Body size and health are read off the bug each one is
built from, and two of them carry a patch that multiplies a paper-thin health scale by
2.5 so their limbs stand up to damage. Flight genes also let a mamono walk over mud,
sand and snow at full speed and cross water at her own pace.

| Mamono | Who she is | Her numbers |
|---|---|---|
| **Devil Bug** | A cockroach-like drudge who raises and mends the hive's walls. | Body 1.0, health 100% after her fix, 72% sharp chitin, 3.75 c/s |
| **Giant Ant** | A diligent, strong digger and hauler. | Body 1.0, health 170%, digging claws |
| **Soldier Beetle** | An armoured, obedient heavy who grows one of three weapons. | Body 1.2, health 250%, slower than her sisters |
| **Greenworm** | A soft, gentle girl who eats far more than any of her sisters, and matures into a papillon. | Body 1.0, health 62% after her fix, hunger 0.50, the highest in the mod |
| **Vamp Mosquito** | A winged blood-drinker with a venomous bite. | Body 1.0, health 70%, weak flight, 3.0 c/s |
| **Abaddon** | The queen of the swarm: four arms, wings, no sleep, endless appetite. | Body 1.16, health 980% |
| **Abaddon Folk** | The swarm's petite infantry, who reads her orders in the pheromones. | Body 0.9, health 100%. Nothing in the world spawns her: dev mode or a xenotype swap |
| **Arachne** | A patient, cruel weaver of the deep dark, who spins and sells silk. | Body 1.3, health 40%, eight legs |
| **Beelzebub** | A lord of gluttony among the swarm's flies: tiny, quick, magically potent. | Body 0.9, health 70% |
| **Girtablilu** | A desert assassin: scorpion-shelled, pincer-handed, with venom in her claws. | Body 1.25, health 44% |
| **Ant Arachne** | A small arachne disguised as a giant ant, who builds and spins in the ants' own nest. | Body 1.0, health 36% |
| **Honey Bee** | The hive's cheerful farmer, whose body fills with honey as she works. | Body 1.0, health 40%, milkable |
| **Mothman** | A drowsy, gentle moth woman who loves lamp light and cannot fight. | Body 1.0, health 40%, bright light lifts her mood |
| **Papillon** | A beautiful, fragile butterfly woman who cannot fight and learns fast. | Body 1.0, health 40% |
| **Hornet** | A quick, bad-tempered wasp who fights for the hive and cannot stand honey bees. | Body 1.0, health 40% |
| **Mantis** | A cold, thin-shelled killer with blades folded into her arms. | Body 1.0, health 40% |

### The genes

These six are the mod's own. The castes also carry genes from Project Mamono and from
VRE Insector, which is where their genelines come from.

| Gene | What it changes |
|---|---|
| **insect** | The gene every caste shares. Carries the pheromones that make real insects ignore her, full immunity to pollution, tox gas and toxic fallout, and treats insect meat as human meat to her, so eating it is cannibalism. |
| **honey** | Her body fills with honey as she works. Milk it by right click, or with the button on her bar. Honey is food that never spoils. |
| **beetle armament** | Settles one of three weapons when she is born: ripper-blades (cut 18), insect mandibles (cut 24) or megaspider horns. Costs 2 metabolic efficiency, 1 complexity. |
| **quarrelsome** | -50 opinion toward every other carrier, and it runs both ways, so a village of soldier beetles is a village of grudges. Gives back 1 metabolic efficiency, costs 1 complexity. |
| **honey-hating** | -50 opinion toward anyone carrying the honey gene, one way only: the honey bee feels nothing back. Gives back 1 metabolic efficiency, costs 1 complexity. |
| **disorientating lights** | +10 mood while she stands in bright light. |

### The events

The mod adds no incidents of its own. Everything happens through the two tribes, on
vanilla's own systems.

- **Raids**, from the insector swarm, which is a permanent enemy and so never stops.
  They are melee only: no ranged group exists for either tribe.
- **Village attacks.** A village map holds its people, hive walls, creep and hives, and
  its queen - one chance in thirty-three of an abaddon standing there. She cannot be
  met any other way, so a queen is only seen by attacking a village.
- **Trade.** The neutral hive sends caravans once your colony is worth visiting, and
  player caravans that reach a village can trade there. A giant ant of the tribe does
  the trading.
- **Kidnapping.** A raid that downs a man may carry him off to a village, and he can be
  freed from there.

### The abilities

- **Web trap** (arachne) - she throws a web that pins whoever it lands on. Cooldown 10
  seconds, range 12, warmup 1.5 seconds.
- **Egg spew** (abaddon) - she lays a brood sac at her feet. Cooldown 15 days, range
  under 2 cells. What comes out is the caste you chose with her brood order.
- **Brood order** (abaddon) - the button on her bar. Pick which of the other fifteen
  castes she lays. Left unset, she lays a swarmling.
- **Flight leap** (every flying caste) - a long leap, range 29.9, no line of sight
  needed, 60 tick cooldown. It comes from Project Mamono's flight genes.
- **Milk honey** (honey bee) - right click her, or use the button on her bar.

### The factions

| Faction | Who they are | Where the player stands |
|---|---|---|
| **Insector hive** | The neutral tribe: devil bugs, giant ants, soldier beetles, ant arachne, honey bees, and the gentle mothman and papillon, with traders on top. | Neutral, and befriendable. Their caravans visit you, and you can trade at their villages. |
| **Insector swarm** | The hostile tribe: the fighters, plus the arachne, the beelzebub and the girtablilu. Their villages hold the abaddon queens. | Always at war. They raid, and they never stop. |

## Found a bug?

Please report it on the issue tracker: **https://github.com/Gaph0/PMM-Insects/issues**

That is the only place it can be fixed from. Please do not leave bug reports in the
Steam comments - they get lost there, and a report on the tracker keeps the description,
the log and the fix together. A good report says what you did, what happened, and
attaches your log.

## Licence and credits

**Licence:** to be added - this mod does not ship a licence file yet.

**Credits:** written by Gapho. Thanks to the mods this one stands on:

- **Project Mamono** (PMM.Core) - the mamonos themselves, the fertility rules and the shared art.
- **Vanilla Races Expanded - Insector** - the geneline genes every caste is built from, and the abilities they carry.
- **Vanilla Factions Expanded - Insectoids 2** - the hives, the jelly and the chitin the tribes trade in.
- **Big and Small - Framework** - the race pattern, the wings, the body swaps and the colour channels.
- **Vanilla Expanded Framework** and **Harmony** - the machinery underneath both.
- **Biotech** - genes and xenotypes at all.

Built with `./build.sh`, which builds Project Mamono first through the project
reference. Deployed to a live install with `./sync.sh`.
