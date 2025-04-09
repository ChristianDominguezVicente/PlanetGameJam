using StarterAssets;
using UnityEngine;

public class KillZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            AIController enemy = GetComponentInParent<AIController>();

            if (enemy != null && enemy.IsChasing)
            {
                other.GetComponent<FirstPersonController>().Die(enemy.transform);
            }
        }
    }
}
