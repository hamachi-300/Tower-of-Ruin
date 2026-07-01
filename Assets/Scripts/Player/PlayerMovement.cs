using UnityEngine;

namespace TowerOfRuin
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 8.0f;
        [SerializeField] private float rotationSpeed = 15.0f;
        
        [Header("Dodge Roll Settings")]
        [SerializeField] private float rollSpeed = 16.0f;       // Speed during the roll
        [SerializeField] private float rollDuration = 0.4f;    // How long the roll lasts (in seconds)
        [SerializeField] private float rollCooldown = 0.8f;    // Time before you can roll again

        private bool isRolling = false;
        private float rollTimer;
        private float cooldownTimer;

        private Vector3 rollDirection;
        private CharacterController characterController;

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
            
            // Print a test message when the game starts
            Debug.Log("PlayerMovement script is Awake and ready!");
        }

        private void Update()
        {
            // 1. Tick Cooldown Timer
            if (cooldownTimer > 0)
            {
                cooldownTimer -= Time.deltaTime;
            }

            // 2. If we are currently rolling, execute roll movement
            if (isRolling)
            {
                // Move in the locked roll direction
                characterController.Move(rollDirection * rollSpeed * Time.deltaTime);

                // Tick down the roll timer
                rollTimer -= Time.deltaTime;
                if (rollTimer <= 0)
                {
                    isRolling = false; // End the roll
                }

                return; // Bypass normal WASD movement while rolling!
            }

            // 3. Normal WASD Movement
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

            Vector3 inputDirection = new Vector3(horizontal, 0f, vertical).normalized;

            // Calculate movement direction relative to camera
            Transform cam = Camera.main != null ? Camera.main.transform : null;
            Vector3 forward = cam != null ? cam.forward : Vector3.forward;
            Vector3 right = cam != null ? cam.right : Vector3.right;

            forward.y = 0f;
            right.y = 0f;
            forward.Normalize();
            right.Normalize();

            Vector3 moveDirection = forward * inputDirection.z + right * inputDirection.x;

            // Walk if pressing keys
            if (inputDirection.magnitude >= 0.1f)
            {
                characterController.Move(moveDirection * moveSpeed * Time.deltaTime);

                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }

            // 4. Listen for Dodge Roll input (Space Key)
            if (Input.GetKeyDown(KeyCode.Space) && cooldownTimer <= 0)
            {
                // If moving, roll in movement direction. Otherwise, roll forward.
                rollDirection = inputDirection.magnitude >= 0.1f ? moveDirection : transform.forward;
                
                isRolling = true;
                rollTimer = rollDuration;
                cooldownTimer = rollCooldown; // Start cooldown
            }
        }
    }
}
