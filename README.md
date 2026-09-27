# Project Mamono Insects

A Project Mamono sub-mod that turns the six insectoids of the Rim into insect mamonos:
human-shaped monster girls. The mamonos live in insector tribes of their own, in villages
raised from hive walls and creep. Real insects still spawn, raid and infest as they
always did.

Two tribes settle on polluted ground: one neutral, one always at war. A giant ant of
the tribe trades with player caravans that reach a village.

| Insect | Mamono | What she keeps |
|--------|------|----------------|
| Megascarab | Devil Bug | 72% sharp chitin, 3.75 c/s, full-grown before her small gene |
| Spelopede | Giant Ant | 170% HP, digging claws, great strength |
| Megaspider | Soldier Beetle | weak flight, large frame, 250% HP, ripper claws, sturdy exoskeleton |
| Larva (Odyssey) | Greenworm | acid sludge spew, frail frame |
| Locust (Odyssey) | Vamp Mosquito | small weak wings, no caravan speed, 3.0 c/s |
| HiveQueen (Odyssey) | Abaddon | four arms, 980% HP, never sleeps, beautiful voice, egg-spew |

Abaddon keeps the strong flight gene, so she does not carry the core large frame gene: the
two are exclusive, and a flying queen is worth more to a caravan than a mount. The soldier
beetle, whose wings only manage short hops, carries the large frame gene instead.

The flight genes also let a mamono ignore rough ground: mud, sand and snow never slow the
three flying mamonos (Abaddon, soldier beetle and vamp mosquito), and they cross water at
their own pace.

Two species needed a health fix. The devil bug and the greenworm copy the bugs' tiny
health scale, which left their limbs paper-thin. A patch multiplies their health scale
by 2.5, so the devil bug sits at 100% of a human and the greenworm at 62%.

Chitin comes in five tones. Every insect xenotype lists the same five skin genes, each
marked `randomChosen`, and vanilla keeps one of them at random per mamono - the same trick
vanilla uses for melanin and tails. So no two insect mamonos are the same colour, and a
daughter rolls her own tone.

The races live in `Defs/ThingDefs/Races_InsectMamono_BS.xml`, on the Big & Small
race pattern.

Abaddon folk are a seventh insect mamono, with no bug of her own. She is the soldier
class of an abaddon's swarm, with four arms and wings. Nothing in the world spawns
her: no village raises her and no raid brings her. You can spawn her in dev mode from
the spawn pawn list, or apply her xenotype to a woman, which turns her into one.

**Requires:** Project Mamono (PMM.Core), Biotech, Harmony, Big and Small - Framework,
Vanilla Expanded Framework, Vanilla Races Expanded - Insector, and Vanilla Factions
Expanded - Insectoids 2.
**Optional:** Odyssey (larvae, locusts, hive queens).

Build with `./build.sh`. It builds Project Mamono first through the project reference.
Deploy to a live RimWorld install with `./sync.sh`.
