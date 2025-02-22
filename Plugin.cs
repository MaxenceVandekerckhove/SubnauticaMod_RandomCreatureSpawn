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
        // Initialisation du logger
        Logger = base.Logger;

        // Application des patchs Harmony
        Harmony.CreateAndPatchAll(Assembly, $"{PluginInfo.PLUGIN_GUID}");
        Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");

        // Chargement de la configuration
        config = OptionsPanelHandler.RegisterModOptions<Config>();

        // Événement pour le chargement de la scène
        Logger.LogInfo("Evenement pour le chargement de la scène");
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // Permet de charger le mod après le chargement d'autres mods.
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Vérifie si la scène chargée est la scène principale (partie en cours)
        Logger.LogInfo($"Nom de la scène : {scene.name}");

        if (scene.name == "Main" && !hasInitialized)
        {
            Logger.LogInfo($"Scène Main détecté !");
            hasInitialized = true;
            Logger.LogInfo("Partie chargée. Initialisation du mod RandomCreatureSpawn...");

            // Initialisation des créatures disponibles
            CreatureGenerator.OnCreatureLoaded += () => StartCoroutine(ShowCreaturesAfterInit());
            CreatureGenerator.Initialize();
        }
    }

    // Méthode permettant d'afficher les créatures disponibles après le chargement des TechTypes
    private IEnumerator ShowCreaturesAfterInit()
    {
        yield return new WaitForSeconds(1);
        List<TechType> creatures = CreatureGenerator.GetAvailableCreatures();

        Plugin.Logger.LogInfo($"Liste des créatures disponibles : {creatures.Count} trouvées.");
        foreach (var creature in creatures)
        {
            Plugin.Logger.LogInfo($"- {creature}");
        }

        // Génère les coordonées et fait apparaitre les créatures
        UpdateCreatureSpawns.RefreshSpawns(config.CreatureDensityMultiplier);
    }
}