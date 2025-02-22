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
        // Déclaration de la liste des créatures disponibles
        private static readonly List<TechType> availableCreatures = new List<TechType>();

        // Déclaration de l'évenement signalant la fin du chargement des créatures
        public static event Action OnCreatureLoaded;

        // Initalisation des créatures
        public static void Initialize()
        {

            // Nettoie la liste des créatures disponibles
            availableCreatures.Clear();
            Plugin.Logger.LogInfo($"Nettoyage des créatures disponbles. Nombre de AvailableCreatures = {availableCreatures.Count}.");

            // Charge les créatures exclues depuis la config
            var excludedList = Config.LoadExcludedCreatures()
                .Select(name => name.Trim())
                .ToHashSet();
            Plugin.Logger.LogInfo($"Nombre de créature à exclure : {excludedList.Count}");
            Plugin.Logger.LogInfo($"Créatures exclues configurées dans le JSON : {string.Join(", ", excludedList)}");

            Plugin.Logger.LogInfo($"Appel de la fonction permettant de charger les créatures.");
            UWE.CoroutineHost.StartCoroutine(LoadAllCreatures(excludedList));
        }

        // Charge et filtre les TechType
        private static IEnumerator LoadAllCreatures(HashSet<String> excludedList)
        {
            Plugin.Logger.LogInfo("Détection des créatures en cours...");
            int pendingTasks = 0;

            try
            {
                foreach (TechType techType in Enum.GetValues(typeof(TechType)))
                {
                    string techTypeName = techType.ToString();
                    Plugin.Logger.LogInfo($"Traitement du TechType : {techTypeName}");

                    // Vérifie si le TechType est exclu
                    if (excludedList.Contains(techTypeName))
                    {
                        Plugin.Logger.LogInfo($"{techType} ignoré.");
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
                                Plugin.Logger.LogInfo($"{techType} est une créature valable");
                                Plugin.Logger.LogInfo($"{techType} ajouté à la liste de créature");
                            }
                        }
                        catch(Exception e)
                        {
                            Plugin.Logger.LogInfo($"Erreur lors de la vérification du Techtype {techType} : {e}");

                        }
                        finally
                        {
                            pendingTasks--;
                            Plugin.Logger.LogInfo($"pendingTasks décrémenté. Restant : {pendingTasks}");
                        }
                    }));
                }
            }
            catch(Exception e)
            {
                Plugin.Logger.LogInfo($"Erreur dans la méthode LoadAllCreature : {e}");
            }

            // Si des tâches sont en cours, on attend la fin.
            while (pendingTasks > 0)
            {
                Plugin.Logger.LogInfo($"pendingTasks est égal à {pendingTasks}, LoadAllCreature non terminé car pendingTash > 0");
                yield return null;
            }

            // Signale la fin de la détection, même en cas d'erreur ou de blocage.
            try
            {
                Plugin.Logger.LogInfo($"Détection terminée : {availableCreatures.Count} créatures valides trouvées !");
                OnCreatureLoaded?.Invoke();
            }
            catch(Exception e)
            {
                Plugin.Logger.LogError($"Erreur lors de la fin de la détection des créatures : {e}");
            }

        }

        // Vérifie si la méthode IsCreature n'est pas bloqué
        private static IEnumerator IsCreatureWithTimeout(TechType techType, float timeout, Action<bool> callBack)
        {
            bool completed = false;

            // Lance la tâche de détection
            UWE.CoroutineHost.StartCoroutine(IsCreature(techType, result =>
            {
                if (!completed)
                {
                    completed = true;
                    callBack?.Invoke(result);
                }
            }));

            // Attente avec timeout
            float startTime = Time.time;
            while (!completed && Time.time - startTime < timeout)
            {
                yield return null;
            }

            // Si le délai est dépassé
            if (!completed)
            {
                Plugin.Logger.LogWarning($"{techType} n'a pas répondu après {timeout} secondes. Ignoré.");
                callBack?.Invoke(false);
            }
        }


        // Vérifie si un TechType est une créature
        private static IEnumerator IsCreature(TechType techType, Action<bool> callBack)
        {
            // Tente de charger le prefab du TechType donné
            var task = CraftData.GetPrefabForTechTypeAsync(techType, verbose: false);
            yield return task;

            // Vérifie si le prefab existe et contient un composant créature
            GameObject prefab = task.GetResult();
            bool isCreature = prefab != null && prefab.GetComponent<Creature>() != null;

            //Renvoie le résultat
            callBack?.Invoke(isCreature);
        }

        // Renvoie la liste des créatures détectées
        public static List<TechType> GetAvailableCreatures() => new List<TechType>(availableCreatures);
    }
}
