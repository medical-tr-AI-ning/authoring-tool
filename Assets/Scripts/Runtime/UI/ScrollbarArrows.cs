using UnityEngine;
using UnityEngine.UI;

namespace Runtime.UI
{
    public class ScrollbarArrows : MonoBehaviour
    {
        private Scrollbar _scroll;
        private int _i;
        [SerializeField] private float _incr = 1f;

        void Start()
        {
            _scroll = GetComponent<Scrollbar>();
        }

        public void SetScroll(int i)
        {
            _i = i;
        }

        void Update()
        {
            _scroll.value += _incr * Time.deltaTime * _i;
        }  
    }
}
