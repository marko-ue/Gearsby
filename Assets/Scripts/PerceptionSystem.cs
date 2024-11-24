using UnityEngine;
using UnityEngine.AI;

public class AIController : MonoBehaviour
{
    public enum AIState { Idle, Patrol, Investigate, Search, Chase, Combat }
    public AIState currentState = AIState.Idle;

    [Header("AI Settings")]
    public float eyeHeight = 1.6f;

    [Header("Detection Settings")]
    public Transform player;
    public LayerMask obstacleMask;
    public float sightRange = 25f;
    public float sightAngle = 90f;
    public float crouchSightModifier = 0.5f;

    [Header("Patrol Settings")]
    public Transform[] patrolPoints;
    private int patrolIndex = 0;

    [Header("Investigation Settings")]
    public float investigationDuration = 5f;
    private float investigationTimer;
    public float investigationReachThreshold = 1f;

    [Header("Search Settings")]
    public int searchPoints = 5;
    public float searchRadius = 10f;
    public float maxSearchDuration = 30f;
    private float currentSearchDuration = 0f;

    [Header("Combat Settings")]
    public float attackRange = 2.5f;
    public float attackCooldown = 2f;
    private float attackTimer;

    [Header("Alert Response Settings")]
    public float alertResponseRange = 30f;
    public float alertInvestigationDuration = 10f;
    private bool isRespondingToAlert = false;
    private Vector3 alertPosition;

