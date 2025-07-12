using System;
using UnityEngine;

namespace Runtime.ScenarioConfiguration.Views.Agent
{
    /// <summary>
    /// UI element for a selectable agent within a list of all agents.
    /// </summary>
    [Serializable]
    public class AgentOption : MonoBehaviour
    {
        public string AgentID;
        public string DescriptionText;
        public string Height;
    }
}