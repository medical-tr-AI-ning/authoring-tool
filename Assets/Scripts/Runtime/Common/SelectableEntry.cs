using System;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.Common
{
    public class SelectableEntry : MonoBehaviour
    {
        [SerializeField] private Sprite _defaultSprite;
        [SerializeField] private Sprite _selectedSprite;
        [SerializeField] private Image _image;
        [SerializeField] private Button _selectButton;
        
        public delegate void SelectPressedEvent();

        public event SelectPressedEvent SelectPressed;

        private void Start()
        {
            _selectButton.onClick.AddListener(() => SelectPressed?.Invoke());
        }

        public T GetPayload<T>() where T : MonoBehaviour
        {
            return GetComponent<T>();
        }

        public void UpdateSprite(bool selected)
        {
            _image.sprite = selected ? _selectedSprite : _defaultSprite;
        }
        
    }
}