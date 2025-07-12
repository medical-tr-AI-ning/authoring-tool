using System;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.ScenarioConfiguration.Views.Environment
{
    /// <summary>
    /// List item that represents one placed item in the environment view.
    /// </summary>
    public class PlacedObjectUIItem : MonoBehaviour
    {
        [NonSerialized] public PlacedObject PlacedObject;
        private EnvironmentView _handler;
        [SerializeField] private Button _deleteButton;

        private void Start()
        {
            _handler = FindObjectOfType<EnvironmentView>();
            _deleteButton.onClick.AddListener(OnDeletePressed);
        }

        private void OnDeletePressed()
        {
            _handler.RemovePlacedObject(PlacedObject);
        }
        
        public void OnHoverEnter()
        {
            PlacedObject.Color.ChangeToNewColor();
        }

        public void OnHoverExit()
        {
            PlacedObject.Color.ResetToOriginalColor();
        }
    }
}