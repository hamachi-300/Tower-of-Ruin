using UnityEngine;

namespace TowerOfRuin
{
    public class ThirdPersonCamera : MonoBehaviour
    {
        [Header("Target & Offset")]
        [SerializeField] private Transform target;
        [SerializeField] private Vector3 offset = new Vector3(0f, 2.5f, -4.5f);

        [Header("Follow Settings")]
        [SerializeField] private float smoothSpeed = 10.0f;

        [Header("Rotation Settings")]
        [SerializeField] private float mouseSensitivity = 3.0f;
        [SerializeField] private float pitchMin = -20.0f;
        [SerializeField] private float pitchMax = 60.0f;

        private float currentYaw;
        private float currentPitch;

        private void Start()
        {
            FindPlayerTarget();
            
            // Lock and hide cursor for smooth camera rotation
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void FindPlayerTarget()
        {
            if (target == null)
            {
                // Try finding by script first
                PlayerMovement pm = FindObjectOfType<PlayerMovement>();
                if (pm != null)
                {
                    target = pm.transform;
                }
                else
                {
                    // Try finding by name
                    GameObject playerObj = GameObject.Find("Player");
                    if (playerObj != null)
                    {
                        target = playerObj.transform;
                    }
                }
            }
        }

        private void LateUpdate()
        {
            if (target == null)
            {
                FindPlayerTarget();
                if (target == null) return;
            }

            // Get Mouse Input
            currentYaw += Input.GetAxis("Mouse X") * mouseSensitivity;
            currentPitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
            currentPitch = Mathf.Clamp(currentPitch, pitchMin, pitchMax);

            // Unlock cursor if ESC is pressed
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }

            // Calculate camera position and rotation
            Quaternion rotation = Quaternion.Euler(currentPitch, currentYaw, 0);
            Vector3 desiredPosition = target.position + rotation * offset;

            // Smoothly move camera
            transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

            // Look at player slightly above their pivot
            transform.LookAt(target.position + Vector3.up * 1.2f);
        }
    }
}
