using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.Common
{
    public abstract class SelectablesList<T> : MonoBehaviour where T : MonoBehaviour
    {
        private bool _initialized = false;
        [SerializeField] private GameObject _selectablesContainer;
        private SelectableEntry[] _selectables;
        private SelectableEntry _selectedEntry;
        [SerializeField] private Button _confirmButton;
        [SerializeField] private Button _cancelButton;


        public void Initialize()
        {
            if (_initialized) return;
            _initialized = true;

            _selectables = _selectablesContainer.GetComponentsInChildren<SelectableEntry>();
            foreach (SelectableEntry entry in _selectables)
            {
                entry.SelectPressed += () => SelectEntry(entry);
            }

            _confirmButton.onClick.AddListener(OnConfirmPressed);
            _cancelButton.onClick.AddListener(OnCancelPressed);

            UpdateUIStates();
        }

        private void Start()
        {
        }

        private void UpdateUIStates()
        {
            _confirmButton.interactable = _selectedEntry != null;
        }

        public void SelectEntry(T payload)
        {
            SelectableEntry targetEntry = _selectables.ToList().Find(
                entry => entry.GetPayload<T>().Equals(payload));

            SelectEntry(targetEntry);
        }

        private void SelectEntry(SelectableEntry selectableEntry)
        {
            foreach (SelectableEntry entry in _selectables)
            {
                bool isSelected = entry == selectableEntry;
                entry.UpdateSprite(isSelected);
            }

            _selectedEntry = selectableEntry;
            UpdateUIStates();
        }

        public void OnConfirmPressed()
        {
            OnSelectableConfirmed(_selectedEntry.GetPayload<T>());
        }

        public void OnCancelPressed()
        {
            SelectEntry((SelectableEntry) null);
            gameObject.SetActive(false);
        }

        protected abstract void OnSelectableConfirmed(T selectablePayload);
    }
}