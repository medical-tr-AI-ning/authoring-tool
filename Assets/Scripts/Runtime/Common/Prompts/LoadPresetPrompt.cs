using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.Common.Prompts
{
    /// <summary>
    /// Prompt for loading a preset
    /// </summary>
    public class LoadPresetPrompt : ResponsePrompt<LoadPresetPrompt.LoadPresetPromptResponse>
    {
        [SerializeField] private LoadPresetPromptEntry _loadPresetEntryTemplate;
        [SerializeField] private string _presetsSubfolder;
        [SerializeField] private string _presetFileExtension;
        [SerializeField] private Transform _defaultPrefabsContainer;
        [SerializeField] private Transform _customPrefabsContainer;
        [SerializeField] private Button _confirmButton;
        private LoadPresetPromptEntry _selectedPreset;


        public void Initialize()
        {
            if (!Directory.Exists(defaultPresetsPath())) Directory.CreateDirectory(defaultPresetsPath());
            if (!Directory.Exists(customPresetsPath())) Directory.CreateDirectory(customPresetsPath());
            addDefaultPresetEntries();
            updateCustomPresetEntries();
        }

        public void Configure(string presetsSubfolder, string presetFileExtension)
        {
            _presetsSubfolder = presetsSubfolder;
            _presetFileExtension = presetFileExtension;
            Initialize();
        }

        private string defaultPresetsPath() =>
            ConfigurationContainer.Instance.GetDefaultPresetsDirectory() + _presetsSubfolder;

        private string customPresetsPath() =>
            ConfigurationContainer.Instance.GetCustomPresetsDirectory() + _presetsSubfolder;

        public void updateUIElements()
        {
            _confirmButton.interactable = _selectedPreset != null;
        }

        private void updateCustomPresetEntries()
        {
            clearPresetEntries(_customPrefabsContainer);
            addCustomPresetEntries();
        }

        private void addCustomPresetEntries()
        {
            addEntriesForFolders(customPresetsPath(), false, _customPrefabsContainer);
        }

        private void addDefaultPresetEntries()
        {
            addEntriesForFolders(defaultPresetsPath(), true, _defaultPrefabsContainer);
        }

        private void addEntriesForFolders(string presetsPath, bool defaultPresets, Transform targetTransform)
        {
            string[] presetDirectories = Directory.GetDirectories(presetsPath);
            Debug.Log($"Found {presetDirectories.Length} entries in {presetsPath}");
            foreach (string presetDirectory in presetDirectories)
            {
                string presetName = Path.GetFileName(presetDirectory);
                createPresetEntry(presetName, presetDirectory, defaultPresets, targetTransform);
            }
        }

        private LoadPresetPromptEntry createPresetEntry(string presetName, string path, bool isDefaultPreset,
            Transform container)
        {
            LoadPresetPromptEntry presetEntry = Instantiate(_loadPresetEntryTemplate, container);
            presetEntry.presetDirectory = path;
            presetEntry.presetName = presetName;
            presetEntry.loadPresetPrompt = this;
            presetEntry.isDefaultPreset = isDefaultPreset;
            return presetEntry;
        }

        private void clearPresetEntries(Transform presetContainer)
        {
            LoadPresetPromptEntry[] entries = presetContainer.GetComponentsInChildren<LoadPresetPromptEntry>();
            foreach (LoadPresetPromptEntry entry in entries)
                Destroy(entry.gameObject);
        }

        public void RenamePreset(LoadPresetPromptEntry preset, string newName)
        {
            if (preset == _selectedPreset) _selectedPreset = null;
            if (!isValidFileName(newName)) return;
            string baseDirectory = Path.GetDirectoryName(preset.presetDirectory);
            // Combine the directory with the new file name to get the new path
            string newFullPath = Path.Combine(baseDirectory, newName);
            Directory.Move(preset.presetDirectory, newFullPath);
            updateCustomPresetEntries();
            updateUIElements();
        }

        public void RequestDeletePreset(LoadPresetPromptEntry preset)
        {
            string warningMessage = $"Das ausgewählte Preset {preset.presetName} wird unwiderruflich gelöscht";
            PromptHandler.Instance.DisplayPrompt(PromptHandler.PromptType.DeletePreset, message: warningMessage,
                onConfirm: () => DeletePreset(preset));
        }

        public void DeletePreset(LoadPresetPromptEntry preset)
        {
            Directory.Delete(preset.presetDirectory, true);
            if (preset == _selectedPreset) _selectedPreset = null;
            updateCustomPresetEntries();
            updateUIElements();
        }

        public void SelectPreset(LoadPresetPromptEntry preset)
        {
            _selectedPreset?.OnDeselect();
            _selectedPreset = preset;
            Debug.Log($"Prefab with name {preset.presetName} selected.");
            _selectedPreset.OnSelect();
            updateUIElements();
        }

        public void OnConfirmPressed()
        {
            PromptHandler.Instance.DisplayPrompt(PromptHandler.PromptType.LoadPreset,
                onConfirm: () => SendResponseAndClose(GetResponse()));
        }

        public void OnCancelPressed()
        {
            CancelResponseAndClose();
        }

        private bool isValidFileName(string fileName)
        {
            char[] invalidChars = Path.GetInvalidFileNameChars();

            return !string.IsNullOrWhiteSpace(fileName) && fileName.All(c => !invalidChars.Contains(c));
        }

        public LoadPresetPromptResponse GetResponse()
        {
            return new LoadPresetPromptResponse
            {
                PresetDirectory = _selectedPreset?.presetDirectory
            };
        }

        protected override bool ResponseIsValid(LoadPresetPromptResponse response)
        {
            return !string.IsNullOrWhiteSpace(response.PresetDirectory);
        }

        public class LoadPresetPromptResponse
        {
            public string PresetDirectory { get; set; }
        }
    }
}