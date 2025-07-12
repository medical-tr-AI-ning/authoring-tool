using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Runtime.UI
{
    public class ToggleButton : MonoBehaviour
    {
        public bool buttonState = false;
        [SerializeField] private Sprite spriteOn;
        [SerializeField] private Sprite spriteOff;
        [SerializeField] private Button button;
        public GameObject toggleContent;
        [SerializeField] public UnityEvent OnToggleOn;
        [SerializeField] public UnityEvent OnToggleOff;

        void Awake()
        {
            Initialize();
        }

        public void Initialize()
        {
            UpdateVisuals();
        }

        public void Toggle()
        {
            SetToggleStateAndInvoke(!buttonState);
        }

        public void SetToggleStateAndInvoke(bool toggled)
        {
            var triggeredEvent = toggled ? OnToggleOn : OnToggleOff;
            triggeredEvent?.Invoke();
            SetToggleState(toggled);
        }

        public void SetToggleState(bool toggled)
        {
            buttonState = toggled;
            UpdateVisuals(); 
        }

        public bool GetToggledState() => buttonState;

        private void UpdateVisuals()
        {
            button.image.sprite = buttonState ? spriteOn : spriteOff;
            toggleContent.SetActive(buttonState);
        }
    }
}