    private NavMeshAgent agent;
    private bool playerInSight = false;
    private bool isDistracted = false;
    private Vector3 distractionPoint;
    private Vector3 lastKnownPosition;
    private bool isCrouched = false;
    private bool hasReachedInvestigationPoint = false;
    private float investigationPointReachedTimer = 0f;
    private float maxInvestigationPointWaitTime = 3f;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        TransitionToState(AIState.Patrol);
        obstacleMask = LayerMask.GetMask("Obstacle", "Default");
        AIManager.Instance.RegisterRegularAI(this);
    }

    private void OnDestroy()
    {
        AIManager.Instance.UnregisterRegularAI(this);
    }

    private void Update()
    {
        if (!isRespondingToAlert) DetectPlayer();
        StateMachine();

        if (currentState == AIState.Chase && playerInSight)
        {
            lastKnownPosition = player.position;
        }
    }

    private void DetectPlayer()
    {
        if (isDistracted || isRespondingToAlert) return;

        float modifiedSightRange = isCrouched ? sightRange * crouchSightModifier : sightRange;

        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

        bool previousPlayerInSight = playerInSight;
        playerInSight = false;

        if (distanceToPlayer <= modifiedSightRange && angleToPlayer <= sightAngle / 2f)
        {
            Vector3 rayOrigin = transform.position + Vector3.up * eyeHeight;
            Vector3 playerEyePos = player.position + Vector3.up * eyeHeight;
            directionToPlayer = (playerEyePos - rayOrigin).normalized;

            RaycastHit hit;
            if (Physics.Raycast(rayOrigin, directionToPlayer, out hit, distanceToPlayer))
            {
                if (hit.transform == player.transform)
                {
                    playerInSight = true;
                    lastKnownPosition = player.position;
                    if (currentState != AIState.Combat)
                    {
                        TransitionToState(AIState.Chase);
                    }
                }
            }

            Debug.DrawLine(rayOrigin, rayOrigin + directionToPlayer * distanceToPlayer, 
                playerInSight ? Color.green : Color.red);
        }

        if (previousPlayerInSight && !playerInSight && currentState != AIState.Search)
        {
            TransitionToState(AIState.Search);
        }
    }

    public void RespondToAlert(Vector3 playerPos, Vector3 alertPos, float alertRange)
    {
        if (currentState == AIState.Combat) return;

        if (Vector3.Distance(transform.position, alertPos) <= alertResponseRange)
        {
            isRespondingToAlert = true;
            alertPosition = playerPos;
            lastKnownPosition = playerPos;
            investigationTimer = 0f;
            hasReachedInvestigationPoint = false;
            TransitionToState(AIState.Investigate);
        }
    }

    public void Distract(Vector3 point)
    {
        isDistracted = true;
        distractionPoint = point;
        lastKnownPosition = point;
        investigationTimer = 0f;
        hasReachedInvestigationPoint = false;
        TransitionToState(AIState.Investigate);
    }

    private void StateMachine()
    {
        switch (currentState)
        {
            case AIState.Idle:
                IdleBehavior();
                break;
            case AIState.Patrol:
                PatrolBehavior();
                break;
            case AIState.Investigate:
                InvestigateBehavior();
                break;
            case AIState.Search:
                SearchBehavior();
                break;
            case AIState.Chase:
                ChaseBehavior();
                break;
            case AIState.Combat:
                CombatBehavior();
                break;
        }
    }

    private void IdleBehavior()
    {
        TransitionToState(AIState.Patrol);
    }

    private void PatrolBehavior()
    {
        if (patrolPoints.Length == 0) return;

        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            patrolIndex = (patrolIndex + 1) % patrolPoints.Length;
            agent.SetDestination(patrolPoints[patrolIndex].position);
        }

        if (playerInSight)
        {
            TransitionToState(AIState.Chase);
        }
    }

    private void InvestigateBehavior()
    {
        if (!hasReachedInvestigationPoint)
        {
            agent.SetDestination(lastKnownPosition);
            
            if (!agent.pathPending && agent.remainingDistance < investigationReachThreshold)
            {
                hasReachedInvestigationPoint = true;
                investigationPointReachedTimer = 0f;
            }
        }
        else
        {
            investigationPointReachedTimer += Time.deltaTime;
            investigationTimer += Time.deltaTime;

            transform.Rotate(0, 120f * Time.deltaTime, 0);

            if (!playerInSight)
            {
                if (investigationTimer >= investigationDuration || investigationPointReachedTimer >= maxInvestigationPointWaitTime)
                {
                    isDistracted = false;
                    isRespondingToAlert = false;
                    hasReachedInvestigationPoint = false;
                    TransitionToState(AIState.Patrol);
                }
            }
            else
            {
                TransitionToState(AIState.Chase);
            }
        }
    }

    private void SearchBehavior()
    {
        if (playerInSight)
        {
            TransitionToState(AIState.Chase);
            return;
        }

        currentSearchDuration += Time.deltaTime;
        if (currentSearchDuration >= maxSearchDuration || searchPoints <= 0)
        {
            TransitionToState(AIState.Patrol);
            return;
        }

        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            Vector3 randomPoint = lastKnownPosition + new Vector3(
                Random.Range(-searchRadius, searchRadius),
                0,
                Random.Range(-searchRadius, searchRadius)
            );

            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, searchRadius, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
                searchPoints--;
            }
            else
            {
                searchPoints = 0;
            }
        }
    }

    private void ChaseBehavior()
    {
        if (playerInSight)
        {
            agent.SetDestination(player.position);

            if (Vector3.Distance(transform.position, player.position) <= attackRange)
            {
                TransitionToState(AIState.Combat);
            }
        }
        else
        {
            TransitionToState(AIState.Search);
        }
    }

    private void CombatBehavior()
    {
        agent.isStopped = true;

        if (!playerInSight)
        {
            agent.isStopped = false;
            TransitionToState(AIState.Search);
            return;
        }

        if (attackTimer <= 0f)
        {
            Debug.Log("Attacking player!");
            attackTimer = attackCooldown;
        }
        else
        {
            attackTimer -= Time.deltaTime;
        }

        if (Vector3.Distance(transform.position, player.position) > attackRange)
        {
            agent.isStopped = false;
            TransitionToState(AIState.Chase);
        }
    }

    private void TransitionToState(AIState newState)
    {
        currentState = newState;

        switch (newState)
        {
            case AIState.Idle:
                agent.isStopped = true;
                break;
            case AIState.Patrol:
                agent.isStopped = false;
                searchPoints = 5;
                isDistracted = false;
                isRespondingToAlert = false;
                currentSearchDuration = 0f;
                break;
            case AIState.Chase:
                agent.isStopped = false;
                isDistracted = false;
                break;
            case AIState.Investigate:
                agent.isStopped = false;
                investigationTimer = 0f;
                hasReachedInvestigationPoint = false;
                break;
            case AIState.Search:
                agent.isStopped = false;
                searchPoints = 5;
                currentSearchDuration = 0f;
                break;
            case AIState.Combat:
                agent.isStopped = true;
                isDistracted = false;
                break;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, sightRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, alertResponseRange);
    }
}