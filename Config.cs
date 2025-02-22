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
        // Multiplicateur du nombre de coordonnées à générer
        [Slider("Creature Density Multiplier", 1, 10, DefaultValue = 1, Step = 1)]
        public int CreatureDensityMultiplier = 1;

        // Définition du chemin pour le fichier JSON des créatures exclus
        private static readonly string jsonPath = Path.Combine("./BepInEx/plugins/RandomCreatureSpawn/ExcludedCreatures.json");

        // Chargement des créatures exclus depuis le JSON
        public static string[] LoadExcludedCreatures()
        {
            // Vérification de l'existence du fichier
            if (!File.Exists(jsonPath))
            {
                Debug.LogWarning($"Fichier {jsonPath} introuvable. Un nouveau sera créé avec les valeurs par défaut.");
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
                Debug.LogError($"Erreur lors du chargement des créatures exclues : {ex.Message}");
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
