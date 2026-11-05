# UpgradesInLevels

Upgrades can now be found while exploring levels.

After the game places its valuables, every **empty** valuable spot has a small chance to receive a
random upgrade. Valuables are never removed: upgrades are added on top.

- Any upgrade from the vanilla shop can appear, except **Map Player Count** (it does not stack).
- Cheaper upgrades are more common: the chance of each upgrade is inversely proportional to its
  base shop price.
- Upgrades work exactly like the ones bought in the shop.
- Only regular levels are affected (not the shop, lobby, truck or arena).

## Multiplayer

**Host-only.** Only the host needs this mod; other players see and use the upgrades without it.

## Configuration

`BepInEx/config/UpgradesInLevels.cfg` (created on first launch, editable from your mod manager):

| Setting | Default | Description |
|---|---|---|
| `SpawnChancePercent` | `2` | Chance (0–100 %) for each empty valuable spot to get an upgrade. |

Out-of-range values are clamped and a warning is written to the BepInEx log.
