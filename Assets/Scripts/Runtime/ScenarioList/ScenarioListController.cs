using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using medicaltraining.assetstore.ScenarioConfiguration.Serialization;
using Runtime.Common;
using Runtime.Common.Prompts;
using Runtime.Utils;
using SFB;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Runtime.ScenarioList
{
    /// <summary>
    /// Handles UI and logic for user actions within the ScenarioList scene;
    /// </summary>
    public class ScenarioListController : MonoBehaviour
    {
        private List<ScenarioEntry> _scenarioEntries;
        [SerializeField] private ScenarioEntry _scenarioEntryTemplate;
        [SerializeField] private Transform _scenarioEntryContainer;
        [SerializeField] private string _sceneDirectory;
        [SerializeField] private TMP_InputField _searchField;
        [SerializeField] private Notification _notificationTemplate;
        [SerializeField] private Transform _notificationContainer;
        [SerializeField] private Transform _uiRoot;
        [SerializeField] private GameObject _unfocusBackground;
        private string _searchFilter;
        [SerializeField] private ScenarioInformationPrompt _scenarioInformationPromptTemplate;
        [SerializeField] private GameObject _skeleton;
        [SerializeField] private NotificationController _notificationController;
        AsyncOperation async;

        private void Start()
        {
            _scenarioEntries = new List<ScenarioEntry>();
            _searchField.onValueChanged.AddListener(updateSearchFilter);
            updateScenarioList();
            displayPendingNotifications();
            async = SceneManager.LoadSceneAsync("ScenarioConfiguration");
            async.allowSceneActivation = false;
        }

        private void updateScenarioList()
        {
            //Remove existing entries
            clearScenarioEntries();
            string[] scenarioDirectories =
                Directory.GetDirectories(ConfigurationContainer.Instance.GetScenariosDirectory());
            List<Tuple<string, VariableScenarioConfig>> scenarioConfigs =
                new List<Tuple<string, VariableScenarioConfig>>();
            foreach (string scenarioDirectory in scenarioDirectories)
            {
                VariableScenarioConfig scenario;
                try
                {
                    string scenarioJsonPath =
                        Path.Combine(scenarioDirectory, ConfigurationContainer.Instance.ScenarioFilename);
                    scenario = SerializationFileUtils.ReadFromJson(scenarioJsonPath);
                }
                catch (Exception exception)
                {
                    Debug.LogWarning(
                        $"Scenario at {scenarioDirectory} could not be parsed! It possibly uses a deprecated configuration format. Exception: {exception.Message}. ");
                    continue;
                }

                //Apply Search Filter
                if (isFilterActive() && !scenarioPassesFilter(scenario)) continue;
                scenarioConfigs.Add(new Tuple<string, VariableScenarioConfig>(scenarioDirectory, scenario));
            }

            scenarioConfigs.Sort((first, second) =>
                second.Item2.ModificationDate.CompareTo(first.Item2.ModificationDate));

            foreach (Tuple<string, VariableScenarioConfig> config in scenarioConfigs)
            {
                createScenarioEntry(config.Item2, config.Item1);
            }
        }

        private void updateSearchFilter(string searchFilter)
        {
            _searchFilter = searchFilter;
            updateScenarioList();
        }

        private bool isFilterActive() => !string.IsNullOrWhiteSpace(_searchFilter);

        private bool scenarioPassesFilter(VariableScenarioConfig scenario) =>
            scenario.Description.ToLowerInvariant().Contains(_searchFilter.ToLowerInvariant()) ||
            scenario.ScenarioName.ToLowerInvariant().Contains(_searchFilter.ToLowerInvariant());

        private void clearScenarioEntries()
        {
            ScenarioEntry[] entries = _scenarioEntryContainer.GetComponentsInChildren<ScenarioEntry>();
            foreach (ScenarioEntry entry in entries)
                Destroy(entry.gameObject);
        }

        private void createScenarioEntry(VariableScenarioConfig scene, string directory)
        {
            ScenarioEntry scenarioEntry = Instantiate(_scenarioEntryTemplate, _scenarioEntryContainer);
            scenarioEntry.AssignScenario(scene);
            scenarioEntry.Directory = directory;
            _scenarioEntries.Add(scenarioEntry);
        }

        private void sortScenarioEntries()
        {
        }

        public void LoadEmptyScenario()
        {
            ConfigurationContainer.Instance.LoadedScenario = null;
            ConfigurationContainer.Instance.ExistingScenarioLoaded = false;
            LoadScenarioEditor();
        }

        public void LoadSavedScenario(ScenarioEntry scenario)
        {
            ConfigurationContainer.Instance.LoadedScenario = scenario.Scenario;
            ConfigurationContainer.Instance.ExistingScenarioLoaded = true;
            ConfigurationContainer.Instance.LoadedScenarioDirectory = scenario.Directory;
            LoadScenarioEditor();
        }

        public void LoadScenarioEditor()
        {
            _skeleton.SetActive(true);
            async.allowSceneActivation = true;
        }

        public void OpenExportPrompt(ScenarioEntry scenarioEntry)
        {
            try
            {
                string exportPath =
                    StandaloneFileBrowser.SaveFilePanel("Szenenkonfiguration exportieren", "",
                        scenarioEntry.Scenario.ScenarioName, "mtscn");
                SerializationFileUtils.ExportDirectoryAsZip(scenarioEntry.Directory, exportPath);
                _notificationController.CreateNotification("Szenario wurde erfolgreich exportiert.");
            }
            catch (Exception e)
            {
                _notificationController.CreateNotification("Fehler beim Export des Szenarios.");
                Debug.LogError(e);
            }
        }

        public void OpenDeletionPrompt(ScenarioEntry scenarioEntry)
        {
            string warningText =
                $"Das ausgewählte Szenario \"{scenarioEntry.Scenario.ScenarioName}\" wird unwiderruflich gelöscht.";
            PromptHandler.Instance.DisplayPrompt(PromptHandler.PromptType.DeleteScenario, message: warningText,
                onConfirm: () =>
                    DeleteScenarioEntry(scenarioEntry));
        }

        public async void OpenEditInformationPrompt(ScenarioEntry scenarioEntry)
        {
            ScenarioInformationPrompt prompt = PromptHandler.Instance
                .DisplayPrompt(_scenarioInformationPromptTemplate, new Vector2(-50, 190))
                .GetComponent<ScenarioInformationPrompt>();
            VariableScenarioConfig data = scenarioEntry.Scenario;
            prompt.Configure(false, true, data.ScenarioName, data.Description);
            ScenarioInformationPrompt.ScenarioInformationPromptResponse response;
            try
            {
                response = await prompt.ShowPrompt();
            }
            catch (TaskCanceledException)
            {
                return;
            }

            //Just overwrite the json file. Keep the resources untouched.
            string jsonPath = Path.Combine(scenarioEntry.Directory, ConfigurationContainer.Instance.ScenarioFilename);
            SerializationFileUtils.SaveScenarioWithMetadata(data, response.Title, response.Description,
                jsonPath, new SimpleFileWriter());
            updateScenarioList();
        }

        public void DeleteScenarioEntry(ScenarioEntry scenarioEntry)
        {
            Directory.Delete(scenarioEntry.Directory, true);
            updateScenarioList();
            _notificationController.CreateNotification(
                $"Szenario \"{scenarioEntry.Scenario.ScenarioName}\" wurde gelöscht.");
        }

        public void DuplicateScenarioEntry(ScenarioEntry scenarioEntry)
        {
            SerializationFileUtils.CopyDirectory(scenarioEntry.Directory,
                ConfigurationContainer.Instance.CreateNewScenarioDirectory(), true);
            updateScenarioList();
            _notificationController.CreateNotification(
                $"Szenario \"{scenarioEntry.Scenario.ScenarioName}\" wurde dupliziert.");
        }

        private void displayPendingNotifications()
        {
            Queue<string> notifications = ConfigurationContainer.Instance.Notifications;
            while (notifications.Count != 0)
            {
                _notificationController.CreateNotification(notifications.Dequeue());
            }
        }
    }
}