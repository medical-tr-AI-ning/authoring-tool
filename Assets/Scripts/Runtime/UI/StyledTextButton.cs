using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.UI
{
    public class StyledTextButton : MonoBehaviour
    {
        [SerializeField] private Color enabledTextCol = new Color(1,1,1,1);
        [SerializeField] private Color enabledIconCol = new Color(1,1,1,1);
        [SerializeField] private Color disabledTextCol = new Color(0,0,0,1);
        [SerializeField] private Color disabledIconCol = new Color(0,0,0,1);
        private Image[] icons;
        private TMP_Text txt;
        [SerializeField] public Button BaseButton;
        private bool _initialized = false;

        private void Awake()
        {
            EnsureInitialized();
            SetInteractable(BaseButton.interactable);
        }
    
        public void EnsureInitialized()
        {
            if (_initialized) return;
            txt = GetComponentInChildren<TMP_Text>();
            icons = GetComponentsInChildren<Image>();
            _initialized = true;
        }
        public void SetInteractable(bool interactable) 
        {
            EnsureInitialized();
            BaseButton.interactable = interactable;
            if(txt != null)
            {
                txt.color = interactable ? enabledTextCol : disabledTextCol;
            }

            foreach(Image icon in icons) 
            {
                if(icon.gameObject != gameObject)
                {
                    icon.color = interactable ? enabledIconCol : disabledIconCol;
                }
            }        
        }
    }
}
