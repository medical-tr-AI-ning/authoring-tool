using UnityEngine;
using UnityEngine.UI;

namespace Runtime.UI
{
    public class TexturePainter : MonoBehaviour
    {
        public Texture2D brush;
        public Texture2D canvas;
        public float size;

        [SerializeField] protected Image roomView;

        private Vector2 getMouseCoords()
        {
            var rt = roomView.rectTransform;

            // Get point inside UI Rect
            bool inside = RectTransformUtility.ScreenPointToLocalPointInRectangle(
                roomView.rectTransform,
                Input.mousePosition,
                GetComponent<Camera>(),
                out Vector2 pointInRect
            );

            // Turn Point in Rect, into Texture Coords
            Vector2 textureCoord = new Vector2(0f, 0f);
            if (inside)
            {
                textureCoord.x = (pointInRect.x+256)/512;
                textureCoord.y = (pointInRect.y+256)/512;
            }
            return textureCoord;
        }

        public void Paint()
        {
            AddMelanoma(getMouseCoords(), canvas, brush, size);   
        }

        public void AddMelanoma(Vector2 uvPosition, Texture2D Canvas, Texture2D Brush, float Size)
        {
            Texture2D melanomaTexture = Resize(Brush ,Mathf.FloorToInt(Brush.width * Size),Mathf.FloorToInt(Brush.height * Size));
            int x = Mathf.FloorToInt(uvPosition.x * Canvas.width);
            int y = Mathf.FloorToInt(uvPosition.y * Canvas.height);

            int melanomaWidth = melanomaTexture.width;
            int melanomaHeight = melanomaTexture.height;

            for (int i = 0; i < melanomaWidth; i++)
            {
                for (int j = 0; j < melanomaHeight; j++)
                {
                    int pixelX = x + i - melanomaWidth / 2;
                    int pixelY = y + j - melanomaHeight / 2;

                    if (pixelX >= 0 && pixelX < Canvas.width && pixelY >= 0 && pixelY < Canvas.height)
                    {
                        Color melanomaColor = melanomaTexture.GetPixel(i, j);
                        if (melanomaColor.a > 0) // Blend only if the melanoma pixel is not transparent
                        {
                            Color originalColor = Canvas.GetPixel(pixelX, pixelY);
                            Color blendedColor = Color.Lerp(originalColor, melanomaColor, melanomaColor.a);
                            blendedColor.a = melanomaColor.a;
                            Canvas.SetPixel(pixelX, pixelY, blendedColor);
                        }
                    }
                }
            }
            Canvas.Apply();
        }

        Texture2D Resize(Texture2D texture2D, int targetX, int targetY)
        {
            RenderTexture rt = new RenderTexture(targetX, targetY, 24);
            RenderTexture.active = rt;
            Graphics.Blit(texture2D, rt);
            Texture2D result = new Texture2D(targetX, targetY);
            result.ReadPixels(new Rect(0, 0, targetX, targetY), 0, 0);
            result.Apply();
            return result;
        }
    }
}
