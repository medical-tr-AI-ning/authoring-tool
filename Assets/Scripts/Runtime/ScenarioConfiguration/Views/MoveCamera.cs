using System.Collections;
using UnityEngine;

namespace Runtime.ScenarioConfiguration.Views
{
    /// <summary>
    /// Handles camera movement in the room configuration and agent configuration views.
    /// </summary>
    public class MoveCamera : MonoBehaviour
    {
        [SerializeField] private float rotationSpeed = 500f;
        [SerializeField] private float zoomSpeed = 3f;
        [SerializeField] private float panSpeed = 1f;
        [SerializeField] private Transform floor;
        [SerializeField] private Transform RotationObject;
        [SerializeField] private Camera RenderingCamera;
        private Vector3 mousePos;
        private Vector3 mousePosOld;
        private float f;
        private Transform parent;
        public bool pan = true;
        public bool zoom = true;
        public bool rotate = true;
        public float clampAngleUpper = 60.0f, clampAngleLower = 0f;
        public float clampX, clampZ, clampHeight;
        private float floorHeight;

        private float rotY = 0.0f; // rotation around the up/y axis
        private float rotX = 0.0f; // rotation around the right/x axis

        void Start()
        {
            Vector3 rot = transform.localRotation.eulerAngles;
            rotY = rot.y;
            rotX = rot.x;
            floorHeight = floor.position.y;
            parent = transform.parent;
        }

        void LateUpdate()
        {
            if (Input.GetMouseButtonDown(1) && !Input.GetMouseButton(0) && rotate)
            {
                StartCoroutine("CamOrbit");
            }

            //if(Input.GetMouseButton(2))
            if ((Input.GetMouseButtonDown(0) && Input.GetMouseButton(1) ||
                 Input.GetMouseButton(0) && Input.GetMouseButtonDown(1)) && pan)
            {
                StartCoroutine("Pan");
            }

            if (zoom)
            {
                Zoom(Input.mouseScrollDelta);
            }
        }

        IEnumerator CamOrbit()
        {
            GameObject objToSpawn = new GameObject("rotationParent");
            RaycastHit hitinfo = new RaycastHit();
            if (RotationObject)
            {
                objToSpawn.transform.position = RotationObject.position;
                objToSpawn.transform.rotation = transform.rotation;
            } else if(Physics.Raycast(RenderingCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f)), out hitinfo)){
                objToSpawn.transform.position = hitinfo.point;
                objToSpawn.transform.rotation = transform.rotation;
                Debug.DrawLine(transform.position, hitinfo.point, Color.red, 20);
            }
            else
            {
                objToSpawn.transform.position = transform.position;
                objToSpawn.transform.rotation = transform.rotation;
            }

            transform.SetParent(objToSpawn.transform);
            transform.localRotation = Quaternion.identity;

            while (Input.GetMouseButton(1) && !Input.GetMouseButton(0))
            {
                if (Input.GetAxis("Mouse Y") != 0 || Input.GetAxis("Mouse X") != 0)
                {
                    float mouseX = Input.GetAxis("Mouse X");
                    float mouseY = -Input.GetAxis("Mouse Y");

                    rotY += mouseX * rotationSpeed * Time.deltaTime;
                    rotX += mouseY * rotationSpeed * Time.deltaTime;

                    rotX = Mathf.Clamp(rotX, clampAngleLower, clampAngleUpper);

                    Quaternion localRotation = Quaternion.Euler(rotX, rotY, 0.0f);
                    objToSpawn.transform.rotation = localRotation;
                }

                yield return 0;
            }

            transform.SetParent(parent);
            transform.rotation = objToSpawn.transform.rotation;
            Destroy(objToSpawn);
            yield return 0;
        }


        void Zoom(Vector2 zoomDir)
        {
            if (zoomDir.y != 0)
            {
                if (zoomDir.y < 0)
                {
                    transform.localPosition = transform.localPosition - (transform.forward * zoomSpeed);
                }

                //rausscrollen
                if (zoomDir.y > 0)
                {
                    transform.localPosition = transform.localPosition + (transform.forward * zoomSpeed);
                }

                transform.localPosition = new Vector3(Mathf.Clamp(transform.localPosition.x, -clampX, clampX),
                    Mathf.Clamp(transform.localPosition.y, floorHeight, clampHeight),
                    Mathf.Clamp(transform.localPosition.z, -clampZ, clampZ));
            }
        }


        IEnumerator Pan()
        {
            while (true)
            {
                if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))
                {
                    break;
                }

                transform.localPosition += transform.localRotation *
                                           (new Vector3(-Input.GetAxis("Mouse X"), -Input.GetAxis("Mouse Y"), 0)) *
                                           panSpeed * Time.deltaTime;
                transform.localPosition = new Vector3(Mathf.Clamp(transform.localPosition.x, -clampX, clampX),
                    Mathf.Clamp(transform.localPosition.y, floorHeight, clampHeight),
                    Mathf.Clamp(transform.localPosition.z, -clampZ, clampZ));
                yield return 0;
            }

            yield return null;
        }
    }
}