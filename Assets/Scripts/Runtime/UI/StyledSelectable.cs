using UnityEngine;
using UnityEngine.UI;

namespace Runtime.UI
{
    public class StyledSelectable : MonoBehaviour
    {
        public bool DefaultState;
        [SerializeField] private Selectable[] selectables;
        [SerializeField] private Image[] currentImages;
        [SerializeField] private Sprite[] inactiveSprites;
        private Sprite[] activeSprites;
        private bool _initialized;

        void OnEnable()
        {
            EnsureInitialized();
        }

        private void EnsureInitialized()
        {
            if (_initialized) return;
            _initialized = true;
            if (currentImages == null || currentImages.Length == 0)
            {
                Debug.LogError("currentImages is not set or empty.");
                return;
            }

            // Active sprites are the sprites assigned by default
            activeSprites = new Sprite[currentImages.Length];
            for (int i = 0; i < currentImages.Length; i++)
            {
                if (currentImages[i] != null)
                {
                    var activeSprite = currentImages[i].sprite;
                    activeSprites[i] = activeSprite;
                }
                else
                {
                    Debug.LogError("currentImages[" + i + "] is null.");
                }
            }
            SetInteractable(DefaultState);
        }

        public void SetInteractable(bool state)
        {
            if (!_initialized)
            {
                EnsureInitialized();
            }

            //Change "interactable" property of selectables
            foreach (Selectable selectable in selectables)
            {
                selectable.interactable = state;
            }

            var newSprites = state ? activeSprites : inactiveSprites;
            SetSprites(newSprites);
        }

        private void SetSprites(Sprite[] sprites)
        {
            if (!_initialized) EnsureInitialized();
            for (int i = 0; i < currentImages.Length; i++)
            {
                currentImages[i].sprite = sprites[i];
            }
        }
    }
}