using System;
using System.IO;
using Newtonsoft.Json;
using Runtime.Common;
using Runtime.Common.Prompts;
using Runtime.UI;
using Runtime.Utils;
using UnityEngine;

namespace Runtime.ScenarioConfiguration.Views
{
    /// <summary>
    /// A configuration view that has capabilities to save and load presets.
    /// </summary>
    /// <typeparam name="T">Type of preset-data that is saved/loaded</typeparam>
    public abstract class PresetConfigurationView<T> : ConfigurationView<T>
    {
        public abstract string PresetSubfolderName { get; }
        public abstract string PresetFileExtension { get; }

        [SerializeField] private Vector2 LoadPresetPromptOffset;
        [SerializeField] private Vector2 SavePresetPromptOffset;
        [SerializeField] private LoadPresetPrompt _loadPresetPromptTemplate;
        [SerializeField] private SavePresetPrompt _savePresetPromptTemplate;

        public void SavePreset(string name)
        {
            //TODO: What if Preset already exists?
            string resourcesFolder = Path.Combine(ConfigurationContainer.Instance.GetCustomPresetsDirectory(),
                PresetSubfolderName, name);
            if (!Directory.Exists(resourcesFolder)) Directory.CreateDirectory(resourcesFolder);
            T configuration = GetConfiguration(resourcesFolder);
            if (!ConfigurationIsValid(configuration))
            {
                throw new Exception("The preset data was invalid!");
            }

            string json = JsonConvert.SerializeObject(configuration);

            string jsonOutputPath = Path.Combine(resourcesFolder, ConfigurationContainer.Instance.PresetFilename);
            File.WriteAllText(jsonOutputPath, json);
            Debug.Log($"Preset has been saved to {jsonOutputPath}");
        }

        public async void OnLoadPresetPressed()
        {
            //TODO: This is unsafe and should validate the preset before loading
            LoadPresetPrompt prompt = PromptHandler.Instance
                .DisplayPrompt(_loadPresetPromptTemplate, offset: LoadPresetPromptOffset)
                .GetComponent<LoadPresetPrompt>();
            prompt.Configure(PresetSubfolderName, PresetFileExtension);
            LoadPresetPrompt.LoadPresetPromptResponse result =
                await prompt.ShowPrompt();
            Debug.Log($"Loading preset with path {result.PresetDirectory}");
            string jsonFilePath = Path.Combine(result.PresetDirectory, ConfigurationContainer.Instance.PresetFilename);
            T preset = SerializationFileUtils.UnsafeReadPresetFromJson<T>(jsonFilePath);
            LoadConfiguration(preset, result.PresetDirectory);
        }

        public async void OnSavePresetPressed()
        {
            SavePresetPrompt prompt = PromptHandler.Instance
                .DisplayPrompt(_savePresetPromptTemplate, offset: SavePresetPromptOffset)
                .GetComponent<SavePresetPrompt>();
            SavePresetPrompt.SavePresetPromptResponse result = await prompt.ShowPrompt();
            SavePreset(result.Name);
        }

        public abstract bool ConfigurationIsValid(T configuration);
    }
}