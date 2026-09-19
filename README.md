# Project Momo Insects

A Project Momo sub-mod that turns the six insectoids of the Rim into insect momos:
human-shaped monster girls. Every vanilla insect becomes the matching momo, so
infestations, hives, ancient dangers, egg sacs, wild spawns and trader livestock all
change over.

| Insect | Momo | What she keeps |
|--------|------|----------------|
| Megascarab | Devil Bug | 72% sharp chitin, 3.75 c/s, short frame |
| Spelopede | Giant Ant | 170% HP, digging claws, great strength |
| Megaspider | Soldier Beetle | 250% HP, ripper claws, sturdy exoskeleton |
| Larva (Odyssey) | Greenworm | acid sludge spew, frail frame |
| Locust (Odyssey) | Vamp Mosquito | wings, 3.0 c/s |
| HiveQueen (Odyssey) | Abaddon | 980% HP, never sleeps, egg-spew |

Two species needed a health fix. The devil bug and the greenworm copy the bugs' tiny
health scale, which left their limbs paper-thin. A patch multiplies their health scale
by 2.5, so the devil bug sits at 100% of a human and the greenworm at 62%.

The six races live in `Defs/ThingDefs/Races_InsectMomo_BS.xml`, on the Big & Small
race pattern.

**Requires:** Project Momo (PMM.Core), Biotech, Harmony, Big and Small - Framework.
**Optional:** Odyssey (larvae, locusts, hive queens).

Build with `./build.sh`. It builds Project Momo first through the project reference.
Deploy to a live RimWorld install with `./sync.sh`.
