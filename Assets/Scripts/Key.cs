using UnityEngine;
using FMODUnity;

public class Key : MonoBehaviour
{
    [SerializeField] private EventReference keyCollectedSound;
    private void OnTriggerEnter(Collider other)
    {
        PlayerInventory playerInventory = other.GetComponent<PlayerInventory>();

        if(playerInventory != null)
        {
            playerInventory.Collected();
            AudioManager.instance.PlayOneShot(keyCollectedSound, this.transform.position);
            gameObject.SetActive(false);
        }
    }
}
