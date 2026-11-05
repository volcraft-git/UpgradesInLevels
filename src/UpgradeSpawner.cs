using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Photon.Pun;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace UpgradesInLevels
{
    // Le jeu appelle VolumesAndSwitchSetup juste après avoir placé objets de valeur et objets
    // cosmétiques : les emplacements encore vides à ce moment sont réellement libres.
    [HarmonyPatch(typeof(ValuableDirector), nameof(ValuableDirector.VolumesAndSwitchSetup))]
    internal static class UpgradeSpawner
    {
        private const float OccupiedDistance = 0.1f;
        private const string ExcludedUpgradeName = "Map Player Count";

        [HarmonyPostfix]
        private static void Postfix()
        {
            if (!SemiFunc.IsMasterClientOrSingleplayer() || !SemiFunc.RunIsLevel())
                return;

            try
            {
                SpawnUpgrades();
            }
            catch (Exception e)
            {
                // Pas de relance : la génération du niveau doit continuer.
                Plugin.Log.LogError($"Échec de l'ajout d'améliorations dans le niveau : {e}");
            }
        }

        private static void SpawnUpgrades()
        {
            float chance = GetSpawnChancePercent();
            if (chance <= 0f)
                return;

            List<Item> upgrades = GetEligibleUpgrades();
            if (upgrades.Count == 0)
            {
                Plugin.Log.LogError("Aucune amélioration éligible trouvée dans StatsManager.itemDictionary : rien n'est ajouté.");
                return;
            }

            List<ValuableVolume> freeVolumes = GetFreeVolumes();
            int spawned = 0;
            foreach (ValuableVolume volume in freeVolumes)
            {
                if (Random.value * 100f < chance && Spawn(PickWeighted(upgrades), volume))
                    spawned++;
            }

            Plugin.Log.LogInfo($"{freeVolumes.Count} emplacements libres, {spawned} améliorations ajoutées (taux {chance} %).");
        }

        private static float GetSpawnChancePercent()
        {
            float value = Plugin.SpawnChancePercent.Value;
            float clamped = float.IsNaN(value) ? 0f : Mathf.Clamp(value, 0f, 100f);
            if (clamped != value)
                Plugin.Log.LogWarning($"SpawnChancePercent={value} hors [0, 100], valeur bornée à {clamped}.");
            return clamped;
        }

        private static List<Item> GetEligibleUpgrades()
        {
            return StatsManager.instance.itemDictionary.Values
                .Where(item => item != null
                    && item.itemType == SemiFunc.itemType.item_upgrade
                    && !item.disabled
                    && item.value != null
                    && item.value.valueMin > 0f
                    && item.name.IndexOf(ExcludedUpgradeName, StringComparison.OrdinalIgnoreCase) < 0)
                .ToList();
        }

        // Poids inversement proportionnel au prix de base : moins cher = plus fréquent.
        private static Item PickWeighted(List<Item> upgrades)
        {
            float totalWeight = upgrades.Sum(item => 1f / item.value.valueMin);
            float roll = Random.value * totalWeight;
            foreach (Item item in upgrades)
            {
                roll -= 1f / item.value.valueMin;
                if (roll <= 0f)
                    return item;
            }
            return upgrades[upgrades.Count - 1]; // arrondi flottant
        }

        private static List<ValuableVolume> GetFreeVolumes()
        {
            List<Vector3> occupied = Object.FindObjectsByType<ValuableObject>(FindObjectsSortMode.None)
                .Select(o => o.transform.position)
                .Concat(Object.FindObjectsByType<CosmeticWorldObject>(FindObjectsSortMode.None)
                    .Select(o => o.transform.position))
                .ToList();

            return Object.FindObjectsByType<ValuableVolume>(FindObjectsSortMode.None)
                .Where(volume => volume.VolumeType != ValuableVolume.Type.Tiny
                    && !occupied.Any(p => Vector3.Distance(p, volume.transform.position) < OccupiedDistance))
                .ToList();
        }

        // Reproduit ValuableDirector.SpawnValuable : objet réseau en multijoueur, local en solo.
        private static bool Spawn(Item item, ValuableVolume volume)
        {
            Vector3 position = volume.transform.position;
            Quaternion rotation = volume.transform.rotation * item.spawnRotationOffset;
            try
            {
                if (SemiFunc.IsMultiplayer())
                    PhotonNetwork.InstantiateRoomObject(item.prefab.ResourcePath, position, rotation);
                else
                    Object.Instantiate(item.prefab.Prefab, position, rotation);
                return true;
            }
            catch (Exception e)
            {
                Plugin.Log.LogError($"Échec de l'apparition de '{item.name}' à {position} : {e}");
                return false;
            }
        }
    }
}
