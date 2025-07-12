using UnityEngine;
using UnityEngine.UI;

namespace Runtime.UI
{
    public class ListCollapser : MonoBehaviour
    {
        [SerializeField] private Sprite spriteOn;
        [SerializeField] private Sprite spriteOff;
        [SerializeField] private GameObject[] arrows;
        [SerializeField] private GameObject[] content;
        private int? expandedContent;

        void Awake()
        {
            Initialize();
        }

        public void Initialize()
        {
            CollapseAllContent();
            CollapseAllArrows();
        }

        public void ClickedArrow(int arrowID)
        {
            CollapseAllArrows();
            CollapseAllContent();
            if(arrowID == expandedContent)
            {
                expandedContent = null;
            }
            else
            {
                arrows[arrowID].GetComponent<Button>().image.sprite = spriteOn;
                content[arrowID].SetActive(true);
                expandedContent = arrowID;
            }
        }

        private void CollapseAllContent()
        {
            foreach(GameObject obj in content)
            {
                obj.SetActive(false);
            }
        }

        private void CollapseAllArrows()
        {
            foreach(GameObject arrow in arrows)
            {
                arrow.GetComponent<Button>().image.sprite = spriteOff;
            }
        }
    }
}