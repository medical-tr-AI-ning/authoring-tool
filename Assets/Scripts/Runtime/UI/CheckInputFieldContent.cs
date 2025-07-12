using TMPro;
using UnityEngine;

namespace Runtime.UI
{
    public class CheckInputFieldContent : MonoBehaviour
    {
        [SerializeField] private TMP_InputField inputField;
        private TMP_Text marker;

        void Start()
        {
            marker = GetComponent<TMP_Text>();
        }

        void OnEnable()
        {
            inputField.onValueChanged.AddListener(CheckText);    
        }

        void OnDisable()
        {
            inputField.onValueChanged.RemoveListener(CheckText);
        }

        private void CheckText(string newValue)
        {
            //If the string is empty, enable the marker. Else, disable it.
            if (string.IsNullOrWhiteSpace(newValue))
            {
                marker.enabled = true;
            }
            else
            {
                marker.enabled = false;
            }
        } 
    }
}
