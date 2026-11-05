---

description: "Task list for UpgradesInLevels (R.E.P.O. mod)"
---

# Tasks: Améliorations à la place d'objets de valeur dans les niveaux

**Input**: Design documents from `/specs/001-upgrade-spawn-levels/`

**Prerequisites**: plan.md, spec.md, research.md, data-model.md, contracts/, quickstart.md

**Tests**: aucun (constitution, principe III). Chaque phase se termine par une vérification
manuelle en jeu décrite dans [quickstart.md](quickstart.md).

**Organization**: tâches groupées par user story.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: parallélisable (fichiers différents, pas de dépendance)
- **[Story]**: US1, US2, US3 (spec.md)

## Repères

- Jeu : `C:\Program Files (x86)\Steam\steamapps\common\REPO` (DLL dans `REPO_Data\Managed`)
- Profil : `%APPDATA%\Thunderstore Mod Manager\DataFolder\REPO\profiles\Default`
  (BepInEx dans `BepInEx\core`, plugins dans `BepInEx\plugins`, log `BepInEx\LogOutput.log`)
- Nom du mod / GUID : `UpgradesInLevels`, version `1.0.0`

---

## Phase 1: Setup

- [X] T001 Installer le SDK .NET 8 (`winget install Microsoft.DotNet.SDK.8`) et vérifier `dotnet --version` ; demander l'accord de l'utilisateur avant l'installation
- [X] T002 Créer `UpgradesInLevels.csproj` à la racine : `TargetFramework` `netstandard2.1`, `AssemblyName` `UpgradesInLevels`, `LangVersion` `latest`, propriétés `GameDir` et `ProfileDir` (chemins des Repères, surchargeables), références `Private=false` vers `$(GameDir)\REPO_Data\Managed\Assembly-CSharp.dll`, `UnityEngine.dll`, `UnityEngine.CoreModule.dll`, `PhotonUnityNetworking.dll`, `PhotonRealtime.dll`, et `$(ProfileDir)\BepInEx\core\BepInEx.dll`, `0Harmony.dll` ; `Compile Include="src/**/*.cs"` uniquement
- [X] T003 Ajouter `.gitignore` à la racine (`bin/`, `obj/`, `dist/`)

---

## Phase 2: Foundational (bloquant)

