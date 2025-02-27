using Nautilus.Json;
using Nautilus.Options;
using Nautilus.Options.Attributes;
using System;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

namespace RandomCreatureSpawn
{
    [Menu("Random Creature Spawn (Restart game required after any change)")]
    internal class Config : ConfigFile
    {
        // Random coordinates number multiplier
        [Slider("Creature Density Multiplier", 1, 10, DefaultValue = 1, Step = 1)]
        public int CreatureDensityMultiplier = 1;

        [Toggle("Spawn new creatures each time you load a World")]
        public bool CreatureSpawnEachWorldLoading = false;

        // Path for the JSON file containing excluded creatures
        private static readonly string jsonPath = Path.Combine("./BepInEx/plugins/RandomCreatureSpawn/ExcludedCreatures.json");

        // Loading of excluded creatures from JSON file.
        public static string[] LoadExcludedCreatures()
        {
            // Check if the file exists
            if (!File.Exists(jsonPath))
            {
                Debug.LogWarning($"File {jsonPath} not found.");
                return Array.Empty<string>();
            }

            try
            {
                string json = File.ReadAllText(jsonPath);
                var data = JsonConvert.DeserializeObject<ExcludedCreaturesData>(json);
                return data?.ExcludedCreatures ?? Array.Empty<string>();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Loading error concerning excluded creatures : {ex.Message}");
                return Array.Empty<string>();
            }
        }

        private class ExcludedCreaturesData
        {
            [JsonProperty("excludedCreatures")]
            public string[] ExcludedCreatures { get; set; }
        }
    }
}
