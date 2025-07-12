using TMPro;
using UnityEngine;

namespace Runtime.ScenarioConfiguration.Views.Environment
{
    /// <summary>
    /// UI list which includes all placed items in the Environment view.
    /// </summary>
    public class PlacedObjectUIItemList : MonoBehaviour
    {
        [SerializeField] private GameObject _itemContainer;
        [SerializeField] private PlacedObjectUIItem _itemTemplate;

        public void AddListItem(PlacedObject placedObject)
        {
            PlacedObjectUIItem item = Instantiate(_itemTemplate, _itemContainer.transform);
            item.PlacedObject = placedObject;
            item.gameObject.SetActive(true);
            TMP_Text itemText = item.GetComponentInChildren<TMP_Text>();
            itemText.text = placedObject.DisplayName;
        }

        public void RemoveListItem(PlacedObject placedObject)
        {
            PlacedObjectUIItem[] items = GetComponentsInChildren<PlacedObjectUIItem>();
            foreach (PlacedObjectUIItem item in items)
            {
                if (item.PlacedObject == placedObject)
                    Destroy(item.gameObject);
            }
        }
    }
}