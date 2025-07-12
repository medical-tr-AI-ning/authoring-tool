using System;
using System.Collections.Generic;
using System.IO;
using medicaltraining.assetstore.ScenarioConfiguration.Serialization;
using UnityEngine;

namespace Runtime.Common
{
    /// <summary>
    /// Global data container which is shared between scenes (DontDestroyOnLoad Singleton).
    /// Used to transfer a loaded scenario from the scenario list to the scenario configurator.
    /// </summary>
    public class ConfigurationContainer : MonoBehaviour
    {
        public static ConfigurationContainer Instance;

        [NonSerialized] public VariableScenarioConfig LoadedScenario;
        public bool ExistingScenarioLoaded;
        public string LoadedScenarioDirectory;
        [SerializeField] private string _scenariosDirectory;
        [SerializeField] private string _defaultPresetsDirectory;
        [SerializeField] private string _customPresetsDirectory;
        public Queue<string> Notifications;

        private void Awake()
        {
            //Singleton Guard
            if (Instance != null)
            {
                Destroy(this);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(this);
            if (!Directory.Exists(GetScenariosDirectory())) Directory.CreateDirectory(GetScenariosDirectory());
            if (!Directory.Exists(GetCustomPresetsDirectory())) Directory.CreateDirectory(GetCustomPresetsDirectory());
            if (!Directory.Exists(GetDefaultPresetsDirectory())) Directory.CreateDirectory(GetDefaultPresetsDirectory());
            Notifications = new Queue<string>();
            if(string.IsNullOrEmpty(_scenariosDirectory)) Debug.LogWarning("Scenario directory is not set! Scenarios will be saved to the application data root!");
            Debug.Log($"Scenarios will be saved to {GetScenariosDirectory()}.");
        }
        public string GetDefaultPresetsDirectory() => $"{Application.streamingAssetsPath}/{_defaultPresetsDirectory}/";
        public string GetCustomPresetsDirectory() => $"{Application.persistentDataPath}/{_customPresetsDirectory}/";
        public string GetScenariosDirectory() => $"{Application.persistentDataPath}/{_scenariosDirectory}/";
        /// <summary>
        /// Creates a new unique filepath for a json file to save a scenario to disk.
        /// </summary>
        /// <returns></returns>
        public string ScenarioFilename => "scenario.json";

        public string PresetFilename => "preset.json";

        public string CreateNewScenarioDirectory()
        {
            string directory = Path.Combine(GetScenariosDirectory(), Guid.NewGuid().ToString());
            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);
            return directory;
        }
    }
}