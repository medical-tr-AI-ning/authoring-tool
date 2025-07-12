using System;
using UnityEngine;

namespace Runtime.ScenarioConfiguration.Views.Environment
{
    /// <summary>
    /// UI element for a selectable agent within a list of all agents.
    /// </summary>
    [Serializable]
    public class RoomOption : MonoBehaviour
    {
        public GameObject Room;
        public string RoomID;
        public string RoomName;
    }
}