using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Runtime.ScenarioConfiguration.Views
{
    /// <summary>
    /// Handles the interaction with a 3D View embedded in the UI
    /// </summary>
    public class Display3D : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] protected RawImage RawImage;
        [SerializeField] protected Camera ViewRenderCamera;
        protected RoomWindow roomView;
        private bool _inside = false;

        protected void Start()
        {
            roomView = RawImage.gameObject.GetComponent<RoomWindow>();
            Resize(ViewRenderCamera.targetTexture, Screen.width, Screen.height);
        }

        void Resize(RenderTexture renderTexture, int width, int height)
        {
            if (!renderTexture) return;
            renderTexture.Release();
            renderTexture.width = width;
            renderTexture.height = height;
        }

        public Camera GetViewRenderCamera() => ViewRenderCamera;
        public RoomWindow GetRoomWindow => roomView;

        public Vector2 GetMouseCoords(out bool inside)
        {
            var rt = RawImage.rectTransform;

            // Get point inside UI Rect
            inside = RectTransformUtility.ScreenPointToLocalPointInRectangle(
                RawImage.rectTransform,
                Input.mousePosition,
                GetComponent<Camera>(),
                out Vector2 pointInRect
            );

            if(!_inside) inside = false;

            // Turn Point in Rect, into Texture Coords
            Vector2 textureCoord = new Vector2(-1f, -1f);
            if (inside)
            {
                textureCoord = pointInRect - rt.rect.min;
                textureCoord.x *= RawImage.uvRect.width / rt.rect.width;
                textureCoord.y *= RawImage.uvRect.height / rt.rect.height;
                textureCoord += RawImage.uvRect.min;
            }
            
            if (textureCoord.x < 0f || textureCoord.x > 1f || textureCoord.y < 0f || textureCoord.y > 1f) 
                inside = false;
            else 
                inside = true;
            return textureCoord;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _inside = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _inside = false;
        }
    }
}