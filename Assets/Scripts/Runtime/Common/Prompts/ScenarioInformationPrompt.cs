using System.Collections;
using Runtime.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.Common.Prompts
{
    /// <summary>
    /// Prompt to enter title and description for a scenario.
    /// </summary>
    public class ScenarioInformationPrompt : ResponsePrompt<ScenarioInformationPrompt.ScenarioInformationPromptResponse>
    {
        [SerializeField] private TMP_InputField _descriptionInputField;
        [SerializeField] private TMP_InputField _titleInputField;
        [SerializeField] private StyledTextButton _submitButton;
        [SerializeField] private StyledTextButton _cancelButton;
        [SerializeField] private Button _saveAsNewButton;
        [SerializeField] private GameObject _saveAsNewNotice;
        [SerializeField] private ProgressBar _loadingScreen;
        private bool _destroyOnSubmit;

        private void Awake()
        {
            _submitButton.BaseButton.onClick.AddListener(OnConfirmButtonClicked);
            _cancelButton.BaseButton.onClick.AddListener(OnCancelButtonClicked);
            _saveAsNewButton.onClick.AddListener(OnSaveAsNewButtonClicked);
            _titleInputField.onValueChanged.AddListener(_ => UpdateUIStates());
            _descriptionInputField.onValueChanged.AddListener(_ => UpdateUIStates());
            _loadingScreen.gameObject.SetActive(false);
        }

        public void Configure(bool canSaveAsNew, bool destroyOnSubmit = true,
            string currentTitle = "", string currentDescription = "")
        {
            _saveAsNewButton.gameObject.SetActive(canSaveAsNew);
            _saveAsNewNotice.SetActive(canSaveAsNew);
            _destroyOnSubmit = destroyOnSubmit;
            _titleInputField.text = currentTitle;
            _descriptionInputField.text = currentDescription;
            UpdateUIStates();
        }

        private void  OnConfirmButtonClicked()
        {
            OnSave(false);
        }

        private void OnSaveAsNewButtonClicked()
        {
            OnSave(true);
        }

        private void OnSave(bool saveAsNew)
        {
            _loadingScreen.gameObject.SetActive(true);
            var response = GetResponseData(saveAsNew);
            if (_destroyOnSubmit)
                SendResponseAndClose(response);
            else
                SendResponse(response);
        }

        private ScenarioInformationPromptResponse GetResponseData(bool saveAsNew)
        {
            return new ScenarioInformationPromptResponse
            {
                Title = _titleInputField.text,
                Description = _descriptionInputField.text,
                SaveAsNew = saveAsNew
            };
        }

        private void OnCancelButtonClicked()
        {
            CancelResponseAndClose();
        }

        public void SetProgress(float progress)
        {
            _loadingScreen.SetProgress(progress);
        }

        public class ScenarioInformationPromptResponse
        {
            public string Title;
            public string Description;
            public bool SaveAsNew;
        }

        public void UpdateUIStates()
        {
            var currentResponse = GetResponseData(true);
            bool inputsValid = ResponseIsValid(currentResponse);
            _submitButton.SetInteractable(inputsValid);
            _saveAsNewButton.interactable = inputsValid;
        }

        protected override bool ResponseIsValid(ScenarioInformationPromptResponse response)
        {
            return !string.IsNullOrWhiteSpace(response.Title) &&
                   !string.IsNullOrWhiteSpace(response.Description);
        }
    }
}