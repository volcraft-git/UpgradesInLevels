# Feature Specification: Améliorations à la place d'objets de valeur dans les niveaux

**Feature Branch**: `001-upgrade-spawn-levels`

**Created**: 2026-11-05

**Status**: Draft

**Input**: User description: "creer un mod pour le jeux repo en utilisant thunderstore. ce mod fait apparaitre dans les differents niveaux dans des endroit normalement prévu pour des objets de valeurs des améliorations avec un taux faible configurable"

## Clarifications

### Session 2026-11-05

- Q: L'amélioration remplace-t-elle l'objet de valeur ou s'ajoute-t-elle ? → A: Elle s'ajoute :
  elle apparaît sur un emplacement d'objet de valeur resté libre, et tous les objets de valeur
  prévus par le jeu sont conservés.
- Q: Quelles améliorations peuvent apparaître ? → A: Toutes celles de la boutique du jeu de base,
  sauf « Map Player Count » (son effet ne se cumule pas, en trouver plusieurs est inutile).
- Q: Toutes les améliorations ont-elles la même chance d'apparaître ? → A: Non : plus une
  amélioration est bon marché en boutique, plus elle a de chances d'apparaître (chance
  inversement proportionnelle à son prix de base).

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Trouver une amélioration en explorant un niveau (Priority: P1)

Un joueur ayant installé le mod lance une partie de R.E.P.O. En explorant un niveau, il trouve
parfois, sur un emplacement d'objet de valeur que le jeu a laissé libre, une
amélioration (ex. santé, endurance, force de saisie). Il peut la ramasser et l'utiliser comme une
amélioration achetée en boutique.

**Why this priority**: c'est le cœur du mod ; sans cela, le mod n'a aucune utilité.

**Independent Test**: installer le mod avec un taux élevé (ex. 100 %), lancer un niveau et
vérifier que des améliorations apparaissent aux emplacements d'objets de valeur et qu'elles
s'utilisent normalement.

**Acceptance Scenarios**:

1. **Given** le mod installé avec le taux par défaut, **When** le joueur joue plusieurs niveaux,
   **Then** des améliorations apparaissent rarement, toujours à des emplacements d'objets de valeur.
2. **Given** une amélioration apparue dans le niveau, **When** le joueur l'utilise,
   **Then** l'effet est le même que pour la même amélioration achetée en boutique.
3. **Given** un niveau où des améliorations sont apparues, **When** on compte ses objets de
   valeur, **Then** ils sont aussi nombreux qu'avec le jeu non modifié.

---

### User Story 2 - Régler le taux d'apparition (Priority: P2)

