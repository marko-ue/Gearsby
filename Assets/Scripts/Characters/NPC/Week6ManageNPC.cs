using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Week6ManageNPC : MonoBehaviour
{
    Animator anim;
    RaycastHit hit;
    AnimatorStateInfo info;
    GameObject target;
    GameObject player;
    string objectInSight;
    Vector3 direction;
    bool isInTheFieldOfView;
    bool noObjectBetweenNPCAndPlayer = false;
    Vector3 rightAngle, leftAngle;
    float distance;

    GameObject breadcrumb; // Reference to the closest breadcrumb
    GameObject playerBreadcrumb; // Reference to the closest player breadcrumb

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        player = GameObject.Find("Player");
        target = player;
        breadcrumb = null; // Initialize without any breadcrumb target
        playerBreadcrumb = null; // Initialize without any player breadcrumb target
    }

    // Update is called once per frame
    void Update()
    {
        info = anim.GetCurrentAnimatorStateInfo(0);
        LookDotProduct();
        Smell();

        if (info.IsName("IDLE"))
        {
            GetComponent<NavMeshAgent>().isStopped = true;
        }
        else if (info.IsName("FOLLOW_PLAYER"))
        {
            if (breadcrumb != null)
            {
                FollowTarget(breadcrumb);
                if (Vector3.Distance(transform.position, breadcrumb.transform.position) < 2.3)
                {
                    anim.SetBool("closeToPlayer", true);
                    breadcrumb = null; // Reset target after reaching a breadcrumb
                }
            }
            else if (playerBreadcrumb != null && Vector3.Distance(transform.position, playerBreadcrumb.transform.position) < 5.0f) // Check if player breadcrumb is within range
            {
                FollowTarget(playerBreadcrumb);
                if (Vector3.Distance(transform.position, playerBreadcrumb.transform.position) < 2.3)
                {
                    anim.SetBool("closeToPlayer", true);
                    playerBreadcrumb = null; // Reset target after reaching a player breadcrumb
                }
            }
            else
            {
                FollowTarget(player);
                if (Vector3.Distance(transform.position, player.transform.position) < 2.3)
                {
                    anim.SetBool("closeToPlayer", true);
                }
            }
        }
        else if (info.IsName("ATTACK_PLAYER"))
        {
            GetComponent<NavMeshAgent>().isStopped = true;
            if (info.normalizedTime % 1.0 >= .98)
            {
                player.GetComponent<ManagePlayerHealth>().DecreaseHealth();
            }
            if (Vector3.Distance(transform.position, player.transform.position) > 2.3)
            {
                anim.SetBool("closeToPlayer", false);
            }
        }
    }

    void FollowTarget(GameObject target)
    {
        GetComponent<NavMeshAgent>().isStopped = false;
        GetComponent<NavMeshAgent>().SetDestination(target.transform.position);
    }

    public void SetGotHitParameter()
    {
        anim.SetTrigger("gotHit");
    }

    public void SetLowHealthParameter(bool newValue)
    {
        anim.SetBool("lowHealth", newValue);
    }

    void Smell()
    {
        GameObject[] allBCs = GameObject.FindGameObjectsWithTag("BC");
        float minDistance = Mathf.Infinity;
        float minPlayerBCDistance = Mathf.Infinity;
        breadcrumb = null;
        playerBreadcrumb = null;

        for (int i = 0; i < allBCs.Length; i++)
        {
            float distanceToBC = Vector3.Distance(transform.position, allBCs[i].transform.position);
            if (allBCs[i].name.Contains("PlayerBC"))
            {
                if (distanceToBC < minPlayerBCDistance)
                {
                    minPlayerBCDistance = distanceToBC;
                    playerBreadcrumb = allBCs[i]; // Set the closest player breadcrumb as the target
                }
            }
            else if (distanceToBC < minDistance)
            {
                minDistance = distanceToBC;
                breadcrumb = allBCs[i]; // Set the closest breadcrumb as the target
            }
        }

        if (breadcrumb != null)
        {
            anim.SetBool("canSmellPlayer", true);
            anim.SetBool("isFollowingBreadcrumb", true); // Set the state to follow breadcrumb
        }
        else if (playerBreadcrumb != null && Vector3.Distance(transform.position, playerBreadcrumb.transform.position) < 5.0f)
        {
            anim.SetBool("canSmellPlayer", true);
            anim.SetBool("isFollowingBreadcrumb", true); // Set the state to follow player breadcrumb within range
        }
        else
        {
            anim.SetBool("canSmellPlayer", false);
            anim.SetBool("isFollowingBreadcrumb", false); // Set the state to not follow breadcrumb
            target = player; // Ensure the player is the target when no breadcrumbs are detected
        }
    }

    void LookDotProduct()
    {
        direction = (player.transform.position - transform.position).normalized;
        isInTheFieldOfView = (Vector3.Dot(transform.forward.normalized, direction) > 0.5);
        Debug.DrawRay(transform.position, direction * 100, Color.green);
        Debug.DrawRay(transform.position, transform.forward * 100, Color.blue);
        rightAngle = Quaternion.Euler(0, 60, 0) * transform.forward;
        Debug.DrawRay(transform.position, rightAngle * 100, Color.red);
        leftAngle = Quaternion.Euler(0, -60, 0) * transform.forward;
        Debug.DrawRay(transform.position, leftAngle * 100, Color.red);
        if (Physics.Raycast(transform.position, direction * 100, out hit))
        {
            if (hit.collider.gameObject.tag == "Player")
                noObjectBetweenNPCAndPlayer = true;
            else
                noObjectBetweenNPCAndPlayer = false;
        }
        if (isInTheFieldOfView && noObjectBetweenNPCAndPlayer)
        {
            anim.SetBool("canSeePlayer", true);
            GetComponent<NavMeshAgent>().isStopped = false;
        }
        else
        {
            anim.SetBool("canSeePlayer", false);
        }
    }
}
