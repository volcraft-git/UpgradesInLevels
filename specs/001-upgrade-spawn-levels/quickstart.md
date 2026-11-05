# Quickstart : vérification manuelle

Pas de tests automatisés (constitution, principe III). Vérification en jeu.

## Prérequis

- SDK .NET 8 : `winget install Microsoft.DotNet.SDK.8`
- R.E.P.O. installé (Steam) et profil `Default` de Thunderstore Mod Manager avec BepInExPack.

## Build et installation locale

```sh
dotnet build -c Release
```

La compilation copie la DLL dans
`%APPDATA%/Thunderstore Mod Manager/DataFolder/REPO/profiles/Default/BepInEx/plugins/UpgradesInLevels/`.
Lancer le jeu depuis le gestionnaire de mods (profil `Default`).

## Scénarios

| # | Réglage | Action | Résultat attendu | Spec |
|---|---|---|---|---|
| 1 | `SpawnChancePercent = 100` | Lancer un niveau solo | Des améliorations sur les emplacements libres ; log Info avec les nombres | US1, FR-001 |
| 2 | 100 | Utiliser une amélioration trouvée | Même effet qu'en boutique | FR-004 |
| 3 | 100 | Comparer la valeur totale du niveau (carte) avec / sans mod | Objets de valeur conservés | SC-001 |
| 4 | 0 | Lancer un niveau | Aucune amélioration ajoutée | SC-003 |
| 5 | 150 puis -5 | Lancer le jeu | Warning dans le log, valeur bornée | US2 |
| 6 | 100 | Boutique / lobby | Rien n'apparaît | FR-005 |
| 7 | 100 | Multijoueur, mod chez l'hôte seulement | Le client voit et utilise les améliorations | FR-006, SC-004 |
| 8 | 100 | Plusieurs niveaux | Jamais de Map Player Count ; les moins chères plus fréquentes | SC-003a |

Logs : `BepInEx/LogOutput.log` du profil.

## Paquet

```sh
dotnet build -c Release -t:Package
```

Produit `dist/UpgradesInLevels-1.0.0.zip`. Vérifier `manifest.json` sur
https://thunderstore.io/tools/manifest-v1-validator/, puis envoyer via
https://thunderstore.io/package/create/ (communauté R.E.P.O.).
