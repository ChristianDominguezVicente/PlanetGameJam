using TMPro;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    private TextMeshProUGUI keyText;

    private void Start()
    {
        keyText = GetComponent<TextMeshProUGUI>();
    }

    public void UpdateText(PlayerInventory playerInventory)
    {
        keyText.text = playerInventory.Key.ToString();
    }
}
