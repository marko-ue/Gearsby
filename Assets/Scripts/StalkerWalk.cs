using UnityEngine;
using UnityEngine.AI;

public class StalkerWalk : MonoBehaviour
{
    [Header("References")]
    public Animator animator; // Reference to the Animator component
    public NavMeshAgent navMeshAgent; // Reference to the NavMeshAgent component

    [Header("Settings")]
    public float rotationSpeed = 5f; // Speed of rotation
    public float movementThreshold = 0.1f; // Threshold to determine if AI is walking

    private Vector3 previousPosition; // To track the AI's previous position

    void Start()
    {
        // Initialize the previous position
        previousPosition = transform.position;

        // If components are not assigned, try to auto-detect them
        if (animator == null)
            animator = GetComponent<Animator>();

        if (navMeshAgent == null)
            navMeshAgent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        // Calculate movement speed by comparing current and previous positions
        Vector3 movement = transform.position - previousPosition;
        float speed = movement.magnitude / Time.deltaTime;
        previousPosition = transform.position;

        // Determine if the AI is walking
        bool isWalking = speed > movementThreshold;
        animator.SetBool("isWalking", isWalking);

        // Rotate AI towards the movement direction
        if (navMeshAgent.velocity.sqrMagnitude > 0.01f) // Ensure the AI is actually moving
        {
            Vector3 direction = navMeshAgent.velocity.normalized;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
        }
    }
}
