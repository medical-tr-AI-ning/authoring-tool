using UnityEngine;

namespace Runtime.Common
{
    /// <summary>
    /// Handles different mouse cursor stylings
    /// </summary>
    public class SetCursor : MonoBehaviour
    {
        [SerializeField] private Texture2D _cursorTexture1;
        [SerializeField] private CursorMode _cursorMode1 = CursorMode.Auto;
        [SerializeField] private Vector2 _hotSpot1 = Vector2.zero;
        [SerializeField] private Texture2D _cursorTexture2;
        [SerializeField] private CursorMode _cursorMode2 = CursorMode.Auto;
        [SerializeField] private Vector2 _hotSpot2 = Vector2.zero;
    
        private void Start()
        {
            DontDestroyOnLoad(gameObject);
            Cursor1();
        }

        public void Cursor1()
        {
            Cursor.SetCursor(_cursorTexture1, _hotSpot1, _cursorMode1);
        }

        public void Cursor2()
        {
            Cursor.SetCursor(_cursorTexture2, _hotSpot2, _cursorMode2);
        }

    }
}