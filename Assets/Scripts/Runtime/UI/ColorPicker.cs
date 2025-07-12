using UnityEngine;
using UnityEngine.Events;

namespace Runtime.UI
{
    public class ColorPicker : MonoBehaviour
    {
        [SerializeField] private Transform colorGizmo;
        [SerializeField] private Texture2D colorGradient;
        private float width;
        private float height;
        public bool active {set; get;}
        public UnityEvent onValueChanged;
        private int textureHeight;

        void Awake()
        {
            width = GetComponent<RectTransform>().rect.width;
            height = GetComponent<RectTransform>().rect.height;
            textureHeight = colorGradient.height;
        }

        void Update()
        {
            if(active)
            {
                colorGizmo.position = Input.mousePosition;
                colorGizmo.localPosition = new Vector2 (Mathf.Clamp(colorGizmo.localPosition.x, 0, width), Mathf.Clamp(colorGizmo.localPosition.y, -height, 0));
                onValueChanged.Invoke();
            }
        }

        public Color GetColor()
        {
            return colorGradient.GetPixel((int)colorGizmo.transform.localPosition.x * 2, textureHeight + (int)colorGizmo.transform.localPosition.y * 2, 0);
        }

        public void SetPosition(Vector2 pos)
        {
            colorGizmo.localPosition = pos;
        }

        public Vector2 GetPosition()
        {
            return colorGizmo.localPosition;
        }
    }
}
