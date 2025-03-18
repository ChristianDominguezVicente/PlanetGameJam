using UnityEngine;
using UnityEngine.AI;

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
    [SerializeField] private float chaseTime = 3f;

    private NavMeshAgent agent;
    private Animator animator;
    private float chaseTimer = 0f;
    private bool isChasing = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        PatrolToNextPoint();
    }

    void Update()
    {
        if (CanSeePlayer())
        {
            isChasing = true;
            chaseTimer = chaseTime;
            agent.destination = player.position;
        }
        else if (isChasing)
        {
            chaseTimer -= Time.deltaTime;
            if (chaseTimer <= 0f)
            {
                isChasing = false;
                PatrolToNextPoint();
            }
        }

        if (!isChasing && !agent.pathPending && agent.remainingDistance < 0.5f)
        {
            PatrolToNextPoint();
        }

        animator.SetFloat("Speed", agent.velocity.magnitude);
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

        Debug.DrawRay(rayOrigin, directionToPlayer * visionRange, Color.green, 1f);

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
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, visionRange);

        Vector3 rayOrigin = transform.position + Vector3.up * 1.2f;
        Vector3 leftLimit = Quaternion.Euler(0, -fieldOfView / 2, 0) * transform.forward;
        Vector3 rightLimit = Quaternion.Euler(0, fieldOfView / 2, 0) * transform.forward;

        Gizmos.color = Color.red;
        Gizmos.DrawRay(rayOrigin, leftLimit * visionRange);
        Gizmos.DrawRay(rayOrigin, rightLimit * visionRange);
    }
}