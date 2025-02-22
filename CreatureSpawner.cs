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
                Debug.LogError("Problem with coordinates");
                return;
            }
            // Records information needed to spawn a creature
            SpawnInfo creatureInfo = new SpawnInfo(techType, coordinates);
            CoordinatedSpawnsHandler.RegisterCoordinatedSpawn(creatureInfo);
        }
    }
}
