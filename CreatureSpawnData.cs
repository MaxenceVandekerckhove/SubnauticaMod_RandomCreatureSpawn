using Nautilus.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomCreatureSpawn
{
    public class CreatureSpawnData : SaveDataCache
    {
        public bool HasSpawnedCreatures = false;

        private static CreatureSpawnData instance;

        public static CreatureSpawnData Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new CreatureSpawnData();
                    instance.Load();
                }
                return instance;
            }
        }

        public void SaveData()
        {
            Save();
        }
    }
}