using System;
using System.IO;
using medicaltraining.assetstore.ScenarioConfiguration.Serialization;
using medicaltraining.assetstore.ScenarioConfiguration.Serialization.Utils;
using Runtime.Common;
using Runtime.ScenarioConfiguration.Views.Agent;
using Runtime.ScenarioConfiguration.Views.Anamnesis;
using Runtime.ScenarioConfiguration.Views.Environment;
using Runtime.ScenarioConfiguration.Views.Pathologies;
using Runtime.Utils;
using UnityEngine;
using UnityEngine.Assertions;

namespace Runtime.ScenarioConfiguration
{
    /// <summary>
    /// Holds scenario data configured by the user and handles saving and loading it.
    /// </summary>
    public class ScenarioConfigurator : MonoBehaviour
    {
        private EnvironmentView _environmentView;
        private AgentView _agentConfiguration;
        private AnamnesisView _anamnesisConfiguration;
        private PathologiesView _pathologiesView;
        private PathologyVariantManager _pathologyVariantManager;

        private void Start()
        {
            _environmentView = FindObjectOfType<EnvironmentView>(includeInactive: true);
            _agentConfiguration = FindObjectOfType<AgentView>(includeInactive: true);
            _anamnesisConfiguration = FindObjectOfType<AnamnesisView>(includeInactive: true);
            _pathologiesView = FindObjectOfType<PathologiesView>(includeInactive: true);
            _pathologyVariantManager = FindObjectOfType<PathologyVariantManager>(includeInactive: true);
            if (ConfigurationContainer.Instance.ExistingScenarioLoaded)
            {
                var resourcesFolder = ConfigurationContainer.Instance.LoadedScenarioDirectory;
                RestoreFromScenarioConfig(ConfigurationContainer.Instance.LoadedScenario, resourcesFolder);
            }
            else
                LoadEmptyScenarioConfig();
        }

        /// <summary>
        /// Collects all configurated information from the multiple views into the serializable scenario
        /// object and saves it as a json file at <b>filepath</b>.
        /// </summary>
        public VariableScenarioConfig GetScenarioInputData(string scenarioFolder, IFileWriter fileWriter)
        {
            //Wipe old contents (resources) inside of scenario folder
            if (Directory.Exists(scenarioFolder)) SerializationFileUtils.ClearScenarioFolder(scenarioFolder);

            VariableScenarioConfig scenarioData = new VariableScenarioConfig();
            AgentConfig agentConfig = _agentConfiguration.GetConfiguration(String.Empty);
            scenarioData.Agents.Add(agentConfig);
            scenarioData.Environments.Add(_environmentView.GetConfiguration(String.Empty));
            int index = 0;
            foreach (var pathologyVariant in _pathologyVariantManager.GetPathologyVariants())
            {
                pathologyVariant.Agent = agentConfig;
                string variantName = FolderStructure.PathologyVariant(index);
                string path = Path.Combine(scenarioFolder, variantName);
                var serializedPathologyVariant = new SerializablePathologyVariant(pathologyVariant, path, fileWriter);
                serializedPathologyVariant.Pathology.PathologyID = variantName;
                scenarioData.Pathologies.Add(serializedPathologyVariant);
                index++;
            }

            scenarioData.UseUnifiedAnamnesisData = _pathologyVariantManager.UseUnifiedAnamnesisData;
            scenarioData.PopulateScenarioVariants();
            return scenarioData;
        }

        public void SaveScenarioInputData(string title, string description, IFileWriter fileWriter,
            bool saveAsNew = false)
        {
            bool createNewDirectory = !ConfigurationContainer.Instance.ExistingScenarioLoaded || saveAsNew;
            string scenarioDirectory = createNewDirectory
                ? ConfigurationContainer.Instance.CreateNewScenarioDirectory()
                : ConfigurationContainer.Instance.LoadedScenarioDirectory;
            VariableScenarioConfig scenarioConfig = GetScenarioInputData(scenarioDirectory, fileWriter);
            string jsonFilePath = Path.Combine(scenarioDirectory, ConfigurationContainer.Instance.ScenarioFilename);
            SerializationFileUtils.SaveScenarioWithMetadata(scenarioConfig, title, description,
                jsonFilePath, fileWriter);
            if (createNewDirectory)
            {
                ConfigurationContainer.Instance.ExistingScenarioLoaded = true;
                ConfigurationContainer.Instance.LoadedScenarioDirectory = scenarioDirectory;
                ConfigurationContainer.Instance.LoadedScenario = scenarioConfig;
            }
            
            
        }

        /// <summary>
        /// Initializes views to load an empty scenario.
        /// </summary>
        public void LoadEmptyScenarioConfig()
        {
            _pathologyVariantManager.LoadEmptyConfiguration();
        }

        /// <summary>
        /// Reads the scenario data stored in the Configuration Container and restores all
        /// user data to the respective views.
        /// </summary>
        public void RestoreFromScenarioConfig(VariableScenarioConfig scenarioConfig, string resourcesFolder)
        {
            Assert.IsTrue(ConfigurationContainer.Instance.ExistingScenarioLoaded);
            Assert.IsNotNull(ConfigurationContainer.Instance.LoadedScenario);
            _agentConfiguration.LoadConfiguration(scenarioConfig.GetDefaultAgent(), resourcesFolder);
            _environmentView.LoadConfiguration(scenarioConfig.GetDefaultEnvironment(), resourcesFolder);
            //PathologyVariantManager handles data for anamnesis and pathology
            _pathologyVariantManager.LoadConfiguration(scenarioConfig.Pathologies, resourcesFolder);
            _pathologyVariantManager.SetUseUnifiedAnamnesisData(scenarioConfig.UseUnifiedAnamnesisData);
        }
    }
}