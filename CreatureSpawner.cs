using Nautilus.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RandomCreatureSpawn
{
    public class CreatureSpawner
    {
        public static void SpawnCreature(TechType techType, UnityEngine.Vector3 coordinates)
        {
            if (coordinates == null)
            {
                Debug.LogError("Problème avec les coordonées");
                return;
            }
            // Enregistre les informations nécessaire au spawn d'une créature.
            SpawnInfo creatureInfo = new SpawnInfo(techType, coordinates);
            CoordinatedSpawnsHandler.RegisterCoordinatedSpawn(creatureInfo);
        }
    }
}
