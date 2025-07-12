using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.Common.Prompts
{
    /// <summary>
    /// Simple prompt with a confirm and cancel button.
    /// </summary>
    public class SimplePrompt : MonoBehaviour
    {
        [SerializeField] private TMP_Text _title;
        [SerializeField] private TMP_Text _message;
        public delegate void ConfirmPressedEvent();

        public event ConfirmPressedEvent ConfirmPressed;

        public delegate void CancelPressedEvent();

        public event CancelPressedEvent CancelPressed;

        [SerializeField] private Button _cancelButton;
        [SerializeField] private Button _confirmButton;

        public void Start()
        {
            _cancelButton.onClick.AddListener(OnCancelPressed);
            _confirmButton.onClick.AddListener(OnConfirmPressed);
        }

        private void OnCancelPressed()
        {
            CancelPressed?.Invoke();
            Destroy(gameObject);
        }

        private void OnConfirmPressed()
        {
            ConfirmPressed?.Invoke();
            Destroy(gameObject);
        }
        
        public void SetMessage(string message) => _message.text = message;
        
        public void SetTitle(string title) => _title.text = title;
    }
}