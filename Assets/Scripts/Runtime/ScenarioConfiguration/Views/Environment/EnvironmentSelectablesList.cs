using Runtime.Common;
using Runtime.ScenarioConfiguration.Views.Agent;
using UnityEngine;

namespace Runtime.ScenarioConfiguration.Views.Environment
{
    public class EnvironmentSelectablesList : SelectablesList<RoomOption>
    {
        [SerializeField] EnvironmentView _environmentView;
        protected override void OnSelectableConfirmed(RoomOption selectablePayload)
        {
            PromptHandler.Instance.DisplayPrompt(PromptHandler.PromptType.ChangeEnvironment,
                onConfirm: () =>
                {
                    _environmentView.SetSelectedRoom(selectablePayload, false);
                    gameObject.SetActive(false);
                });
           
        }
    }
}