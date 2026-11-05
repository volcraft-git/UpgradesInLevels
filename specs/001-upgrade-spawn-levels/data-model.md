# Data Model

Aucune donnée persistée par le mod. Entités du jeu lues à l'exécution :

| Entité | Source dans le jeu | Champs utilisés | Règles |
|---|---|---|---|
| Emplacement | `ValuableVolume` | `transform.position`, `transform.rotation`, `VolumeType` | Éligible si `VolumeType != Tiny` et libre (rien à < 0,1 m) |
| Objet occupant | `ValuableObject`, `CosmeticWorldObject` | `transform.position` | Rend un emplacement occupé |
| Amélioration | `Item` dans `StatsManager.instance.itemDictionary` | `itemType`, `disabled`, `name`, `value.valueMin`, `prefab`, `spawnRotationOffset` | `itemType == item_upgrade`, `!disabled`, nom sans `Map Player Count`, `valueMin > 0` |
| Configuration | fichier BepInEx | `SpawnChancePercent` (float) | Borné à [0, 100] avec avertissement |

Poids d'une amélioration : `1 / value.valueMin`. Tirage : roulette pondérée.

Cycle par niveau (hôte uniquement) : niveau généré → objets du jeu placés → pour chaque
emplacement libre éligible, tirage `Random.value * 100 < SpawnChancePercent` → tirage pondéré
d'une amélioration → apparition réseau.
