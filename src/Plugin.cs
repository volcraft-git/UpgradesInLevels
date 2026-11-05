using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;

namespace UpgradesInLevels
{
    [BepInPlugin(Guid, Guid, Version)]
    public class Plugin : BaseUnityPlugin
    {
        public const string Guid = "UpgradesInLevels";
        public const string Version = "1.0.0";

        internal static ManualLogSource Log;
        internal static ConfigEntry<float> SpawnChancePercent;

        private void Awake()
        {
            Log = Logger;
            SpawnChancePercent = Config.Bind(
                "General",
                "SpawnChancePercent",
                2f,
                "Chance (en %) qu'un emplacement d'objet de valeur libre reçoive une amélioration.");

            new Harmony(Guid).PatchAll();
            Log.LogInfo($"{Guid} {Version} chargé (SpawnChancePercent = {SpawnChancePercent.Value}).");
        }
    }
}
