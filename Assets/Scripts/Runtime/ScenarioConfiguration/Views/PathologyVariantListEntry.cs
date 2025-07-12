using medicaltraining.assetstore.ScenarioConfiguration.Serialization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.ScenarioConfiguration.Views
{
    /// <summary>
    /// UI entry of available pathology elements. Should be instantiated by the parent list.
    /// </summary>
    public class PathologyVariantListEntry : MonoBehaviour
    {
        [SerializeField] private GameObject _selectionMarker;
        [SerializeField] private TMP_Text _nameText;
        public PathologyVariant PathologyVariant { get; set; }

        public delegate void SelectPressedEvent();

        public event SelectPressedEvent SelectPressed;

        [SerializeField] private Button _selectButton;

        public delegate void DeletePressedEvent();

        public event DeletePressedEvent DeletePressed;

        [SerializeField] private Button _deleteButton;

        public void Start()
        {
            _selectButton?.onClick.AddListener(() => SelectPressed?.Invoke());
            _deleteButton?.onClick.AddListener(() => DeletePressed?.Invoke());
        }

        public void SetSelected(bool selected)
        {
            _selectionMarker.SetActive(selected);
        }
        public void SetName(string entryName)
        {
            _nameText.text = entryName;
        }

    }
}