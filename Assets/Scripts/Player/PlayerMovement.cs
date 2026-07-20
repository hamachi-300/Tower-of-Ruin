using UnityEngine;

namespace TowerOfRuin
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 8.0f;
        [SerializeField] private float rotationSpeed = 15.0f;

        [Header("Stamina Settings")]
        [SerializeField] private float maxStamina = 100.0f;
        [SerializeField] private float staminaRegenRate = 30.0f; // Recover 30 per second
        [SerializeField] private float rollStaminaCost = 25.0f;
        private float currentStamina;

        [Header("Dodge Roll Settings")]
        [SerializeField] private float rollSpeed = 16.0f;
        [SerializeField] private float rollDuration = 0.4f;
        [SerializeField] private float rollCooldown = 0.8f;

        [Header("Lock-On Setting")]
        [SerializeField] private float lockOnRange = 15.0f;
        [SerializeField] private KeyCode lockOnKey = KeyCode.Q;

        private bool isRolling = false;
        private float rollTimer;
        private float cooldownTimer;
        private Vector3 rollDirection;

        private CharacterController characterController;
        private Transform lockOnTarget = null;

        // Public getters so other scripts (like UI) can read stamina values
        public float CurrentStamina => currentStamina;
        public float MaxStamina => maxStamina;
        public bool IsInvincible => isRolling;
        public Transform LockOnTarget => lockOnTarget;
        

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
            currentStamina = maxStamina;
        }

        private void Update()
        {
            // Tick Cooldown
            if (cooldownTimer > 0) cooldownTimer -= Time.deltaTime;



            // Regenerate Stamina smoothly when not rolling
            if (!isRolling && currentStamina < maxStamina)
            {
                currentStamina += staminaRegenRate * Time.deltaTime;
                currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);
            }

            // Check Lock-On Toggle Input
            if (Input.GetKeyDown(lockOnKey))
            {
                ToggleLockOn();
            }

            // Roll State Loop
            if (isRolling)
            {
                characterController.Move(rollDirection * rollSpeed * Time.deltaTime);
                rollTimer -= Time.deltaTime;
                if (rollTimer <= 0) isRolling = false;
                return; // Lock controls
            }

            // Normal Movement
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            Vector3 inputDirection = new Vector3(horizontal, 0f, vertical).normalized;

            Transform cam = Camera.main != null ? Camera.main.transform : null;
            Vector3 forward = cam != null ? cam.forward : Vector3.forward;
            Vector3 right = cam != null ? cam.right : Vector3.right;
            forward.y = 0f; right.y = 0f;
            forward.Normalize(); right.Normalize();

            Vector3 moveDirection = forward * inputDirection.z + right * inputDirection.x;

            // Seperate position movement from rotation
            if (inputDirection.magnitude >= 0.1f)
            {
                characterController.Move(moveDirection * moveSpeed * Time.deltaTime);
            }

            // Lock-On with Rotation
            if (lockOnTarget != null)
            {
                // Face lock-on target
                Vector3 targetDir = lockOnTarget.position - transform.position;
                targetDir.y = 0f;

                if (targetDir.magnitude > 0.1f)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(targetDir);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                }
            } 
            else if (inputDirection.magnitude >= 0.1f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }

            // Roll Input Check
            if (Input.GetKeyDown(KeyCode.Space) && cooldownTimer <= 0 && currentStamina >= rollStaminaCost)
            {
                rollDirection = inputDirection.magnitude >= 0.1f ? moveDirection : transform.forward;
                isRolling = true;
                rollTimer = rollDuration;
                cooldownTimer = rollCooldown;

                currentStamina -= rollStaminaCost; // Consume stamina
            }
        }

        private void ToggleLockOn()
        {
            // Release lock-on
            if (lockOnTarget != null)
            {
                lockOnTarget = null;
                return;
            }

            // Lock-On
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            Transform closestEnemy = null;
            float closestDistance = lockOnRange;

            foreach (GameObject enemy in enemies)
            {
                float distance = Vector3.Distance(transform.position, enemy.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestEnemy = enemy.transform;
                }
            }

            lockOnTarget = closestEnemy;
        }
    }
}
