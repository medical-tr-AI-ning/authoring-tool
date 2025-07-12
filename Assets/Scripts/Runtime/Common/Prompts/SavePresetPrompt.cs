using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.Common.Prompts
{
    /// <summary>
    /// Prompt for saving a preset
    /// </summary>
    public class SavePresetPrompt : ResponsePrompt<SavePresetPrompt.SavePresetPromptResponse>
    {
        [SerializeField] private Button _saveButton;
        [SerializeField] private TMP_InputField _nameInputField;

        private void Awake()
        {
            _nameInputField.onValueChanged.AddListener(_ => UpdateUI());
            _saveButton.onClick.AddListener(OnSavePressed);
            UpdateUI();
        }

        public void UpdateUI()
        {
            _saveButton.interactable = ResponseIsValid(GetResponseData());
        }

        public void OnSavePressed()
        {
            SendResponseAndClose(GetResponseData());
        }

        public void OnCancelPressed()
        {
            CancelResponseAndClose();
        }
        
        protected SavePresetPromptResponse GetResponseData()
        {
            return new SavePresetPromptResponse
            {
                Name = _nameInputField.text
            };
        }

        protected override bool ResponseIsValid(SavePresetPromptResponse response) =>
            !string.IsNullOrWhiteSpace(_nameInputField.text);

        public class SavePresetPromptResponse
        {
            public string Name { get; set; }
        }
    }
}