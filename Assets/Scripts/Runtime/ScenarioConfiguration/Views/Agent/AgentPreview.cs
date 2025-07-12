using System;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime.ScenarioConfiguration.Views.Agent
{
    /// <summary>
    /// Handles changes to the displayed agent of the agent configuration view.
    /// </summary>
    public class AgentPreview : MonoBehaviour
    {
        [SerializeField] List<AgentMapping> _agentMappings;

        public void showAgent(string agentID)
        {
            foreach (AgentMapping mapping in _agentMappings)
            {
                bool isSelectedAgent = mapping.AgentID == agentID;
                mapping.AgentGameObject.SetActive(isSelectedAgent);
            }
        }

        [Serializable]
        public class AgentMapping
        {
            public string AgentID;
            public GameObject AgentGameObject;
        }
        public GameObject GetAgentGameObjectByAgentID(string agentID)
        {
            foreach (AgentMapping mapping in _agentMappings)
            {
                if (mapping.AgentID == agentID) return mapping.AgentGameObject;
            }
            Debug.LogError($"No agent found for {agentID}");
            return null;
        }
    }
}