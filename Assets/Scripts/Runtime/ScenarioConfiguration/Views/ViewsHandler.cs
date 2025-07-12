using UnityEngine;

namespace Runtime.ScenarioConfiguration.Views
{
    /// <summary>
    /// Handles switching between views and navigation-button UI states.
    /// </summary>
    public class ViewsHandler : MonoBehaviour
    {
        [SerializeField] private View[] _tabViews;
        [SerializeField] private ViewNavigationButton[] _tabViewButtons;

        private void Awake()
        {
            foreach (View tabView in _tabViews)
            {
                tabView.Initialize();
            }
        }

        public void OnTabViewButtonPressed(ViewNavigationButton pressedNavigationButton)
        {
            foreach (ViewNavigationButton button in _tabViewButtons)
            {
                button.SetSelected(button == pressedNavigationButton);
            }

            SwitchToView(pressedNavigationButton.View);
        }

        private void SwitchToView(View newActiveTabview)
        {
            foreach (View tabView in _tabViews)
            {
                tabView.gameObject.SetActive(false);
            }

            newActiveTabview.gameObject.SetActive(true);
        }
    }
}