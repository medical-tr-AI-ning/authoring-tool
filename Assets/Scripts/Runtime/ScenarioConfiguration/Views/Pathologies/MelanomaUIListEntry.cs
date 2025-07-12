using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.ScenarioConfiguration.Views.Pathologies
{
    /// <summary>
    /// Entry representing a melanoma created by the user. Should be instantiated by the list.
    /// </summary>
    public class MelanomaUIListEntry : MonoBehaviour
    {
        public Melanoma Melanoma;

        public delegate void SelectPressedEvent();

        public event SelectPressedEvent SelectPressed;
        
        [SerializeField] private Button _selectButton;
        [SerializeField] private TMP_Text _descriptionText;
        
        public delegate void DeletePressedEvent();

        public event DeletePressedEvent DeletePressed;
        
        [SerializeField] private Button _deleteButton;

        public void Start()
        {
            _selectButton.onClick.AddListener(() => SelectPressed?.Invoke());
            _deleteButton.onClick.AddListener(() => DeletePressed?.Invoke());
        }

        public void SetDescription(string description)
        {
            _descriptionText.text = description;
        }
    }
}