using System.Collections;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class EnemyAi : MonoBehaviour
{
    [Header("Enemy Stats")]
    [SerializeField] float attackCooldown = 1f;
    [SerializeField] float speed = 10f;
    [SerializeField] float attackRange = 5f;
    [SerializeField] float damageToDeal = 10f;
    bool onCooldown = false;

    public enum State { Idle, Patrol, Active, Attack}
    public State currentState = State.Patrol;

    [SerializeField] float detectionRange = 20f;
    [SerializeField] Transform player;
    [SerializeField] Transform[] patrolPoints;

    PlayerHealth phScript;
    

    private Transform currentPatrolPoint;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetRandomPatrol();
        currentState = State.Patrol;
        phScript = GameObject.Find("Player").GetComponent<PlayerHealth>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case State.Idle:
                IdleBehavior();
                break;
            case State.Patrol:
                PatrolBehavior();
                break;
            case State.Active:
                ActiveBehavior();
                break;
            case State.Attack:
                AttackBehavior();
                break;
        }
        StateDetection();

    }

    void IdleBehavior()
    {
        //todo play idle animation
    }
    //Patrol behavior function cn
    void PatrolBehavior()
    {
        // Move towards the current patrol point
        transform.position = Vector3.MoveTowards(transform.position, currentPatrolPoint.position, speed * Time.deltaTime);

        // Check if the enemy reached the patrol point
        if (Vector3.Distance(transform.position, currentPatrolPoint.position) < 1f)
        {
            // Set a new random patrol point between the two
            SetRandomPatrol();
        }
    }
    void ActiveBehavior()
    {
        transform.position = Vector3.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
    }
    void StateDetection()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Detects player in a set range and triggers active state
        if (distanceToPlayer < attackRange)
        {
            currentState = State.Attack; // Switch to Attack state if within attack range
        }
        else if (distanceToPlayer < detectionRange)
        {
            currentState = State.Active; // Switch to Active state if within detection range
        }
        else
        {
            currentState = State.Patrol; // Return to patrol if player is out of detection range
        }
    }
    void SetRandomPatrol()
    {
        currentPatrolPoint = patrolPoints[Random.Range(0, patrolPoints.Length)];
    }
    void AttackBehavior()
    {
        if (!onCooldown)
        {
            StartCoroutine(EnemyAttack());
            Debug.Log(phScript.health);
        }

    }

    IEnumerator EnemyAttack()
    {
        if (phScript.health != phScript.minHealth)
        {
            onCooldown = true;
            phScript.health -= damageToDeal;
            yield return new WaitForSeconds(attackCooldown);
            onCooldown = false;
        }
    }
}