Le joueur (ou l'hôte de la partie) modifie le taux d'apparition des améliorations dans le
fichier de configuration du mod, pour rendre les améliorations plus rares ou plus fréquentes.

**Why this priority**: le taux configurable est explicitement demandé ; le mod reste jouable
avec la valeur par défaut sans cette histoire.

**Independent Test**: régler le taux à 0 %, vérifier qu'aucune amélioration n'apparaît ; le
régler à 100 %, vérifier que tous les emplacements libres éligibles en reçoivent une.

**Acceptance Scenarios**:

1. **Given** un taux réglé à 0, **When** un niveau est généré, **Then** aucune amélioration
   n'apparaît et le niveau est identique au jeu non modifié.
2. **Given** une valeur de taux invalide (négative, > 100 %, non numérique), **When** le jeu
   démarre, **Then** le mod signale l'erreur clairement dans les logs et utilise une valeur
   bornée ou la valeur par défaut.
3. **Given** le mod installé pour la première fois, **When** le jeu démarre, **Then** le fichier
   de configuration est créé avec le taux par défaut et une description du réglage.

---

### User Story 3 - Installer le mod via Thunderstore (Priority: P3)

Le joueur installe le mod depuis Thunderstore avec un gestionnaire de mods, comme n'importe quel
autre mod R.E.P.O.

**Why this priority**: c'est le canal de distribution demandé, mais le mod peut être testé
localement avant publication.

**Independent Test**: installer le paquet via un gestionnaire de mods Thunderstore dans un
profil vierge, lancer le jeu, vérifier que le mod se charge.

**Acceptance Scenarios**:

1. **Given** un profil vierge dans un gestionnaire de mods, **When** le joueur installe le mod,
   **Then** ses dépendances sont installées automatiquement et le mod se charge au lancement.

---

### Edge Cases

- Emplacement d'objet de valeur trop petit pour une amélioration : l'emplacement est ignoré
  (aucune amélioration n'y apparaît).
- Aucun emplacement libre dans le niveau : aucune amélioration n'apparaît.
- Aucune amélioration éligible trouvée (ex. après une mise à jour du jeu) : l'erreur est
  enregistrée dans les logs et le mod ne fait rien.
- Niveaux sans objets de valeur (boutique, camion, lobby, arène) : aucune amélioration n'apparaît.
- Partie multijoueur : seul l'hôte décide des apparitions ; tous les joueurs voient les mêmes
  améliorations aux mêmes endroits.
- Échec de l'apparition d'une amélioration : l'erreur est enregistrée dans les logs et le niveau
  continue de se générer normalement (l'emplacement reste vide).
- Mise à jour du jeu modifiant les emplacements : le mod signale l'erreur dans les logs au lieu
  de planter le jeu.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: Le mod MUST, après que le jeu a placé les objets de valeur d'un niveau, faire
  apparaître une amélioration sur chaque emplacement d'objet de valeur resté libre, selon un taux
  de probabilité par emplacement. Les objets de valeur prévus par le jeu MUST être conservés.
- **FR-002**: Le taux MUST être réglable dans le fichier de configuration du mod, en pourcentage
  de 0 à 100, avec une valeur par défaut faible de 2 %.
- **FR-003**: L'amélioration apparue MUST être choisie aléatoirement parmi les améliorations de
  la boutique du jeu de base, « Map Player Count » exclue.
- **FR-003a**: La chance de chaque amélioration MUST être inversement proportionnelle à son prix
  de base en boutique : une amélioration deux fois moins chère a deux fois plus de chances
  d'apparaître.
- **FR-004**: Une amélioration apparue MUST se comporter exactement comme la même amélioration
  achetée en boutique (ramassage, utilisation, effet, synchronisation entre joueurs).
- **FR-005**: Le mod MUST n'agir que dans les niveaux contenant des objets de valeur.
- **FR-006**: En multijoueur, seul l'hôte MUST décider des apparitions, et le résultat MUST être
  identique pour tous les joueurs.
- **FR-007**: Toute erreur du mod (configuration invalide, échec d'apparition) MUST être
  enregistrée dans les logs du jeu avec un message clair, sans être ignorée en silence, et sans
  empêcher le niveau de se générer.
- **FR-008**: Le mod MUST être publié sous forme de paquet Thunderstore installable par un
  gestionnaire de mods, avec ses dépendances déclarées.

### Key Entities

- **Emplacement d'objet de valeur**: endroit d'un niveau où le jeu peut faire apparaître un objet
  de valeur ; a une taille qui limite ce qui peut y apparaître. Il est soit occupé par un objet de
  valeur, soit libre (seuls les emplacements libres reçoivent une amélioration).
- **Amélioration**: objet consommable qui améliore durablement une statistique du joueur
  (santé, endurance, saut, etc.). A un prix de base en boutique, qui détermine sa chance
  d'apparaître.
- **Configuration du mod**: fichier contenant le taux d'apparition.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: Quel que soit le taux, les niveaux conservent 100 % de leurs objets de valeur
  habituels.
- **SC-002**: Sur un grand nombre d'emplacements, la proportion d'améliorations apparues
  correspond au taux configuré, à quelques points près.
- **SC-003**: Avec un taux de 0 %, aucun niveau ne contient d'amélioration ajoutée par le mod.
- **SC-003a**: « Map Player Count » n'apparaît jamais ; sur un grand nombre d'apparitions, les
  améliorations les moins chères sont les plus fréquentes, en proportion inverse de leur prix.
- **SC-004**: En multijoueur, 100 % des améliorations apparues sont visibles et utilisables par
  tous les joueurs.
- **SC-005**: Le mod ne provoque aucun plantage ni blocage de la génération d'un niveau.

## Assumptions

- Le mod cible la version actuelle de R.E.P.O. sur PC et l'écosystème de mods Thunderstore
  habituel du jeu.
- Le mod n'est requis que chez l'hôte : les améliorations sont des objets du jeu de base,
  répliqués aux autres joueurs par le jeu (confirmé en planification, research §4).
- Les améliorations ajoutées par d'autres mods ne sont pas prises en compte.
- Pas de limite du nombre d'améliorations par niveau : le taux faible suffit.
- Pas de tests automatisés (constitution, principe III) : vérification manuelle en jeu.
