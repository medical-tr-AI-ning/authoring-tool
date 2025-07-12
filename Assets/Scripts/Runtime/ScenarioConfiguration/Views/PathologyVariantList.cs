using System.Collections.Generic;
using System.Linq;
using medicaltraining.assetstore.ScenarioConfiguration.Serialization;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.ScenarioConfiguration.Views
{
    /// <summary>
    /// UI list of available pathology variants
    /// </summary>
    public class PathologyVariantList : MonoBehaviour
    {
        [SerializeField] private Transform _itemContainer;
        [SerializeField] private PathologyVariantListEntry _itemTemplate;
        [SerializeField] private PathologyVariantHandler _pathologyVariantDataHandler;
        [SerializeField] private Button _addItemButton;

        [SerializeField] private float _singleRowLayoutHeight;
        [SerializeField] private float _twoRowsLayoutHeight;
        [SerializeField] private int _itemsPerRow;
        [SerializeField] private GameObject _scrollBar;


        private void Awake()
        {
            if (_addItemButton != null)
                _addItemButton.onClick.AddListener(() => _pathologyVariantDataHandler.OnVariantCreateRequested());
        }

        public void ClearList()
        {
            foreach (var entry in _itemContainer.GetComponentsInChildren<PathologyVariantListEntry>())
            {
                Destroy(entry.gameObject);
            }
        }

        public void AddListItem(PathologyVariant pathologyVariant, string entryName)
        {
            int newIndex = _itemContainer.GetComponentsInChildren<PathologyVariantListEntry>().Length;
            PathologyVariantListEntry item = Instantiate(_itemTemplate, _itemContainer);
            item.transform.SetSiblingIndex(newIndex);
            item.SetName(entryName);
            item.PathologyVariant = pathologyVariant;
            item.SelectPressed += () => _pathologyVariantDataHandler.OnVariantSelectRequested(item.PathologyVariant);
            item.DeletePressed += () => _pathologyVariantDataHandler.OnVariantRemoveRequested(item.PathologyVariant);
        }

        public void ReplaceItems(List<PathologyVariant> pathologyVariants)
        {
            ClearList();
            int counter = 1;
            foreach (PathologyVariant pathologyVariant in pathologyVariants)
            {
                string entryName = $"Hautbild {counter}";
                AddListItem(pathologyVariant, entryName);
                counter++;
            }

            updateLayout();
        }

        public List<PathologyVariantListEntry> GetItems()
        {
            return _itemContainer.GetComponentsInChildren<PathologyVariantListEntry>().ToList();
        }

        public void SetActiveItem(PathologyVariant pathologyVariant)
        {
            foreach (var item in GetItems())
            {
                bool isSelected = item.PathologyVariant == pathologyVariant;
                item.SetSelected(isSelected);
            }
        }

        private void updateLayout()
        {
            int itemCount = GetItems().Count;
            setLayout(itemCount > _itemsPerRow);
        }

        private void setLayout(bool twoRows)
        {
            float newHeight = twoRows ? _twoRowsLayoutHeight : _singleRowLayoutHeight;
            var rect = GetComponent<RectTransform>();
            var newSize = new Vector2(rect.sizeDelta.x, newHeight);
            rect.sizeDelta = newSize;
            _scrollBar.SetActive(twoRows);

            ForceLayoutGroupUpdate();
        }

        private void ForceLayoutGroupUpdate()
        {
            //Uneasy lies the head that does UI. (Shakespeare)
            gameObject.SetActive(false);
            gameObject.SetActive(true);
        }
    }
}