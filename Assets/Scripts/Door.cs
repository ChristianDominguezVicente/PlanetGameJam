using System.Collections;
using TMPro;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private GameObject doorText;
    [SerializeField] private int keys = 1;

    private void OnTriggerEnter(Collider other)
    {
        PlayerInventory player = other.GetComponent<PlayerInventory>();

        if (player != null)
        {
            if(player.Key >= keys)
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
