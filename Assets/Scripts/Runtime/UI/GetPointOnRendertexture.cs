using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Runtime.UI
{
    public class GetPointOnRendertexture : MonoBehaviour
    {
        //IMPORTANT: The RawImage´s and object marker´s Anchor must be set to the bottom left corner.
        public Transform HighlightedObject;
        public UnityEvent Delete;
        public UnityEvent Pickup;
        public UnityEvent Edit;
        [SerializeField] Button _delete;
        [SerializeField] Button _place;
        [SerializeField] Button _edit;
        [SerializeField] TMP_Text _name;
        [SerializeField] private RectTransform _uiMarker;
        [SerializeField] private RectTransform _3dView;
        [SerializeField] private Camera _3dViewCam;
        [SerializeField] private RectTransform mainCanvas;

        // Variablen zur Berechnung der Position
        private Vector3 renderTexPos;
        private Vector2 screenPos;

        // Für das Erkennen von Klicks
        private GraphicRaycaster raycaster;
        private PointerEventData pointerEventData;
        private EventSystem eventSystem;

        void Start()
        {
            _delete?.onClick.AddListener(() => Delete.Invoke());
            _place?.onClick.AddListener(() => Pickup.Invoke());
            _edit?.onClick.AddListener(() => Edit.Invoke());
            _uiMarker.pivot = new Vector2(0.5f, 0.5f);

            raycaster = GetComponentInParent<GraphicRaycaster>();
            eventSystem = EventSystem.current;
        }

        void Update()
        {
            if (!HighlightedObject) return;

            // Verwendung von WorldToViewportPoint für Auflösungsunabhängigkeit
            renderTexPos = _3dViewCam.WorldToViewportPoint(HighlightedObject.position);

            // Berechnung der Bildschirmposition relativ zum Canvas (unter Berücksichtigung des Seitenverhältnisses)
            screenPos = new Vector2(renderTexPos.x * _3dView.sizeDelta.x, renderTexPos.y * _3dView.sizeDelta.y ) + _3dView.anchoredPosition;              

            // Setze die Position des UI-Markers entsprechend der berechneten Position
            _uiMarker.anchoredPosition = screenPos;

            // Überprüfe, ob der Nutzer außerhalb des Markers klickt, um ihn zu deaktivieren
            if (Input.GetMouseButtonDown(0))
            {
                pointerEventData = new PointerEventData(eventSystem);
                pointerEventData.position = Input.mousePosition;
                List<RaycastResult> results = new List<RaycastResult>();
                raycaster.Raycast(pointerEventData, results);

                bool clickedOnValid = false;
                foreach (RaycastResult result in results)
                {
                    if (result.gameObject == gameObject)
                    {
                        clickedOnValid = true;
                        break;
                    }
                }

                // Prüfe, ob auf das markierte Objekt in der 3D-Welt geklickt wurde
                Ray ray = _3dViewCam.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.transform == HighlightedObject)
                    {
                        clickedOnValid = true;
                    }
                }

                // Deaktiviere das Objekt, wenn nicht auf den Marker oder das Objekt geklickt wurde
                if (!clickedOnValid)
                {
                    gameObject.SetActive(false);
                }
            }
        }

        // Methode zum Setzen des Namens des markierten Objekts
        public void SetName(string name)
        {
            _name.text = name;
        }
    }
}
