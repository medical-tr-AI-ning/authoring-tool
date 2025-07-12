using medicaltraining.assetstore.ScenarioConfiguration.Serialization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.ScenarioList
{
    /// <summary>
    /// UI element that represents a Scenario within the Scenario List.
    /// </summary>
    public class ScenarioEntry : MonoBehaviour
    {
        [SerializeField] private TMP_Text _modificationDate;
        [SerializeField] private TMP_Text _title;
        [SerializeField] private TMP_Text _description;
        [SerializeField] private GameObject _submenue;
        [SerializeField] private Image _agentPreview;
        public VariableScenarioConfig Scenario { get; private set; }
        public string Directory { get; set; }

        private ScenarioListController _scenarioListController;
        private PreviewHandler _previewHandler;

        public void Awake()
        {
            _scenarioListController = FindObjectOfType<ScenarioListController>();
            _previewHandler = FindObjectOfType<PreviewHandler>();
        }

        public void AssignScenario(VariableScenarioConfig scenario)
        {
            _modificationDate.text = scenario.ModificationDate.ToShortDateString();
            _title.text = scenario.ScenarioName;
            _description.text = scenario.Description;
            Scenario = scenario;
            _agentPreview.sprite = _previewHandler.GetAgentPreview(scenario.GetDefaultAgent().AgentID);
        }

        public void OnEditPressed()
        {
            _scenarioListController.LoadSavedScenario(this);
        }

        public void OnExportPressed()
        {
            _scenarioListController.OpenExportPrompt(this);
        }

        public void OnDeletePressed()
        {
            _scenarioListController.OpenDeletionPrompt(this);
        }

        public void OnDuplicatePressed()
        {
            _scenarioListController.DuplicateScenarioEntry(this);
        }

        public void OnEditInformationPressed()
        {
            _scenarioListController.OpenEditInformationPrompt(this);
        }
    }
}