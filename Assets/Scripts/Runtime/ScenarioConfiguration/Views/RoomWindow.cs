using UnityEngine;
using UnityEngine.EventSystems;

namespace Runtime.ScenarioConfiguration.Views
{
    /// <summary>
    /// Detects wether the Pointer is inside the UI Element or not
    /// </summary>
    public class RoomWindow : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public bool isInside = false;

        public void OnPointerEnter(PointerEventData eventData)
        {
            isInside = true;
            //Debug.Log("inside");
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            isInside = false;
            //Debug.Log("ouside");
        }
    }
}