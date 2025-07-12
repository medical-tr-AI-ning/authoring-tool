using System.Collections.Generic;
using System.Linq;
using medicaltraining.assetstore.ScenarioConfiguration.Serialization;
using Runtime.Common;
using Runtime.ScenarioConfiguration.Views.Pathologies;
using Runtime.UI;
using TMPro;
using UnityEngine;

namespace Runtime.ScenarioConfiguration.Views.Agent
{
    /// <summary>
    /// Handles the "Patient" view UI and logic.
    /// </summary>
    public class AgentView : ConfigurationView<AgentConfig>
    {
        [SerializeField] private AgentPreview _agentPreview;
        [SerializeField] private AgentOption _defaultAgentOption;
        private readonly List<AgentOption> _agentOptions = new List<AgentOption>();
        private AgentOption _selectedAgentOption;
        [SerializeField] private TMP_InputField _nameTextInput;
        [SerializeField] private TMP_InputField _ageTextInput;
        [SerializeField] private TMP_InputField _occupationTextInput;
        [SerializeField] private TMP_InputField _heightTextInput;
        [SerializeField] private TMP_InputField _weightTextInput;
        [SerializeField] private TMP_Text _agentDescriptionText;
        [SerializeField] private ToggleButton _showOnPatientCardToggle;
        [SerializeField] private AgentCameraController _camera;
        [SerializeField] private AgentSelectablesList _agentSelectablesList;

        public override void Initialize()
        {
            _agentSelectablesList.Initialize();
            SetSelectedAgent(_defaultAgentOption, true);
            foreach (AgentOption agent in GetComponentsInChildren<AgentOption>(includeInactive: true))
                _agentOptions.Add(agent);
            _showOnPatientCardToggle.Initialize();
        }

        public string GetSelectedAgent()
        {
            return _selectedAgentOption.AgentID;
        }

        public void SetSelectedAgent(AgentOption agent, bool updateSelectablesList)
        {
            _selectedAgentOption = agent;
            _heightTextInput.text = agent.Height;
            _agentDescriptionText.text = agent.DescriptionText;
            _agentPreview.showAgent(GetSelectedAgent());
            SelectedAgentChanged?.Invoke();
            if(updateSelectablesList) _agentSelectablesList.SelectEntry(agent);
        }

        public void SetSelectedAgent(string agentID, bool updateSelectablesList)
        {
            AgentOption agentOption = _agentOptions.FirstOrDefault(option => option.AgentID == agentID);
            if (!agentOption)
            {
                Debug.LogError($"Agent with ID {agentID} doesn't exist! Falling back to {_defaultAgentOption.AgentID}!");
                agentOption = _defaultAgentOption;
            }

            SetSelectedAgent(agentOption, updateSelectablesList);
        }

        public override AgentConfig GetConfiguration(string resourcesFolder)
        {
            return new AgentConfig
            {
                AgentID = GetSelectedAgent(),
                Name = _nameTextInput.text,
                Age = _ageTextInput.text,
                Occupation = _occupationTextInput.text,
                Height = _heightTextInput.text,
                Weight = _weightTextInput.text,
                ShowDetails = _showOnPatientCardToggle.GetToggledState()
            };
        }

        public override void LoadConfiguration(AgentConfig agentConfig, string resourcesFolder)
        {
            SetSelectedAgent(agentConfig.AgentID, true);
            _nameTextInput.text = agentConfig.Name;
            _ageTextInput.text = agentConfig.Age;
            _occupationTextInput.text = agentConfig.Occupation;
            _heightTextInput.text = agentConfig.Height;
            _weightTextInput.text = agentConfig.Weight;
            _showOnPatientCardToggle.SetToggleStateAndInvoke(agentConfig.ShowDetails);
        }

        public AgentData GetCurrentAgentData()
        {
            return _agentPreview.GetAgentGameObjectByAgentID(GetSelectedAgent()).GetComponent<AgentData>();
        }

        public AgentData GetAgentData(string agentID)
        {
            return _agentPreview.GetAgentGameObjectByAgentID(agentID).GetComponent<AgentData>();
        }
        
        public delegate void SelectedAgentChangedEvent();

        public event SelectedAgentChangedEvent SelectedAgentChanged;

        new void OnEnable()
        {
            base.OnEnable();
            GetCurrentAgentData().SetNaked(false);
            _camera.Reset1();
        }
    }
}