- [X] T004 Créer `src/Plugin.cs` : classe `Plugin : BaseUnityPlugin` avec `[BepInPlugin("UpgradesInLevels", "UpgradesInLevels", "1.0.0")]` ; champ statique `Log` (`ManualLogSource` = `Logger`) ; champ statique `SpawnChancePercent` (`ConfigEntry<float>`) lié à `Config.Bind("General", "SpawnChancePercent", 2f, "Chance (en %) qu'un emplacement d'objet de valeur libre reçoive une amélioration.")` ; dans `Awake`, `new Harmony("UpgradesInLevels").PatchAll()` puis log Info « chargé »
- [X] T005 Ajouter dans `UpgradesInLevels.csproj` une cible `AfterTargets="Build"` qui copie `$(TargetPath)` vers `$(ProfileDir)\BepInEx\plugins\UpgradesInLevels\`

**Checkpoint**: `dotnet build -c Release` réussit ; au lancement du jeu, le log contient « chargé ».

---

## Phase 3: User Story 1 - Trouver une amélioration dans un niveau (P1) 🎯 MVP

**Goal**: des améliorations apparaissent sur des emplacements libres, en plus des objets de valeur.

**Independent Test**: quickstart scénarios 1, 2, 3, 6, 7, 8 avec `SpawnChancePercent = 100`.

- [X] T006 [US1] Créer `src/UpgradeSpawner.cs` : classe statique `[HarmonyPatch(typeof(ValuableDirector), nameof(ValuableDirector.VolumesAndSwitchSetup))]` avec un `[HarmonyPostfix] static void Postfix()` qui sort immédiatement si `!SemiFunc.IsMasterClientOrSingleplayer() || !SemiFunc.RunIsLevel()`, sinon appelle `SpawnUpgrades()` dans un `try/catch (Exception e)` qui logue `Plugin.Log.LogError` avec « échec de l'ajout d'améliorations dans le niveau » + `e` (jamais de catch vide, ne jamais relancer : la génération du niveau doit continuer)
- [X] T007 [US1] Dans `src/UpgradeSpawner.cs`, méthode `GetEligibleUpgrades()` : depuis `StatsManager.instance.itemDictionary.Values`, garder les `Item` avec `itemType == SemiFunc.itemType.item_upgrade`, `!disabled`, `value != null && value.valueMin > 0`, et dont `name` ne contient pas `Map Player Count` (comparaison insensible à la casse) ; si la liste est vide, `LogError` « aucune amélioration éligible trouvée » et le postfix ne fait rien
- [X] T008 [US1] Dans `src/UpgradeSpawner.cs`, méthode `PickWeighted(List<Item>)` : roulette pondérée avec poids `1f / item.value.valueMin` et `UnityEngine.Random.value`
- [X] T009 [US1] Dans `src/UpgradeSpawner.cs`, méthode `GetFreeVolumes()` : `Object.FindObjectsByType<ValuableVolume>(FindObjectsSortMode.None)` sans les `VolumeType == ValuableVolume.Type.Tiny` ; un volume est occupé si un `ValuableObject` ou un `CosmeticWorldObject` (via `FindObjectsByType`) est à moins de 0,1 m de `volume.transform.position`
- [X] T010 [US1] Dans `src/UpgradeSpawner.cs`, méthode `Spawn(Item, ValuableVolume)` reproduisant `ValuableDirector.SpawnValuable` : rotation `volume.transform.rotation * Quaternion.Euler(item.spawnRotationOffset)` ; si `SemiFunc.IsMultiplayer()` alors `PhotonNetwork.InstantiateRoomObject(item.prefab.ResourcePath, pos, rot)` sinon `Object.Instantiate(item.prefab.Prefab, pos, rot)` ; chaque appel dans un `try/catch` qui logue `LogError` avec le nom de l'item et la position, puis continue avec l'emplacement suivant (si le type de `item.prefab` diffère de `PrefabRef`, adapter et le noter dans research.md §4)
- [X] T011 [US1] Dans `src/UpgradeSpawner.cs`, `SpawnUpgrades()` : pour chaque volume libre, si `Random.value * 100f < chance` alors `Spawn(PickWeighted(...), volume)` ; à la fin `LogInfo` « {libres} emplacements libres, {n} améliorations ajoutées »
- [ ] T012 [US1] Vérification manuelle : `dotnet build -c Release`, puis quickstart scénarios 1, 2, 3, 6, 8 en solo et 7 en multijoueur (mod chez l'hôte seulement) ; reporter tout écart dans research.md

**Checkpoint**: MVP jouable.

---

## Phase 4: User Story 2 - Régler le taux (P2)

**Goal**: taux configurable, valeurs invalides signalées.

**Independent Test**: quickstart scénarios 4 et 5.

- [X] T013 [US2] Dans `src/UpgradeSpawner.cs` (ou `src/Plugin.cs` si plus lisible), lire `Plugin.SpawnChancePercent.Value` au début de chaque niveau ; si `< 0` ou `> 100` (ou NaN), `LogWarning` « SpawnChancePercent={valeur} hors [0, 100], valeur bornée à {bornée} » et utiliser la valeur bornée (NaN → 0) ; si la valeur est 0, sortir sans rien faire
- [ ] T014 [US2] Vérification manuelle : quickstart scénarios 4 et 5 ; vérifier que `BepInEx/config/UpgradesInLevels.cfg` est créé avec la description (contracts/config-and-package.md)

---

## Phase 5: User Story 3 - Paquet Thunderstore (P3)

**Goal**: zip installable par un gestionnaire de mods.

**Independent Test**: installer le zip dans un profil vierge (import local) ; le mod se charge.

- [X] T015 [P] [US3] Créer `package/manifest.json` exactement comme dans contracts/config-and-package.md (`name` en `[A-Za-z0-9_]`, `version_number` `1.0.0`, `website_url` `""`, `description` ≤ 250 caractères, dépendance `BepInEx-BepInExPack-5.4.2305`)
- [X] T016 [P] [US3] Créer `package/README.md` (UTF-8) en anglais : fonctionnement, mod côté hôte seulement, réglage `SpawnChancePercent` (défaut 2), Map Player Count exclue, poids selon le prix
- [X] T017 [P] [US3] Créer `package/CHANGELOG.md` avec l'entrée `1.0.0`
- [X] T018 [P] [US3] Créer `package/icon.png` 256×256 PNG (icône simple générée par script PowerShell `System.Drawing`, ou fournie par l'utilisateur)
- [X] T019 [US3] Ajouter dans `UpgradesInLevels.csproj` une cible `Package` (`DependsOnTargets="Build"`) : copier `package/*` et `$(TargetPath)` dans `obj/package/`, puis `ZipDirectory` vers `dist/UpgradesInLevels-1.0.0.zip` (`Overwrite="true"`)
- [ ] T020 [US3] Vérification manuelle : `dotnet build -c Release -t:Package`, valider le manifest sur https://thunderstore.io/tools/manifest-v1-validator/, importer le zip dans un profil vierge et lancer le jeu

---

## Phase 6: Polish

- [X] T021 Relire `src/*.cs` contre la constitution (noms explicites, aucun catch vide, commentaires « pourquoi » seulement) et mettre à jour quickstart.md si une commande a changé

---

## Dependencies & Execution Order

- Phase 1 → Phase 2 → US1 → US2 (même fichier que US1) → Polish
- US3 : T015–T018 peuvent démarrer dès la Phase 1 ; T019–T020 après la Phase 2

### Parallel Opportunities

```text
T015, T016, T017, T018 (fichiers de paquet distincts) en parallèle de la Phase 3
```

Dans US1, toutes les tâches touchent `src/UpgradeSpawner.cs` : séquentielles.

## Implementation Strategy

1. Phases 1–2, puis US1 → vérifier en jeu (MVP, taux par défaut lu depuis la config)
2. US2 : validation du taux
3. US3 : paquet, validation, publication manuelle par l'utilisateur
