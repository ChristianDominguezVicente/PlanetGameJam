using System.Collections;
using TMPro;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private GameObject doorText;

    private void OnTriggerEnter(Collider other)
    {
        PlayerInventory player = other.GetComponent<PlayerInventory>();

        if (player != null)
        {
            if(player.Key >= 1)
            {
                transform.parent.gameObject.SetActive(false);
            }
            else
            {
                StartCoroutine(ShowTextTemporarily());
            }
        }
    }

    private IEnumerator ShowTextTemporarily()
    {
        doorText.SetActive(true);
        yield return new WaitForSeconds(3f);
        doorText.SetActive(false);
    }
}
