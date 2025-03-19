using StarterAssets;
using UnityEngine;
using UnityEngine.UI;

public class StaminaBarUI : MonoBehaviour
{
    [SerializeField] private FirstPersonController player;
    [SerializeField] private Image staminaFill;

    private void Update()
    {
        if (player != null && staminaFill != null)
        {
            float newFillAmount = player.CurrentStamina / player.MaxStamina;
            staminaFill.fillAmount = newFillAmount;
        }
    }
}
