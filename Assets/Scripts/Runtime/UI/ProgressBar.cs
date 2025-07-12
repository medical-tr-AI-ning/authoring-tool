using UnityEngine;

namespace Runtime.UI
{
    public class ProgressBar : MonoBehaviour
    {
        [SerializeField] private RectTransform _progressBar;
        [SerializeField] private RectTransform _background;

        void Awake()
        {
            SetProgress(0);
        }

        public void SetProgress(float prog)
        {
            _progressBar.sizeDelta = new Vector2(_background.rect.width * prog, _progressBar.sizeDelta.y);
        }
    }
}
