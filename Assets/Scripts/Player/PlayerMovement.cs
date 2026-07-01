using UnityEngine;

namespace TowerOfRuin
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 8.0f;
        [SerializeField] private float rotationSpeed = 15.0f;

        private CharacterController characterController;

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
            
            // Print a test message when the game starts
            Debug.Log("PlayerMovement script is Awake and ready!");
        }

        private void Update()
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

            Vector3 inputDirection = new Vector3(horizontal, 0f, vertical).normalized;

            if (inputDirection.magnitude >= 0.1f)
            {
                // Get Camera orientation safely
                Transform cam = Camera.main != null ? Camera.main.transform : null;

                Vector3 forward = cam != null ? cam.forward : Vector3.forward;
                Vector3 right = cam != null ? cam.right : Vector3.right;

                forward.y = 0f;
                right.y = 0f;
                forward.Normalize();
                right.Normalize();

                Vector3 moveDirection = forward * inputDirection.z + right * inputDirection.x;

                // Move the character controller
                characterController.Move(moveDirection * moveSpeed * Time.deltaTime);

                // Rotate towards movement direction smoothly
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }
    }
}
