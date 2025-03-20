using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class AlertSystem : MonoBehaviour
{
    [SerializeField] private Image alertImage;
    [SerializeField] private Color patrolColor = Color.green;
    [SerializeField] private Color searchingColor = Color.yellow;
    [SerializeField] private Color chasingColor = Color.red;

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
            alertImage.color = patrolColor;
            return;
        }

        bool isChasing = enemies.Any(enemy => enemy.IsChasing);
        bool isSearching = enemies.Any(enemy => enemy.IsSearching);

        if (isChasing)
            alertImage.color = chasingColor;
        else if (isSearching)
            alertImage.color = searchingColor;
        else
            alertImage.color = patrolColor;
    }
}
