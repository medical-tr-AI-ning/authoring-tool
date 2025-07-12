using UnityEngine;

namespace Runtime.ScenarioConfiguration.Views
{
    /// <summary>
    /// Top-class of all views (tabs) which can be opened by pressing the corresponding navigation button.
    /// </summary>
    public abstract class View : MonoBehaviour
    {
        [SerializeField] private GameObject _rootView;
        
        
        protected virtual void OnEnable()
        {
            if (_rootView != null)
            {
                _rootView.SetActive(true);
            }
        }

        protected virtual void OnDisable()
        {
            if (_rootView != null)
            {
                _rootView.SetActive(false);
            }
        }

        public virtual void Initialize()
        {
        }
    }
}