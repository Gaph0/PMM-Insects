# Changelog

## Player-facing

- 2026-09-19: Added Big and Small - Framework as a required mod.
- 2026-09-19: Changed all six insect momos to the Big & Small body system. Each one now draws at her real size: the devil bug scuttles at your feet, the abaddon stands over you.
- 2026-09-19: Fixed devil bug and greenworm limbs breaking too easily.
- 2026-09-19: Added a chitin-dark carapace skin to every insect momo.
- 2026-09-19: Removed the insectoid gene's +15% sharp armour. The new race armour already covered it, and the two stacked.
- 2026-09-19: Changed the insectoid gene's category from Violence to Miscellaneous.
- 2026-09-19: Fixed adult insect momos failing to roll an age and logging "Tried 300 times to generate age".
- 2026-09-07: Fixed hive nourishment refilling too slowly.
- 2026-09-06: Fixed errors that appeared when the mod loaded.
- 2026-09-06: Added the six insect momos: devil bug, giant ant, soldier beetle, greenworm, vamp mosquito and abaddon. Every vanilla insect becomes the matching momo, so infestations, hives, ancient dangers, egg sacs, wild spawns and trader livestock all change over.
- 2026-09-06: Added each bug's stats to her momo: chitin armour, health scale, speed, cold-blooded comfort range, toxin and vacuum immunity, mandibles and claws, and the hive queen's sleeplessness, egg-spew and jelly butchery.
- 2026-09-06: Added hive nourishment. A momo near a nest tops up her hunger and mana.
- 2026-09-06: Added insect flesh, insect meat and insect blood to every insect momo.
- 2026-09-06: Changed wild insect momos to be tameable wild women.
- 2026-09-06: Changed every insect momo to an 80-year lifespan, so adult ages generate correctly.
- 2026-09-06: Removed the Hemogenic gene from the vamp mosquito.
- 2026-09-06: Changed Odyssey to an optional dependency. Without it only the three base-game insects change.

## Internal

- 2026-09-20: Changed the README to match the code.
- 2026-09-20: Removed the unused legacy race base left over from the race clone era.
- 2026-09-19: Changed the six race clones to Human-based races with Big & Small race trackers.
- 2026-09-19: Added a Harmony patch that multiplies health scale for the two small species.
- 2026-09-19: Changed the race-level stat bases to live on the race trackers.
- 2026-09-19: Changed Races_InsectMomo.xml to hold only the shared abstract base.
- 2026-09-19: Added a Workshop preview image.
- 2026-09-19: Changed the build to use MSBuild.
- 2026-09-06: Changed armour to live on the race trackers, not the race stat bases.
- 2026-09-06: Changed the generation swap to affect genuine insect spawns only.
- 2026-09-06: Changed the generation swap to skip a spawn that already fixes a biological age.
- 2026-09-06: Added a shared hive backstory to silence backstory warnings.
- 2026-09-06: Removed the "added to non-humanlike faction" error for hive momos.
- 2026-09-06: Removed a re-added Filth_BloodSmearInsect blood smear def.
