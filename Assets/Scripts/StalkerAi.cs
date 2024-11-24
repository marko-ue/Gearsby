using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class StalkerAI : MonoBehaviour
{
    public enum StalkerState { Idle, Search, Listen, Chase }
    private StalkerState currentState = StalkerState.Idle;

    [Header("Detection Settings")]
    public Transform player;
    public LayerMask obstacleLayer;
    public LayerMask playerLayer;
    public float sightRange = 20f;
    public float sightAngle = 90f;
    public float hearingRange = 15f;
    public float noiseThreshold = 0.1f;
    public float timeToHearPlayer = 2f;
    private float hearingTimer = 0f;

    [Header("AI Settings")]
    public float eyeHeight = 1.6f;
    public float searchRadius = 10f;
    public float searchPointReachedDistance = 1f;
    public float maxSearchDuration = 30f;
    private float currentSearchDuration = 0f;

    private NavMeshAgent agent;
    private bool playerInSight = false;
    private bool playerInHearing = false;
    private bool isSearching = false;

    private Vector3 lastKnownPosition;
    private Vector3 previousPlayerPosition;
    private CharacterController playerController;
    private Vector3 currentSearchCenter;

    [Header("Communication Settings")]
    public float alertRange = 30f;
    public float alertCooldown = 5f;
    private float alertTimer = 0f;

    [Header("Search Settings")]
    public float searchPointRadius = 15f;
    public int maxSearchPoints = 5;
    public float searchWaitTime = 1f;
    private List<Vector3> searchPoints = new List<Vector3>();
    private int currentSearchPointIndex = 0;
    private bool isWaitingAtSearchPoint = false;
    private float searchWaitTimer = 0f;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        playerController = player.GetComponent<CharacterController>();
        AIManager.Instance.RegisterStalkerAI(this);
        
        playerLayer = LayerMask.GetMask("Player");
        obstacleLayer = LayerMask.GetMask("Default", "Climbable");

        if (playerController == null)
        {
            Debug.LogError("Player is missing a CharacterController component!");
        }

        previousPlayerPosition = player.position;
        currentSearchCenter = transform.position;
        GenerateSearchPoints();
        TransitionToState(StalkerState.Search);
    }

    private void Update()
    {
        DetectPlayer();
        StateMachine();
        
        if (currentState == StalkerState.Search)
        {
            currentSearchDuration += Time.deltaTime;
            if (currentSearchDuration >= maxSearchDuration)
            {
                RegenerateSearchPoints();
            }
        }
    }

    private void DetectPlayer()
    {
        bool wasPlayerInSight = playerInSight;
        
        playerInSight = false;
        playerInHearing = false;

        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

        // Visual Detection
        if (distanceToPlayer <= sightRange && angleToPlayer <= sightAngle / 2f)
        {
            Vector3 rayOrigin = transform.position + Vector3.up * eyeHeight;
            Vector3 playerEyePos = player.position + Vector3.up * eyeHeight;
            directionToPlayer = (playerEyePos - rayOrigin).normalized;

            if (Physics.Raycast(rayOrigin, directionToPlayer, out RaycastHit hit, distanceToPlayer))
            {
                if (hit.transform == player)
                {
                    playerInSight = true;
                    lastKnownPosition = player.position;
                    currentSearchCenter = lastKnownPosition;
                    currentSearchDuration = 0f;
                    
                    if (!wasPlayerInSight)
                    {
                        AIManager.Instance.OnStalkerSpottedPlayer(player.position);
                    }
                    
                    Debug.DrawLine(rayOrigin, hit.point, Color.green);
                }
                else
                {
                    if (wasPlayerInSight)
                    {
                        InitiateSearch();
                    }
                    Debug.DrawLine(rayOrigin, hit.point, Color.red);
                }
            }
        }
        else if (wasPlayerInSight)
        {
            InitiateSearch();
        }

        // Hearing Detection
        if (distanceToPlayer <= hearingRange)
        {
            float hearingMultiplier = 1f;
            
            if (Physics.Raycast(transform.position, directionToPlayer, out RaycastHit hit, distanceToPlayer, obstacleLayer))
            {
                hearingMultiplier = 0.5f;
            }

            float distanceFactor = 1f - (distanceToPlayer / hearingRange);
            hearingMultiplier *= distanceFactor;

            if (IsPlayerMakingNoise())
            {
                hearingTimer += Time.deltaTime * hearingMultiplier;
                
                if (hearingTimer >= timeToHearPlayer)
                {
                    playerInHearing = true;
                    lastKnownPosition = player.position;
                    currentSearchCenter = lastKnownPosition;
                    hearingTimer = timeToHearPlayer;
                }
            }
            else
            {
                hearingTimer = Mathf.Max(0, hearingTimer - Time.deltaTime);
            }
        }
        else
        {
            hearingTimer = 0f;
        }

        if (playerInSight || playerInHearing)
        {
            TransitionToState(StalkerState.Chase);
            
            if (alertTimer <= 0)
            {
                AlertNearbyAIs();
                alertTimer = alertCooldown;
            }
        }

        if (alertTimer > 0)
        {
            alertTimer -= Time.deltaTime;
        }
    }

    private void InitiateSearch()
    {
        currentSearchPointIndex = 0;
        isWaitingAtSearchPoint = false;
        currentSearchDuration = 0f;
        GenerateSearchPoints();
        TransitionToState(StalkerState.Search);
    }

    private bool IsPlayerMakingNoise()
    {
        if (playerController == null) return false;

        float movementMagnitude = (player.position - previousPlayerPosition).magnitude / Time.deltaTime;
        previousPlayerPosition = player.position;

        return movementMagnitude > noiseThreshold;
    }

    private void SearchBehavior()
    {
        if (playerInSight || playerInHearing)
        {
            TransitionToState(StalkerState.Chase);
            return;
        }

        if (isWaitingAtSearchPoint)
        {
            searchWaitTimer += Time.deltaTime;
            if (searchWaitTimer >= searchWaitTime)
            {
                isWaitingAtSearchPoint = false;
                searchWaitTimer = 0f;
                MoveToNextSearchPoint();
            }
            return;
        }

        if (!agent.pathPending && agent.remainingDistance < searchPointReachedDistance)
        {
            isWaitingAtSearchPoint = true;
            searchWaitTimer = 0f;
        }
    }

    private void GenerateSearchPoints()
    {
        searchPoints.Clear();
        for (int i = 0; i < maxSearchPoints; i++)
        {
            Vector3 randomPoint = currentSearchCenter + Random.insideUnitSphere * searchPointRadius;
            randomPoint.y = currentSearchCenter.y;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, searchPointRadius, NavMesh.AllAreas))
            {
                searchPoints.Add(hit.position);
            }
        }
        currentSearchPointIndex = 0;
        MoveToNextSearchPoint();
    }

    private void MoveToNextSearchPoint()
    {
        if (searchPoints.Count == 0)
        {
            RegenerateSearchPoints();
            return;
        }

        currentSearchPointIndex = (currentSearchPointIndex + 1) % searchPoints.Count;
        agent.SetDestination(searchPoints[currentSearchPointIndex]);
    }

    private void RegenerateSearchPoints()
    {
        currentSearchCenter += Random.insideUnitSphere * searchRadius;
        currentSearchDuration = 0f;
        GenerateSearchPoints();
    }

    private void ListenBehavior()
    {
        agent.SetDestination(lastKnownPosition);

        if (!agent.pathPending && agent.remainingDistance < searchPointReachedDistance)
        {
            TransitionToState(StalkerState.Search);
        }
    }

    private void ChaseBehavior()
    {
        if (playerInSight || playerInHearing)
        {
            agent.SetDestination(player.position);
            lastKnownPosition = player.position;
            currentSearchCenter = lastKnownPosition;
        }
        else
        {
            InitiateSearch();
        }
    }

    private void IdleBehavior()
    {
        if (!isSearching)
        {
            TransitionToState(StalkerState.Search);
        }
    }

    private void StateMachine()
    {
        switch (currentState)
        {
            case StalkerState.Idle:
                IdleBehavior();
                break;
            case StalkerState.Search:
                SearchBehavior();
                break;
            case StalkerState.Listen:
                ListenBehavior();
                break;
            case StalkerState.Chase:
                ChaseBehavior();
                break;
        }
    }

    private void TransitionToState(StalkerState newState)
    {
        currentState = newState;

        switch (newState)
        {
            case StalkerState.Idle:
                agent.isStopped = true;
                break;
            case StalkerState.Search:
                agent.isStopped = false;
                isWaitingAtSearchPoint = false;
                searchWaitTimer = 0f;
                isSearching = true;
                break;
            case StalkerState.Listen:
            case StalkerState.Chase:
                agent.isStopped = false;
                break;
        }
    }

    private void OnDestroy()
    {
        AIManager.Instance.UnregisterStalkerAI(this);
    }

    private void AlertNearbyAIs()
    {
        AIManager.Instance.AlertAllNearbyAIs(player.position, transform.position, alertRange);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue * 0.5f;
        Gizmos.DrawWireSphere(transform.position, hearingRange);

        Gizmos.color = Color.red * 0.5f;
        Gizmos.DrawWireSphere(transform.position, sightRange);

        if (currentState == StalkerState.Search)
        {
            Gizmos.color = Color.yellow * 0.3f;
            Gizmos.DrawWireSphere(currentSearchCenter, searchPointRadius);

            // Draw search points
            Gizmos.color = Color.cyan;
            foreach (Vector3 point in searchPoints)
            {
                Gizmos.DrawSphere(point, 0.5f);
            }

            // Highlight current search point
            if (searchPoints.Count > 0 && currentSearchPointIndex < searchPoints.Count)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(searchPoints[currentSearchPointIndex], 0.7f);
            }
        }

        Vector3 rightDir = Quaternion.Euler(0, sightAngle / 2f, 0) * transform.forward;
        Vector3 leftDir = Quaternion.Euler(0, -sightAngle / 2f, 0) * transform.forward;
        Debug.DrawRay(transform.position, rightDir * sightRange, Color.yellow);
        Debug.DrawRay(transform.position, leftDir * sightRange, Color.yellow);
    }
}