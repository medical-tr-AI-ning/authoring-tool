using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Runtime.Common.Prompts;
using UnityEngine;

namespace Runtime.Common
{
    public class PromptHandler : MonoBehaviour
    {
        public static PromptHandler Instance;

        [SerializeField] private Transform promptsContainer;

        [SerializeField] private GameObject blockingOverlay;

        [SerializeField] private Vector2 defaultOffset;

        private Dictionary<PromptType, SimplePrompt> _prompts;

        [SerializeField] private SimplePrompt _deleteMelanomaPromptTemplate;
        [SerializeField] private SimplePrompt _deleteVariantPromptTemplate;
        [SerializeField] private SimplePrompt _deleteScenarioPromptTemplate;
        [SerializeField] private SimplePrompt _changeAgentPromptTemplate;
        [SerializeField] private SimplePrompt _changeEnvironmentPromptTemplate;
        [SerializeField] private SimplePrompt _loadPresetPromptTemplate;
        [SerializeField] private SimplePrompt _deletePresetPromptTemplate;
        [SerializeField] private SimplePrompt _copyAnamnesisDataPromptTemplate;
        [SerializeField] private SimplePrompt _returnToScenarioListPromptTemplate;

        void Start()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(this);
            }

            //Set up prompt templates

            _prompts = new Dictionary<PromptType, SimplePrompt>
            {
                { PromptType.DeleteMelanoma, _deleteMelanomaPromptTemplate },
                { PromptType.DeleteVariant, _deleteVariantPromptTemplate },
                { PromptType.DeleteScenario, _deleteScenarioPromptTemplate },
                { PromptType.ChangeAgent, _changeAgentPromptTemplate },
                { PromptType.ChangeEnvironment, _changeEnvironmentPromptTemplate },
                { PromptType.LoadPreset, _loadPresetPromptTemplate },
                { PromptType.DeletePreset, _deletePresetPromptTemplate },
                { PromptType.CopyAnamnesisData, _copyAnamnesisDataPromptTemplate },
                { PromptType.ReturnToScenarioList, _returnToScenarioListPromptTemplate }
            };
        }

        public enum PromptType
        {
            DeleteMelanoma,
            DeleteVariant,
            DeleteScenario,
            ChangeAgent,
            ChangeEnvironment,
            LoadPreset,
            DeletePreset,
            CopyAnamnesisData,
            ReturnToScenarioList
        }

        public void DisplayPrompt(PromptType promptType, Action onConfirm = null, Action onCancel = null,
            string title = "",
            string message = "", Vector2 offset = default)
        {
            SimplePrompt promptTemplate = _prompts[promptType];
            DisplayPrompt(promptTemplate, onConfirm, onCancel, title, message, offset);
        }

        public void DisplayPrompt(SimplePrompt promptTemplate, Action onConfirm = null, Action onCancel = null,
            string title = "",
            string message = "", Vector2 offset = default)
        {
            var prompt = DisplayPrompt(promptTemplate.gameObject, offset).GetComponent<SimplePrompt>();
            var blocker = Instantiate(blockingOverlay, promptsContainer);
            blocker.transform.SetSiblingIndex(prompt.transform.GetSiblingIndex());

            //Change title and message if provided
            if (!String.IsNullOrWhiteSpace(title)) prompt.SetTitle(message);
            if (!String.IsNullOrWhiteSpace(message)) prompt.SetMessage(message);

            //Setup action hooks
            if (onConfirm != null) prompt.ConfirmPressed += onConfirm.Invoke;
            if (onCancel != null) prompt.CancelPressed += onCancel.Invoke;
            prompt.ConfirmPressed += () => Destroy(blocker);
            prompt.CancelPressed += () => Destroy(blocker);
        }

        public ResponsePrompt<T> DisplayPrompt<T>(ResponsePrompt<T> promptTemplate, Vector2 offset = default)
        {
            if (offset == default) offset = defaultOffset;

            var prompt = DisplayPrompt(promptTemplate.gameObject, offset).GetComponent<ResponsePrompt<T>>();
            var blocker = Instantiate(blockingOverlay, promptsContainer);
            blocker.transform.SetSiblingIndex(prompt.transform.GetSiblingIndex());

            prompt.PromptClosed += () => Destroy(blocker);
            return prompt;
        }

        public GameObject DisplayPrompt(GameObject promptTemplate, Vector2 offset = default)
        {
            if (offset == default) offset = defaultOffset;
            GameObject prompt = Instantiate(promptTemplate, promptsContainer);
            //Set Prompt position
            prompt.GetComponent<RectTransform>().anchoredPosition = offset;
            return prompt;
        }
    }
}