using UnityEngine;

    [AddComponentMenu("Camera Navigation")]
    public class CameraHandler : MonoBehaviour
    {
        public LineRenderer targetPath;
        public LineRenderer missilePath;
        public float lookSpeed = 5f;
        public float moveSpeed = 5f;
        public float sprintSpeed = 50f;
        public float scrollSpeed = 20f; 
        public float lineWidth = 0.01f;
        public Camera ThisCamera;
        private float m_yaw = 0f;
        private float m_pitch = 0f;
        private float initialDistance = 10f;
        public Vector3 SavedPosition;
        public Vector3 SavedLookAt;
        void Start()
        {
            Cursor.visible = true;
        }
        public static CameraHandler instance;
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(this);
            }
        }

        void Update()
        {
            if(IsMouseInCameraViewport(ThisCamera, Input.mousePosition))
            {
                HandleInput();
                HandleMouseInput();
                HandleFreeLook();
                ScrollToAdjustInitialDistance();
                ResetCamera();
            }
        }
        bool IsMouseInCameraViewport(Camera camera, Vector3 mousePosition)
        {
            Rect viewportRect = camera.pixelRect;
            return viewportRect.Contains(mousePosition);
        }

        void HandleInput()
        {
            UpdateLineWidth(targetPath);
            UpdateLineWidth(missilePath);
        }

        void HandleFreeLook()
        {
            var speed = Time.deltaTime * (Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : moveSpeed);
            var forward = speed * Input.GetAxis("Vertical");
            var right = speed * Input.GetAxis("Horizontal");
            var up = speed * ((Input.GetKey(KeyCode.Q) ? 1f : 0f) - (Input.GetKey(KeyCode.E) ? 1f : 0f));
            transform.position += transform.forward * forward + transform.right * right + Vector3.up * up;
        }

        void HandleMouseInput()
        {
            if (!Input.GetMouseButton(1))
                return;

            m_yaw += lookSpeed * Input.GetAxis("Mouse X");
            m_pitch -= lookSpeed * Input.GetAxis("Mouse Y");
            m_pitch = Mathf.Clamp(m_pitch, -90f, 90f);
            transform.rotation = Quaternion.Euler(m_pitch, m_yaw, 0f);
        }
        void ResetCamera()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                transform.position = SavedPosition;
                transform.LookAt(SavedLookAt);
            }
            if (Input.GetKeyDown(KeyCode.T))
            {
                SavedPosition = transform.position;
                SavedLookAt = transform.position + transform.forward;
            }
        }
        void ScrollToAdjustInitialDistance()
        {
            float scrollDelta = Input.mouseScrollDelta.y;
            initialDistance = Mathf.Clamp(initialDistance - scrollDelta * scrollSpeed, 1f, 200f);
        }
        void UpdateLineWidth(LineRenderer lineRenderer)
        {
            lineWidth=initialDistance/10;
            lineRenderer.startWidth = lineWidth;
            lineRenderer.endWidth = lineWidth;
        }

    }