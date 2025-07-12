using TMPro;
using UnityEngine;

namespace Runtime.UI
{
    public class TextLength : MonoBehaviour
    {
        [SerializeField] private TMP_InputField inputField;
        private int characterLimit;
        private TMP_Text counter;

        void Start()
        {
            inputField.onValueChanged.AddListener(OnTextChanged);
            characterLimit = inputField.characterLimit;     
            counter = GetComponent<TMP_Text>();
            counter.text = "0/" + characterLimit;

        }

        void OnTextChanged(string text)
        {
            int textLength = text.Length;
            counter.text = textLength + "/" + characterLimit;
        }

        void OnDestroy()
        {
            if (inputField != null)
            {
                inputField.onValueChanged.RemoveListener(OnTextChanged);
            }
        }
    }
}
