using UnityEngine;
using UnityEngine.UI;

namespace TowerOfRuin.UI
{
    public class StaminaUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Slider staminaSlider;
        [SerializeField] private PlayerMovement playerMovement;

        private void Start()
        {
            // If slider isn't assigned, try to get it on this same object
            if (staminaSlider == null)
            {
                staminaSlider = GetComponent<Slider>();
            }

            // Find the player automatically if not assigned
            if (playerMovement == null)
            {
                playerMovement = FindObjectOfType<PlayerMovement>();
            }
        }

        private void Update()
        {
            if (playerMovement != null && staminaSlider != null)
            {
                // Update slider fill amount (stamina slider value goes between 0 and 1)
                staminaSlider.value = playerMovement.CurrentStamina / playerMovement.MaxStamina;
            }
        }
    }
}
