using System;
using System.Threading.Tasks;
using medicaltraining.assetstore.ScenarioConfiguration.Serialization;
using Runtime.Common;
using Runtime.Common.Prompts;
using Runtime.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Runtime.ScenarioConfiguration
{
    /// <summary>
    /// Handles UI logic not directly related to any specific view.
    /// </summary>
    public class ScenarioConfiguratorUI : MonoBehaviour
    {
        private ScenarioConfigurator _scenarioConfigurator;
        [SerializeField] private ScenarioInformationPrompt _scenarioInformationPromptTemplate;
        [SerializeField] private NotificationController _notificationController;

        public void Start()
        {
            _scenarioConfigurator = FindObjectOfType<ScenarioConfigurator>();
        }

        public async void SaveScenario()
        {
            ScenarioInformationPrompt prompt = PromptHandler.Instance
                .DisplayPrompt(_scenarioInformationPromptTemplate, new Vector2(0, 200))
                .GetComponent<ScenarioInformationPrompt>();
            ScenarioInformationPrompt.ScenarioInformationPromptResponse response;
            if (ConfigurationContainer.Instance.ExistingScenarioLoaded)
            {
                VariableScenarioConfig existingData = ConfigurationContainer.Instance.LoadedScenario;
                prompt.Configure(true, false, existingData.ScenarioName, existingData.Description);
            }
            else
            {
                prompt.Configure(false, false);
            }

            try
            {
                response = await prompt.ShowPrompt();
            }
            //handle clicking Cancel
            catch (TaskCanceledException)
            {
                return;
            }

            if (response == null) return;

            ProgressFileWriter progressFileWriter = ProgressFileWriter.GetSingleton();
            try
            {
                _scenarioConfigurator.SaveScenarioInputData(response.Title, response.Description, progressFileWriter,
                    response.SaveAsNew);
            }
            catch (Exception e)
            {
                prompt.ClosePrompt();  
                _notificationController.CreateNotification($"Speichern ist fehlgeschlagen: {e.Message}");
            }

            var fileWriteProgressChangedAction = new ProgressFileWriter.FileWriteProgressChangedEvent(() =>
            {
                Debug.Log($"{progressFileWriter.GetProgress() * 100}% completed");
                prompt.SetProgress(progressFileWriter.GetProgress());
            });

            progressFileWriter.FileWriteProgressChanged += fileWriteProgressChangedAction;

            ProgressFileWriter.FileWriteCompletedEvent fileWriteCompletedAction = null;
            fileWriteCompletedAction = () =>
            {
                _notificationController.CreateNotification($"Szenario \"{response.Title}\" wurde gespeichert.");
                prompt.ClosePrompt();
                progressFileWriter.FileWriteProgressChanged -= fileWriteProgressChangedAction;
                progressFileWriter.FileWriteCompleted -= fileWriteCompletedAction;
            };
            progressFileWriter.FileWriteCompleted += fileWriteCompletedAction;
            StartCoroutine(progressFileWriter.WriteQueuedFiles());

            //TODO: Refactor
        }

        public void RequestReturnToScenarioList()
        {
            PromptHandler.Instance.DisplayPrompt(PromptHandler.PromptType.ReturnToScenarioList,
                onConfirm: () => SceneManager.LoadScene("ScenarioList"));
        }
    }
}