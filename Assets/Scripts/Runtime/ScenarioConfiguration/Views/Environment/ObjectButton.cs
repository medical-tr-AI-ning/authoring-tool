using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Runtime.ScenarioConfiguration.Views.Environment
{
    /// <summary>
    /// Button to select a prefab to place into the scene.
    /// </summary>
    public class ObjectButton : MonoBehaviour, IPointerDownHandler
    {
        [SerializeField] private string _prefabID;

        private Button _button;
        private EnvironmentView _interactionHandler;
        private bool _isPressed = false;

        private void Start()
        {
            _button = GetComponent<Button>();
            _interactionHandler = FindObjectOfType<EnvironmentView>();
        } 

        public void OnPointerDown(PointerEventData data)
        {
            _interactionHandler.StartPlacement(_prefabID);
        }
    }
}