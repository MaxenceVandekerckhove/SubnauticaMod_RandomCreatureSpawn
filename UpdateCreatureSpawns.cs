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

            // Checking for biome presence in BiomeCoordinates.json file
            if (biomes == null || biomes.Count == 0)
            {
                Plugin.Logger.LogWarning("No biomes found in file /plugins/RandomCreatureSpawns/BiomeCoordinates.json.");
                return;
            }

            // Retrieving valid creatures to spawn
            List<TechType> validCreatures = CreatureGenerator.GetAvailableCreatures();

            Plugin.Logger.LogInfo($"UpdateCreatureSpawns : Number of valid creatures found: {validCreatures.Count}");

            if (validCreatures.Count == 0)
            {
                Plugin.Logger.LogWarning("No valid creatures found for spawn.");
                return;
            }

            // For each Biome: Log the name of the biome
            foreach (var biome in biomes)
            {
                Plugin.Logger.LogInfo($"Biome : {biome.Name}");

                // For each spawn Zone: Log the name of the zone and generate the random coordinates.
                foreach (var spawnZone in biome.SpawnZones)
                {
                    int creatureCount = (int)(10 * densityMultiplier);
                    List<Vector3> generatedCoordinates = CoordinateGenerator.GenerateRandomCoordinates(spawnZone, creatureCount);

                    Plugin.Logger.LogInfo($"Generating {creatureCount} créatures in {biome.Name} : {spawnZone.Name}");

                    // For each random coordinate: Spawn a random creature.
                    foreach (var coord in generatedCoordinates)
                    {

                        TechType randomCreature = validCreatures[UnityEngine.Random.Range(0, validCreatures.Count)];

                        CreatureSpawner.SpawnCreature(randomCreature, coord);
                        Plugin.Logger.LogInfo($"{randomCreature} spawned at : {coord}");
                    }

                }
            }
        }
    }
}
