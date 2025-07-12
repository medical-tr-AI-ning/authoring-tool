using UnityEngine;

namespace Runtime.ScenarioConfiguration.Views.Agent
{
    /// <summary>
    /// Handles the agent camera and its position
    /// </summary>
    public class AgentCameraController : MonoBehaviour
    {
        public Transform characterTarget; // Character's transform
        public Camera mainCamera; // Main camera used for the view
        public LayerMask interactionLayers; // Layers that the camera should interact with

        public float rotationSpeed = 5f;
        public float moveSpeed = 0.5f;
        public float zoomSpeed = 2f;
        public float minDistance = 0.5f; // Minimum distance from the character
        public float maxDistance = 20f; // Maximum distance

        private Vector3 currentFocusPoint;
        private float currentDistance;
        private float pitch;
        private float yaw;
        private bool isMoving = false;
        public float clampX, clampZ, clampY;

        public void Reset1()
        {
            currentFocusPoint = characterTarget.position; // Set initial focus point
            //UpdateFocusPointIfNeeded();
            currentDistance = 5f; // Initial camera distance
            pitch = 15f;
            yaw = 160f;
            UpdateCameraPosition();
        }

        public void Reset2()
        {
            currentFocusPoint = characterTarget.position; // Set initial focus point
            currentFocusPoint.y = currentFocusPoint.y + .4f;
            //UpdateFocusPointIfNeeded();
            currentDistance = 1.5f; // Initial camera distance
            pitch = 15f;
            yaw = 160f;
            UpdateCameraPosition();
        }

        void Update()
        {
            if (!Input.GetMouseButton(1)) // Update focus point only if the right button is not clicked
            {
                UpdateFocusPointIfNeeded();
            }

            if (Input.GetMouseButton(0) && Input.GetMouseButton(1))
            {
                HandleMovement();
                isMoving = true;
            }
            else
            {
                if (Input.GetMouseButton(1)) // Right click only
                {
                    HandleRotation();
                }
                isMoving = false;
            }

            if (!isMoving)
            {
                HandleZoom();
            }

            UpdateCameraPosition();
            //PreventCameraPenetration();
        }

        public void SetStartPosition(int i)
        {

        }

        private void UpdateFocusPointIfNeeded()
        {
            RaycastHit hit;
        
            Ray ray = mainCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
            if (Physics.Raycast(ray, out hit, 100f, interactionLayers))
            {
                if((hit.point - currentFocusPoint).magnitude > .1f)
                {
                    // Calculate new distance from the camera to the hit point
                    currentDistance = Vector3.Distance(transform.position, hit.point);

                    currentFocusPoint = hit.point;

                    // Ensure the distance is within set bounds
                    currentDistance = Mathf.Clamp(currentDistance, minDistance, maxDistance);

                    // Update camera position with new focus point and distance
                    UpdateCameraPosition();
                }
            }
        }


        private void HandleRotation()
        {
            yaw += Input.GetAxis("Mouse X") * rotationSpeed;
            pitch -= Input.GetAxis("Mouse Y") * rotationSpeed;
            pitch = Mathf.Clamp(pitch, -90, 90);
            UpdateCameraPosition();
        }

        private void HandleMovement()
        {
            Vector3 moveDir = (-transform.right * Input.GetAxis("Mouse X") + -transform.up * Input.GetAxis("Mouse Y")) * moveSpeed * currentDistance;
            transform.Translate(moveDir, Space.World);
            transform.localPosition = new Vector3(Mathf.Clamp(transform.localPosition.x, -clampX, clampX),
                Mathf.Clamp(transform.localPosition.y, -clampY, clampY),
                Mathf.Clamp(transform.localPosition.z, -clampZ, clampZ));
            currentFocusPoint = transform.position + transform.forward * currentDistance; // Update the focus point to maintain the view
        }


        private void HandleZoom()
        {
            float zoomDelta = Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
            currentDistance -= zoomDelta;
            currentDistance = Mathf.Clamp(currentDistance, minDistance, maxDistance);
            UpdateCameraPosition();
        }

        private void UpdateCameraPosition()
        {
            Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
            Vector3 negDistance = new Vector3(0.0f, 0.0f, -currentDistance);
            Vector3 position = rotation * negDistance + currentFocusPoint;

            transform.rotation = rotation;
            transform.position = position;
        }

        private void OnCollisionStay(Collision other) {
            if((transform.position - other.contacts[0].point).magnitude < 0.1){
                currentFocusPoint += (transform.position - other.contacts[0].point);
                UpdateCameraPosition();
                GetComponent<Rigidbody>().velocity = Vector3.zero;
                GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            }
        }
    }
}
