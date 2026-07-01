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
        [SerializeField] private float pitchMin = -60.0f;
        [SerializeField] private float pitchMax = 60.0f;

        private float currentYaw;
        private float currentPitch;
        private Renderer playerRenderer;

        private void Start()
        {
            FindPlayerTarget();
            
            // Find the player's Renderer component
            if (target != null) 
            {
                playerRenderer = target.GetComponent<Renderer>();
            }
            
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

            // Find the player's Renderer component
            if (playerRenderer == null)
            {
                playerRenderer = target.GetComponent<Renderer>();
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

            // --- CAMERA COLLISION CODE ---
            Vector3 targetPivot = target.position + Vector3.up * 1.2f; // Focus on player's chest
            RaycastHit hit;
            
            // Cast a line from player's chest to the camera's desired spot
            if (Physics.Linecast(targetPivot, desiredPosition, out hit))
            {
                // If we hit something (like the floor or a wall), push the camera slightly in front of the hit point
                desiredPosition = hit.point + hit.normal * 0.2f;
            }

            // Smoothly move camera
            transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

            // Look at player slightly above their pivot
            transform.LookAt(target.position + Vector3.up * 1.2f);

            // Calculate distance between camera and player chest
            float distance = Vector3.Distance(transform.position, targetPivot);

            if (playerRenderer != null) 
            {
                // Smoothly calculate alpha (transparency) based on distance
                // At 2.0 meters or more: alpha = 1.0 (fully visible)
                // At 1.0 meter or less: alpha = 0.0 (fully faded)
                float alpha = Mathf.InverseLerp(1.0f, 2.0f, distance);

                // Get current color, modify alpha, and apply it back
                Color color = playerRenderer.material.color;
                color.a = alpha;
                playerRenderer.material.color = color;
            }

        }
    }
}
