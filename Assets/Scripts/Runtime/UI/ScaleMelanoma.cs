using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.UI
{
    public class ScaleMelanoma : MonoBehaviour
    {
        [SerializeField] private GameObject image;
        private RectTransform rt;
        private Vector2 startPivot;
        private Vector2 startPos;
        private Vector2 startSize;
        private int zoomLvl;
        private Vector2 oldZoomPivot;
        private Vector2 oldPos;
        [SerializeField] private TMP_Text txt;
        [SerializeField] private Button zoomIn;
        [SerializeField] private Button zoomOut;


        void Start()
        {
            rt = image.GetComponent<RectTransform>();
            startPivot = rt.pivot;
            startPos = rt.anchoredPosition;
            startSize = rt.sizeDelta;
        }

        public void ZoomIn(RectTransform rect)
        {
            if(zoomLvl < 2)
            {

                zoomLvl += 1;
                if(zoomLvl == 2)
                {
                    zoomIn.interactable = false;
                }
                else
                {
                    zoomIn.interactable = true;
                    zoomOut.interactable = true;
                }
                rt.pivot = (oldZoomPivot + rect.pivot) / zoomLvl;

                rt.sizeDelta = rt.sizeDelta * 2;
                rt.anchoredPosition = rect.anchoredPosition;

                if(zoomLvl == 1)
                {
                    oldZoomPivot = rt.pivot;
                    oldPos = rect.anchoredPosition;
                }


                UpdateZoomLvl();
            }
        }

        public void ZoomOut()
        {
            if(zoomLvl > 0)
        
            {       
                zoomLvl -= 1;
            
                if(zoomLvl == 1)
                {
                    rt.pivot = oldZoomPivot;
                    rt.anchoredPosition = oldPos;
                }

                if(zoomLvl == 0)
                {
                    zoomOut.interactable = false;
                    rt.pivot = startPivot;
                    oldZoomPivot = new Vector2(0,0);
                    rt.anchoredPosition = startPos;
                    Debug.Log(rt.anchoredPosition);
                }
                else
                {
                    zoomIn.interactable = true;
                    zoomOut.interactable = true;
                }            

                rt.sizeDelta = rt.sizeDelta / 2;
 
                UpdateZoomLvl();
            }
        }

        private void UpdateZoomLvl()
        {
            txt.text = (rt.sizeDelta.x/startSize.x) + "x";
        }
    }
}