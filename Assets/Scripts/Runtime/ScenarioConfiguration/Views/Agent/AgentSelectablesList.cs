using Runtime.Common;
using UnityEngine;

namespace Runtime.ScenarioConfiguration.Views.Agent
{
    public class AgentSelectablesList : SelectablesList<AgentOption>
    {
        [SerializeField] AgentView _agentView;
        protected override void OnSelectableConfirmed(AgentOption selectablePayload)
        {
            PromptHandler.Instance.DisplayPrompt(PromptHandler.PromptType.ChangeAgent,
                onConfirm: () =>
                {
                    _agentView.SetSelectedAgent(selectablePayload, false); 
                    gameObject.SetActive(false);
                });
           
        }
    }
}