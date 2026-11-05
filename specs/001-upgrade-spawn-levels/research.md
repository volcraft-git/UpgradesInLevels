# Research: Améliorations dans les niveaux

Sources : `Assembly-CSharp.dll` du jeu installé (lecture des métadonnées et des appels IL,
Steam `common/REPO`), profil Thunderstore Mod Manager `Default`, wiki Thunderstore
(creating-a-package, packaging-your-mods, your-first-upload). La page
https://thunderstore.io/api/docs/ demande une connexion : non lue, aucune info API utilisée.

## 1. Chargeur de mods et langage

- **Decision**: BepInEx 5 (paquet `BepInEx-BepInExPack-5.4.2305`, déjà installé dans le
  profil) + Harmony (fourni par BepInEx), plugin C# ciblant `netstandard2.1`.
- **Rationale**: standard de l'écosystème R.E.P.O. sur Thunderstore ; aucune autre dépendance.
- **Alternatives considered**: REPOLib (`Items.SpawnItem`) — rejeté : le jeu sait déjà faire
  apparaître un objet en réseau en 2 lignes (voir §4), une dépendance de plus n'apporte rien.

## 2. Où et quand agir

- **Decision**: Postfix Harmony sur `ValuableDirector.VolumesAndSwitchSetup`, uniquement si
  `SemiFunc.IsMasterClientOrSingleplayer()` et `SemiFunc.RunIsLevel()`.
- **Rationale**: dans la coroutine `ValuableDirector.SetupHost`, le jeu mélange les
  `ValuableVolume` par taille, place les objets de valeur sur les N premiers, puis place des
  `CosmeticWorldObject` sur les suivants, puis appelle `VolumesAndSwitchSetup`. À cet instant,
  tous les objets du jeu sont placés et les emplacements restants sont réellement libres.
  `RunIsLevel` exclut boutique, lobby, camion, arène, tutoriel.
- **Alternatives considered**: postfix sur `SetupHost` — rejeté (coroutine : le postfix
  s'exécute avant la fin). Lire l'index interne de la coroutine — rejeté (variable locale non
  accessible, fragile).

## 3. Détecter un emplacement libre

- **Decision**: un `ValuableVolume` est libre si aucun `ValuableObject` ni `CosmeticWorldObject`
  n'a sa position à moins de 0,1 m de la position de l'emplacement. Les emplacements de taille
  `Tiny` sont exclus (trop petits pour une amélioration).
- **Rationale**: `SpawnValuable` et `SpawnCosmeticWorldObject` instancient l'objet exactement à
  `volume.transform.position`. Comparaison simple et indépendante des listes internes.
- **Alternatives considered**: `Physics.OverlapBox` — rejeté (attrape le décor, plus complexe).

## 4. Faire apparaître une amélioration (multijoueur)

- **Decision**: reproduire `ValuableDirector.SpawnValuable` : en multijoueur
  `PhotonNetwork.InstantiateRoomObject(item.prefab.ResourcePath, pos, rot)`, en solo
  `Object.Instantiate(item.prefab.Prefab, pos, rot)`.
- **Rationale**: objets du jeu de base, chargés depuis ses ressources : Photon les réplique chez
  tous les joueurs. **Le mod n'est requis que chez l'hôte** (point différé de la spec résolu).
- **Confirmé à la compilation**: `Item.prefab` expose `ResourcePath` et `Prefab` (comme pour les
  valuables) ; `Item.spawnRotationOffset` est un `Quaternion` (rotation = volume × offset).

## 5. Liste des améliorations et poids

- **Decision**: `StatsManager.instance.itemDictionary.Values` filtré sur
  `itemType == SemiFunc.itemType.item_upgrade`, `!disabled`, et nom ne contenant pas
  `Map Player Count`. Poids = `1 / item.value.valueMin` (prix de base).
- **Rationale**: dictionnaire déjà chargé par le jeu ; `Value.valueMin` est le prix de base
  avant les hausses de la boutique (`ShopManager.upgradeValueIncrease`), ce qui répond à la
  question « quel prix » de la clarification.
- **Alternatives considered**: liste de noms codée en dur — rejeté (casse à chaque mise à jour).

## 6. Configuration

- **Decision**: `ConfigFile` de BepInEx, entrée `[General] SpawnChancePercent` (float,
  défaut 2). Valeur hors [0, 100] : avertissement dans le log + valeur bornée.
- **Rationale**: standard BepInEx, fichier créé automatiquement, éditable via le gestionnaire
  de mods. Pas d'`AcceptableValueRange` : il borne en silence (contraire au principe IV).

## 7. Erreurs et logs

- **Decision**: `ManualLogSource` de BepInEx (`LogOutput.log` + console). Le postfix entier est
  dans un `try/catch` qui logue l'exception avec son contexte ; chaque apparition aussi, pour
  qu'un échec n'empêche ni les suivantes ni la génération du niveau.

## 8. Paquet Thunderstore

- **Decision**: zip contenant à la racine `manifest.json`, `icon.png` (256×256 PNG),
  `README.md`, `CHANGELOG.md` et la DLL. Généré par une cible MSBuild `ZipDirectory`.
- **Règles (wiki)**: `name` en `[A-Za-z0-9_]` (≤ 128) ; `version_number` `Major.Minor.Patch` ;
  `description` ≤ 250 caractères ; `website_url` obligatoire (`""` accepté) ; dépendances au
  format `Équipe-Nom-Version`. Envoi via le formulaire https://thunderstore.io/package/create/
  (équipe, communauté R.E.P.O., catégories). Vérifier le manifest avec
  https://thunderstore.io/tools/manifest-v1-validator/.

## 9. Outils de build

- **Constat**: le SDK .NET n'est pas installé sur la machine.
- **Decision**: prérequis `winget install Microsoft.DotNet.SDK.8`. Les DLL du jeu et de BepInEx
  sont référencées depuis leurs dossiers installés (chemins en propriétés MSBuild), pas de
  NuGet spécifique.
