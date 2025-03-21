using System.Drawing;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class AIController : MonoBehaviour
{
    [Header("Patrulla")]
    [SerializeField] private Transform[] waypoints;
    private int currentWaypointIndex = 0;

    [Header("Persecución")]
    [SerializeField] private Transform player;
    [SerializeField] private float visionRange = 10f;
    [SerializeField] private float fieldOfView = 90f;
    [SerializeField] private LayerMask visionMask;
    [SerializeField] private float chaseTime = 10f;
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private float chaseSpeed = 5f;
    
    [Header("Luces")]
    [SerializeField] private GameObject vigilanceLight;
    [SerializeField] private GameObject cylinder;
    [SerializeField] private GameObject[] pointLights;
    private UnityEngine.Color greenLight = new UnityEngine.Color(0.494f, 0.980f, 0.667f);
    private UnityEngine.Color redLight = new UnityEngine.Color(1f, 0f, 0.082f);
    private UnityEngine.Color yellowLight = new UnityEngine.Color(1f, 1f, 0f);
    


    private NavMeshAgent agent;
    private Animator animator;
    private float chaseTimer = 0f;
    private bool isChasing = false;
    private bool playerLost = false;
    private bool isSearching = false;
    private int counter = 0;
    private bool isPlayerNearby = false;

    public bool IsChasing { get => isChasing; }
    public bool IsSearching { get => isSearching; }

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        agent.speed = patrolSpeed;
        animator.SetTrigger("StartMoving");
    }

    void Update()
    {
        if (CanSeePlayer())
        {

            StartChasing();
        }
        else if (isChasing)
        {
            UpdateChase();
        }
        else if (!isSearching)
        {
            Patrol();
        }

        animator.SetFloat("Speed", agent.velocity.magnitude);

        if(isPlayerNearby && Keyboard.current.eKey.wasPressedThisFrame)
        {
            DeactivateEnemy();
        }
    }

    private void StartChasing()
    {
        if (!isChasing)
        {
            isChasing = true;
            playerLost = false;
            chaseTimer = chaseTime;
            agent.speed = chaseSpeed;
        }

        agent.destination = player.position;

        //color luz
        ChangeLights(redLight);

    }

    private void UpdateChase()
    {
        chaseTimer -= Time.deltaTime;

        if (chaseTimer <= 0f)
        {
            playerLost = true;
            StartSearching();
        }

        if (!playerLost)
        {
            agent.destination = player.position;
        }
    }

    private void StartSearching()
    {
        isChasing = false;
        isSearching = true;
        agent.speed = 0;
        animator.SetBool("Vigilant", true);

        ChangeLights(yellowLight);
    }

    public void EndSearching()
    {
        counter++;
        if(counter >= 2)
        {
            isSearching = false;
            animator.SetBool("Vigilant", false);
            agent.speed = patrolSpeed;
            counter = 0;
        }
    }

    private void Patrol()
    {
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            animator.SetTrigger("StartMoving");
            PatrolToNextPoint();
        }

        //color luz
        ChangeLights(greenLight);
    }

    private void PatrolToNextPoint()
    {
        if (waypoints.Length == 0) return;

        agent.destination = waypoints[currentWaypointIndex].position;
        currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
    }

    private bool CanSeePlayer()
    {
        Vector3 rayOrigin = transform.position + Vector3.up * 0.5f;
        Vector3 directionToPlayer = (player.position - rayOrigin).normalized;
        float angle = Vector3.Angle(transform.forward, directionToPlayer);
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        Debug.DrawRay(rayOrigin, directionToPlayer * visionRange, UnityEngine.Color.green, 1f);

        if (angle < fieldOfView / 2 && distanceToPlayer < visionRange)
        {
            if (Physics.Raycast(rayOrigin, directionToPlayer, out RaycastHit hit, visionRange, visionMask))
            {
                if (hit.transform.root == player)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = UnityEngine.Color.yellow;
        Gizmos.DrawWireSphere(transform.position, visionRange);

        Vector3 rayOrigin = transform.position + Vector3.up * 1.2f;
        Vector3 leftLimit = Quaternion.Euler(0, -fieldOfView / 2, 0) * transform.forward;
        Vector3 rightLimit = Quaternion.Euler(0, fieldOfView / 2, 0) * transform.forward;

        Gizmos.color = UnityEngine.Color.red;
        Gizmos.DrawRay(rayOrigin, leftLimit * visionRange);
        Gizmos.DrawRay(rayOrigin, rightLimit * visionRange);
    }

    private void DeactivateEnemy()
    {
        if (isChasing) return;
        agent.isStopped = true;
        animator.SetTrigger("Dead");
    }

    private void End()
    {
        animator.SetTrigger("End");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
        }
    }
    
    private void ChangeLights(UnityEngine.Color color)
    {
        vigilanceLight.GetComponent<Light>().color = color;
        cylinder.GetComponent<Renderer>().material.color = color;
        for (int i = 0; i < pointLights.Length; i++)
        {
            pointLights[i].GetComponent<Light>().color = color;
        }

        
    }
    
}