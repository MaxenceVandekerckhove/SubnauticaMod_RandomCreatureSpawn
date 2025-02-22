using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RandomCreatureSpawn
{
    public class CoordinateGenerator
    {
        public static List<Vector3> GenerateRandomCoordinates(SpawnZone spawnZone,float CreatureDensity)
        {
            // Generating the list of coordinates to send as output
            List<Vector3> generatedCoordinates = new List<Vector3>();

            // Recovering the 4 vertices of the spawn zone
            var vertices = spawnZone.Vertices;

            // Calculate the area limits (min and max for each axis)
            float minX = Mathf.Min(vertices[0].X, vertices[1].X, vertices[2].X, vertices[3].X);
            float maxX = Mathf.Max(vertices[0].X, vertices[1].X, vertices[2].X, vertices[3].X);

            float minZ = Mathf.Min(vertices[0].Z, vertices[1].Z, vertices[2].Z, vertices[3].Z);
            float maxZ = Mathf.Max(vertices[0].Z, vertices[1].Z, vertices[2].Z, vertices[3].Z);


            // Use the Y value set in the JSON, adjusting it randomly by ±30%
            float baseY = vertices[0].Y;
            float minY = baseY * 0.7f;
            float maxY = baseY * 1.3f;

            // Generating random coordinates within the boundaries of the area
            for (int i = 0; i < CreatureDensity; i++)
            {
                float randomX = UnityEngine.Random.Range(minX, maxX);
                float randomY = UnityEngine.Random.Range(minY, maxY);
                float randomZ = UnityEngine.Random.Range(minZ, maxZ);

                // Added the generated coordinate to the "generate Coordinates" list
                generatedCoordinates.Add(new Vector3(randomX, randomY, randomZ));
            }

            return generatedCoordinates;
        }

    }
}
