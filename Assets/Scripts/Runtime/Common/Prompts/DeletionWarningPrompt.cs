using TMPro;
using UnityEngine;

namespace Runtime.Common.Prompts
{
    /// <summary>
    /// Warning prompt before deleting an important item.
    /// </summary>
    public class DeletionWarningPrompt : SimplePrompt
    {
        [SerializeField] private string _warningNotice;

        public void SetScenarioName(string scenarioName)
        {
            SetMessage(_warningNotice.Replace("{scenarioName}", scenarioName));
        }
    }
}