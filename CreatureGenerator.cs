using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UWE;
using static HandReticle;

namespace RandomCreatureSpawn
{
    public static class CreatureGenerator
    {
        // Declaration of the list of available creatures
        private static readonly List<TechType> availableCreatures = new List<TechType>();

        // Declaration of the event signaling the end of loading of creatures
        public static event Action OnCreatureLoaded;

        // Initializing Creatures
        public static void Initialize()
        {

            // Clean the available creature list
            availableCreatures.Clear();
            Plugin.Logger.LogInfo($"Available creatures cleaning. Number of AvailableCreatures = {availableCreatures.Count}.");

            // Loads excluded creatures from config
            var excludedList = Config.LoadExcludedCreatures()
                .Select(name => name.Trim())
                .ToHashSet();
            Plugin.Logger.LogInfo($"Number of creatures to exclude : {excludedList.Count}");
            Plugin.Logger.LogInfo($"Excluded creatures configured in JSON file : {string.Join(", ", excludedList)}");

            Plugin.Logger.LogInfo($"Call the function to load the creatures.");
            UWE.CoroutineHost.StartCoroutine(LoadAllCreatures(excludedList));
        }

        // Function that load and filter the TechTypes
        private static IEnumerator LoadAllCreatures(HashSet<String> excludedList)
        {
            Plugin.Logger.LogInfo("Creature detection...");
            int pendingTasks = 0;

            try
            {
                foreach (TechType techType in Enum.GetValues(typeof(TechType)))
                {
                    string techTypeName = techType.ToString();
                    Plugin.Logger.LogInfo($"TechType processing : {techTypeName}");

                    // Check if the TechType is excluded
                    if (excludedList.Contains(techTypeName))
                    {
                        Plugin.Logger.LogInfo($"{techType} ignored.");
                        continue;
                    }

                    pendingTasks++;
                    UWE.CoroutineHost.StartCoroutine(IsCreatureWithTimeout(techType, 1f, IsCreature =>
                    {
                        try
                        {
                            if (IsCreature)
                            {
                                availableCreatures.Add(techType);
                                Plugin.Logger.LogInfo($"{techType} is an available creature");
                                Plugin.Logger.LogInfo($"{techType} added to the spawnable creature list");
                            }
                        }
                        catch(Exception e)
                        {
                            Plugin.Logger.LogInfo($"Error while verifying Techtype {techType} : {e}");

                        }
                        finally
                        {
                            pendingTasks--;
                            Plugin.Logger.LogInfo($"pendingTasks decremented. Remaining: : {pendingTasks}");
                        }
                    }));
                }
            }
            catch(Exception e)
            {
                Plugin.Logger.LogInfo($"Error in LoadAllCreature method : {e}");
            }

            // If tasks are in progress, we wait for them to finish.
            while (pendingTasks > 0)
            {
                Plugin.Logger.LogInfo($"pendingTasks equals {pendingTasks}, LoadAllCreature not completed because pendingTash > 0");
                yield return null;
            }

            // Signals the end of detection, even in case of error or blocking.
            try
            {
                Plugin.Logger.LogInfo($"Detection Completed: {availableCreatures.Count} valid creatures found!");
                OnCreatureLoaded?.Invoke();
            }
            catch(Exception e)
            {
                Plugin.Logger.LogError($"Error while completing creature detection: {e}");
            }

        }

        // Checks if the IsCreature method is not blocked
        private static IEnumerator IsCreatureWithTimeout(TechType techType, float timeout, Action<bool> callBack)
        {
            bool completed = false;

            // Start the detection task
            UWE.CoroutineHost.StartCoroutine(IsCreature(techType, result =>
            {
                if (!completed)
                {
                    completed = true;
                    callBack?.Invoke(result);
                }
            }));

            // Waiting with timeout
            float startTime = Time.time;
            while (!completed && Time.time - startTime < timeout)
            {
                yield return null;
            }

            // If the deadline is exceeded
            if (!completed)
            {
                Plugin.Logger.LogWarning($"{techType} did not respond after {timeout} seconds. Ignored.");
                callBack?.Invoke(false);
            }
        }


        // Checks if a TechType is a creature
        private static IEnumerator IsCreature(TechType techType, Action<bool> callBack)
        {
            // Attempts to load the prefab of the given TechType
            var task = CraftData.GetPrefabForTechTypeAsync(techType, verbose: false);
            yield return task;

            // Checks if the prefab exists and contains a creature component
            GameObject prefab = task.GetResult();
            bool isCreature = prefab != null && prefab.GetComponent<Creature>() != null;

            // Returns the result
            callBack?.Invoke(isCreature);
        }

        // Returns the list of detected creatures
        public static List<TechType> GetAvailableCreatures() => new List<TechType>(availableCreatures);
    }
}
