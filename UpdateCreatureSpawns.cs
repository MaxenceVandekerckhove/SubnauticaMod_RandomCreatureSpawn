using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RandomCreatureSpawn
{
    public class UpdateCreatureSpawns
    {
        public static void RefreshSpawns(float densityMultiplier)
        {
            List<Biome> biomes = BiomeLoader.LoadJson();

            // Vérification de la présence de biome dans le fichier BiomeCoordinates.json
            if (biomes == null || biomes.Count == 0)
            {
                Plugin.Logger.LogWarning("Aucun biome trouvé dans le fichier /plugins/RandomCreatureSpawns/BiomeCoordinates.json.");
                return;
            }

            // Récupération des créatures valables à faire appaitre
            List<TechType> validCreatures = CreatureGenerator.GetAvailableCreatures();

            Plugin.Logger.LogInfo($"UpdateCreatureSpawns : Nombre de créatures valables trouvées : {validCreatures.Count}");

            if (validCreatures.Count == 0)
            {
                Plugin.Logger.LogWarning("Aucune créature valable trouvée pour le spawn.");
                return;
            }

            // Pour chaque Biome : Logguer le nom du biome
            foreach (var biome in biomes)
            {
                Plugin.Logger.LogInfo($"Biome : {biome.Name}");

                // Pour chaque spawnZone : Logguer le nom de la zone et génerer les coordonées aléatoires.
                foreach (var spawnZone in biome.SpawnZones)
                {
                    int creatureCount = (int)(10 * densityMultiplier);
                    List<Vector3> generatedCoordinates = CoordinateGenerator.GenerateRandomCoordinates(spawnZone, creatureCount);

                    Plugin.Logger.LogInfo($"Génération de {creatureCount} créatures dans {biome.Name} : {spawnZone.Name}");

                    // Pour chaque coordonées aléatoires : Spawn une créature aléatoire.
                    foreach (var coord in generatedCoordinates)
                    {

                        TechType randomCreature = validCreatures[UnityEngine.Random.Range(0, validCreatures.Count)];

                        CreatureSpawner.SpawnCreature(randomCreature, coord);
                        Plugin.Logger.LogInfo($"{randomCreature} spawn à : {coord}");
                    }

                }
            }
        }
    }
}
