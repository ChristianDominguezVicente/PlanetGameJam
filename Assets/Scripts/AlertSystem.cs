using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class AlertSystem : MonoBehaviour
{
    [SerializeField] private GameObject lowAlert;
    [SerializeField] private GameObject midAlert;
    [SerializeField] private GameObject highAlert;

    private AIController[] enemies;

    private void Start()
    {
        UpdateEnemyList();
        UpdateAlertLevel();
    }

    private void Update()
    {
        UpdateAlertLevel();
    }

    private void UpdateEnemyList()
    {
        GameObject[] enemyObjects = GameObject.FindGameObjectsWithTag("Enemy");
        enemies = enemyObjects.Select(e => e.GetComponent<AIController>()).Where(e => e != null).ToArray();
    }

    private void UpdateAlertLevel()
    {
        if (enemies.Length == 0)
        {
            lowAlert.SetActive(true);
            midAlert.SetActive(false);
            highAlert.SetActive(false);
            return;
        }

        bool isChasing = enemies.Any(enemy => enemy.IsChasing);
        bool isSearching = enemies.Any(enemy => enemy.IsSearching);

        if (isChasing)
        {
            lowAlert.SetActive(false);
            midAlert.SetActive(false);
            highAlert.SetActive(true);
        }

        else if (isSearching)
        {
            lowAlert.SetActive(false);
            midAlert.SetActive(true);
            highAlert.SetActive(false);
        }
            

        else
        {
            lowAlert.SetActive(true);
            midAlert.SetActive(false);
            highAlert.SetActive(false);
        }
            
    }
}
