using UnityEngine;
using UnityEngine.Events;

public class PlayerInventory : MonoBehaviour
{
    private int key;

    public int Key { get => key; set => key = value; }

    public UnityEvent<PlayerInventory> OnKeyCollected;

    public void Collected()
    {
        Key++;
        OnKeyCollected.Invoke(this);
    }
}
