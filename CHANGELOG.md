# Changelog

## 0.1.0 (unreleased)
- Initial release: the six insectoids of the Rim (megascarab, spelopede,
  megaspider + larvae, locusts and hive queens with Odyssey) become insect
  momos — devil bugs, giant ants, soldier beetles, greenworms, vamp mosquitoes
  and abaddons.
- Generation swap: a Harmony prefix on PawnGenerator replaces every insect
  pawnkind with the matching momo pawnkind, so infestations, hives, ancient
  dangers, egg sacs, wild spawns and trader livestock are all affected.
- Stats kept 1:1 from the bug defs: chitin armor, health scale, speed,
  cold-blooded comfort ranges, toxin/vacuum immunity, melee mandibles and
  claws, and the hive queen's sleeplessness, egg-spew and jelly butchery.
- Wild insect momos are tameable wild women (wild-man recipe: factionless
  spawn, IsWildMan + ShouldReachOutside patches, wild think-tree inserts).
- Fixed: removed the per-race lifeExpectancy values copied from the bugs
  (4-75 years). They sit below the humanlike adult age of 18, so the game
  failed 300 times to pick an age and logged "Tried 300 times to generate
  age" for every adult insect momo it spawned. All races now inherit the
  humanlike 80 from the base def — lifeExpectancy is the one stat not
  copied 1:1 from the bugs.
- Odyssey is a soft dependency: without it only the three base-game insects
  are transformed.
