using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public class BiomeLoader
{
    public static List<Biome> LoadJson()
    {
        using (StreamReader r = new StreamReader(".\\BepInEx\\plugins\\RandomCreatureSpawn\\BiomeCoordinates.json"))
        {
            string json = r.ReadToEnd();

            if(json == null)
            {
                Debug.LogError($"Json reading error : BiomeCoordinates.json is unreadable or empty");
                return null;
            }

            try
            {
                BiomeData data = JsonConvert.DeserializeObject<BiomeData>(json);
                return data?.Biomes;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Erreur lors du chargement des biomes : {ex.Message}");
                return null;
            }
        }
        
    }
}

// Define the BiomeData Class
public class BiomeData
{
    [JsonProperty("biomes")]
    public List<Biome> Biomes { get; set; }
}

// Define the Biome Class
public class Biome
{
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("spawn_zones")]
    public List<SpawnZone> SpawnZones { get; set; }
}

// Define the Spawnzone Class
public class SpawnZone
{
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("vertices")]
    public List<Coord> Vertices { get; set; }
}

// Define the Coord Class
public class Coord
{
    [JsonProperty("x")]
    public float X { get; set; }

    [JsonProperty("y")]
    public float Y { get; set; }

    [JsonProperty("z")]
    public float Z { get; set; }

    public override string ToString()
    {
        return $"({X}, {Y}, {Z})";
    }
}
