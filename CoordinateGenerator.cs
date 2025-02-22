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
            // Génération de la liste des coordonées à renvoyer en sortie
            List<Vector3> generatedCoordinates = new List<Vector3>();

            // Récupération des 4 sommets de la zone de spawn
            var vertices = spawnZone.Vertices;

            // Calculer les limites de la zone (min et max pour chaque axe)
            float minX = Mathf.Min(vertices[0].X, vertices[1].X, vertices[2].X, vertices[3].X);
            float maxX = Mathf.Max(vertices[0].X, vertices[1].X, vertices[2].X, vertices[3].X);

            float minZ = Mathf.Min(vertices[0].Z, vertices[1].Z, vertices[2].Z, vertices[3].Z);
            float maxZ = Mathf.Max(vertices[0].Z, vertices[1].Z, vertices[2].Z, vertices[3].Z);


            // Utiliser la valeur Y fixée dans le JSON, en l'ajustant aléatoirement de ±30%
            float baseY = vertices[0].Y;
            float minY = baseY * 0.7f;
            float maxY = baseY * 1.3f;

            // Génération des coordonées aléatoires dans les limites de la zone
            for (int i = 0; i < CreatureDensity; i++)
            {
                float randomX = UnityEngine.Random.Range(minX, maxX);
                float randomY = UnityEngine.Random.Range(minY, maxY);
                float randomZ = UnityEngine.Random.Range(minZ, maxZ);

                // Ajout de la coordonée généré à la liste "generatedCoordinates"
                generatedCoordinates.Add(new Vector3(randomX, randomY, randomZ));
            }

            return generatedCoordinates;
        }

    }
}
