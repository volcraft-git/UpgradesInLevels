<!--
Sync Impact Report
- Version: 1.1.0 → 1.2.0
- Section modifiée : Contraintes de code (ajout des sources de documentation Thunderstore)
- Sections ajoutées / retirées : aucune
- TODO : aucun
-->
# mod-repo-upgrade-spawn-level Constitution

## Core Principles

### I. Maintenabilité par l'IA (NON NÉGOCIABLE)
Le code DOIT pouvoir être lu, compris et modifié facilement par Claude.
- Noms explicites pour fichiers, fonctions et variables ; pas d'abréviations obscures.
- Fichiers courts et à responsabilité unique ; une fonctionnalité se trouve en un seul endroit.
- Structure de dossiers prévisible et conventions constantes dans tout le projet.
- Commentaires uniquement pour expliquer le *pourquoi* que le code ne montre pas.

Justification : Claude est le mainteneur principal ; un code prévisible réduit les erreurs
et le contexte nécessaire pour chaque modification.

### II. Simplicité
On écrit le minimum de code qui résout entièrement le besoin.
- Pas d'abstraction, d'option ou de configuration « pour plus tard ».
- Bibliothèque standard et dépendances existantes avant toute nouvelle dépendance.
- Supprimer vaut mieux qu'ajouter.

Justification : moins de code = moins à relire et à maintenir.

### III. Pas de tests automatisés
Aucun test automatisé (unitaire, intégration, e2e) n'est requis ni attendu.
- Les plans et listes de tâches NE DOIVENT PAS inclure de tâches de test.
- La vérification se fait manuellement en lançant le projet.

Justification : décision explicite du propriétaire du projet.

### IV. Gestion explicite des erreurs (NON NÉGOCIABLE)
Toute erreur DOIT être gérée explicitement et remontée correctement.
- Aucune erreur ne DOIT être avalée en silence (pas de `catch` vide, pas d'échec ignoré).
- Une erreur non traitable localement DOIT être propagée à l'appelant, avec son contexte
  (quelle opération, quelle donnée) et sans perdre l'erreur d'origine.
- L'erreur finale DOIT être visible par l'utilisateur ou dans les logs, avec un message clair.

Justification : une erreur cachée rend les bugs introuvables, pour un humain comme pour Claude.

## Contraintes de code

- Tout ce qui touche à Thunderstore (format du paquet, `manifest.json`, icône, README,
  dépendances, publication) DOIT suivre la documentation officielle :
  - Wiki : https://wiki.thunderstore.io/
  - API : https://thunderstore.io/api/docs/
  En cas de doute, consulter ces sources plutôt que de supposer.

- Les entrées externes (fichiers, configuration, données de mod) DOIVENT être validées.

## Workflow de développement

- Chaque changement reste petit et ciblé sur une seule intention.
- Avant de modifier une fonction, identifier tous ses appelants.

## Governance

Cette constitution prime sur toute autre pratique du projet. Toute modification passe par
`/speckit-constitution`, avec mise à jour de la version selon le versionnage sémantique
(MAJOR : retrait ou redéfinition d'un principe ; MINOR : ajout de principe ou de section ;
PATCH : reformulation). Les plans (`/speckit-plan`) DOIVENT vérifier leur conformité
avec ces principes.

**Version**: 1.2.0 | **Ratified**: 2026-11-05 | **Last Amended**: 2026-11-05
