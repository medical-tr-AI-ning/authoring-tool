using System;
using System.Linq;
using UnityEngine;

namespace Runtime.ScenarioList
{
    /// <summary>
    /// Controls the preview images for scenario entries.
    /// </summary>
    public class PreviewHandler : MonoBehaviour
    {
        //Simple implementation for now
        [SerializeField] private AgentPreview[] _agentPreviewMapping;

        public Sprite GetAgentPreview(string agentID)
        {
            try
            {
                return _agentPreviewMapping.First(preview => preview.AgentID == agentID).Preview;
            }
            catch
            {
                Debug.LogError($"No matching agent preview for agent {agentID}");
                return null;
            }
        }

        [Serializable]
        public class AgentPreview
        {
            public string AgentID;
            public Sprite Preview;
        }
    }
}