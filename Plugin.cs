using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Nautilus.Handlers;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace RandomCreatureSpawn;

[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
[BepInDependency("com.snmodding.nautilus")]
public class Plugin : BaseUnityPlugin
{
    public new static ManualLogSource Logger { get; private set; }
    
    private static Assembly Assembly { get; } = Assembly.GetExecutingAssembly();

    internal static Config config;

    private static bool hasInitialized = false;

    private void Awake()
    {
        // Logger initialization
        Logger = base.Logger;

        // Configuration loading
        config = OptionsPanelHandler.RegisterModOptions<Config>();

        // Event that detects the scene loading
        Logger.LogInfo("Scene loading event");
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // Allow to load the mod after the loading of other mod objects (Creatures from other mods).
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Check if the loaded scene is the MAIN scene (Player going in game)
        Logger.LogInfo($"Scene name : {scene.name}");

        if (scene.name == "Main" && !hasInitialized)
        {
            Logger.LogInfo($"Main scene detected !");
            hasInitialized = true;
            Logger.LogInfo("Game loaded. RandomCreatureSpawn initialization...");

            // Available creatures initialization
            CreatureGenerator.OnCreatureLoaded += () => StartCoroutine(ShowCreaturesAfterInit());
            CreatureGenerator.Initialize();
        }
    }

    // Display availables creatures that can be spawned
    private IEnumerator ShowCreaturesAfterInit()
    {
        yield return new WaitForSeconds(1);
        List<TechType> creatures = CreatureGenerator.GetAvailableCreatures();

        Plugin.Logger.LogInfo($"List of available creatures : {creatures.Count} found.");
        foreach (var creature in creatures)
        {
            Plugin.Logger.LogInfo($"- {creature}");
        }

        // Generate the random coordinates and spawn the creatures
        UpdateCreatureSpawns.RefreshSpawns(config.CreatureDensityMultiplier);
    }
}