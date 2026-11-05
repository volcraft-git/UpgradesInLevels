# Contrats exposés

## Fichier de configuration

Chemin : `BepInEx/config/<GUID>.cfg`, créé au premier lancement.

```ini
[General]

## Chance (en %) qu'un emplacement d'objet de valeur libre reçoive une amélioration.
# Setting type: Single
# Default value: 2
SpawnChancePercent = 2
```

- Valeur < 0 ou > 100 : avertissement dans le log, valeur bornée à 0 ou 100.
- Valeur non numérique : BepInEx logue l'erreur et utilise la valeur par défaut.

## Paquet Thunderstore (racine du zip)

```text
manifest.json
icon.png          # 256x256, PNG
README.md         # UTF-8
CHANGELOG.md
UpgradesInLevels.dll
```

`manifest.json` :

```json
{
  "name": "UpgradesInLevels",
  "version_number": "1.0.0",
  "website_url": "",
  "description": "Upgrades can rarely spawn on empty valuable spots in levels. Cheaper upgrades are more common. Chance is configurable. Host-only.",
  "dependencies": ["BepInEx-BepInExPack-5.4.2305"]
}
```

## Messages de log (préfixe `[UpgradesInLevels]`)

| Niveau | Quand |
|---|---|
| Info | Par niveau : nombre d'emplacements libres éligibles et d'améliorations ajoutées |
| Warning | Taux hors [0, 100] borné |
| Error | Aucune amélioration éligible trouvée ; échec d'une apparition ; exception inattendue (avec pile) |
