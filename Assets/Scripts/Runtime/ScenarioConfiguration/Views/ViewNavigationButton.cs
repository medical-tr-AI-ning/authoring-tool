using UnityEngine;
using UnityEngine.UI;

namespace Runtime.ScenarioConfiguration.Views
{
    /// <summary>
    /// Button that is pressed to open up a certain view.
    /// </summary>
    public class ViewNavigationButton : MonoBehaviour
    {
        private Button _button;
        public View View;
        [SerializeField] private GameObject _selectionMarker;
        private ViewsHandler _viewsHandler;

        private void Start()
        {
            _button = GetComponent<Button>();
            _viewsHandler = FindObjectOfType<ViewsHandler>();
            _button.onClick.AddListener(OnButtonPressed);
        }

        private void OnButtonPressed()
        {
            _viewsHandler.OnTabViewButtonPressed(this);
        }

        public void SetSelected(bool selected)
        {
            _selectionMarker.SetActive(selected);
        }
    }
}