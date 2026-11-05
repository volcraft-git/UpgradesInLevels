# Implementation Plan: Améliorations à la place d'objets de valeur dans les niveaux

**Branch**: `001-upgrade-spawn-levels` | **Date**: 2026-11-05 | **Spec**: [spec.md](spec.md)

**Input**: Feature specification from `/specs/001-upgrade-spawn-levels/spec.md`

## Summary

Plugin BepInEx (côté hôte) pour R.E.P.O. : une fois que le jeu a placé objets de valeur et
objets cosmétiques, chaque emplacement d'objet de valeur resté libre (hors taille Tiny) a
`SpawnChancePercent` % de chances de recevoir une amélioration de la boutique, tirée au sort avec
un poids `1 / prix de base`, « Map Player Count » exclue. L'apparition passe par le même mécanisme
réseau que le jeu, donc les autres joueurs n'ont pas besoin du mod. Distribué en paquet
Thunderstore. Détails : [research.md](research.md).

## Technical Context

**Language/Version**: C# / `netstandard2.1`, SDK .NET 8 pour compiler

**Primary Dependencies**: BepInEx 5 (BepInExPack 5.4.2305), Harmony (inclus), DLL du jeu
(`Assembly-CSharp`, `UnityEngine.CoreModule`, `PhotonUnityNetworking`)

**Storage**: fichier de configuration BepInEx

**Testing**: aucun test automatisé (constitution III) — vérification manuelle, voir
[quickstart.md](quickstart.md)

**Target Platform**: R.E.P.O. PC (Windows), Unity/Mono

**Project Type**: plugin de jeu (mod BepInEx)

**Performance Goals**: traitement unique par niveau, imperceptible (quelques centaines
d'emplacements au plus)

**Constraints**: ne jamais bloquer la génération d'un niveau ; hôte uniquement

**Scale/Scope**: 2 fichiers C#, 1 csproj, 4 fichiers de paquet

## Constitution Check

| Principe | Statut | Comment |
|---|---|---|
| I. Maintenabilité par l'IA | ✅ | 2 fichiers courts, noms explicites, un seul point d'accroche |
| II. Simplicité | ✅ | Aucune dépendance hors BepInEx ; REPOLib écarté ; pas d'option au-delà du taux |
| III. Pas de tests | ✅ | Aucune tâche de test ; quickstart manuel |
| IV. Erreurs explicites | ✅ | `try/catch` qui logue avec contexte ; taux invalide signalé ; aucun `catch` vide |
| Contraintes : entrées validées | ✅ | Taux borné avec avertissement ; liste d'améliorations vide signalée |
| Contraintes : doc Thunderstore | ✅ | Format du paquet tiré du wiki (research §8). API docs non accessibles (connexion requise) |

Re-vérifié après la phase 1 : aucun changement.

## Project Structure

### Documentation (this feature)

```text
specs/001-upgrade-spawn-levels/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/config-and-package.md
└── tasks.md             # /speckit-tasks
```

### Source Code (repository root)

```text
UpgradesInLevels.csproj      # références au jeu/BepInEx, copie vers le profil, cible Package
src/
├── Plugin.cs                # point d'entrée BepInEx : config, logger, Harmony
└── UpgradeSpawner.cs        # postfix ValuableDirector.VolumesAndSwitchSetup + logique
package/
├── manifest.json
├── icon.png
├── README.md
└── CHANGELOG.md
```

**Structure Decision**: projet unique à la racine ; la logique tient dans `UpgradeSpawner.cs`,
l'amorçage dans `Plugin.cs`. Pas de dossier de tests.

## Complexity Tracking

Aucune violation.
