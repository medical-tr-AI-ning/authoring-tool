using System.Collections.Generic;
using Runtime.UI;
using UnityEngine;

namespace Runtime.ScenarioConfiguration.Views.Pathologies
{
    /// <summary>
    /// UI list containing all created melanoma
    /// </summary>
    public class MelanomaUIList : MonoBehaviour
    {
        [SerializeField] private Transform _itemContainer;
        [SerializeField] private MelanomaUIListEntry _itemTemplate;
        [SerializeField] private PathologiesView _pathologiesView;
        [SerializeField] private GetBodySegmentation _getBodySegmentation;
        
        public void ClearList()
        {
            foreach (var entry in _itemContainer.GetComponentsInChildren<MelanomaUIListEntry>())
            {
                Destroy(entry.gameObject);
            }
        }

        public void AddListItem(Melanoma melanoma)
        {
            int newIndex = _itemContainer.GetComponentsInChildren<MelanomaUIListEntry>().Length;
            MelanomaUIListEntry item = Instantiate(_itemTemplate, _itemContainer);
            item.transform.SetSiblingIndex(newIndex);
            item.Melanoma = melanoma;
            item.SetDescription(_getBodySegmentation.GetDescriptor(melanoma.TextureCoord));
            item.SelectPressed += () => _pathologiesView.SelectMelanoma(item.Melanoma);
            item.DeletePressed += () => _pathologiesView.RequestDeleteMelanoma(item.Melanoma);
        }

        public void ReplaceItems(List<Melanoma> melanomas)
        {
            ClearList();
            foreach (Melanoma melanoma in melanomas)
            {
                AddListItem(melanoma);
            }
        }
    }
}