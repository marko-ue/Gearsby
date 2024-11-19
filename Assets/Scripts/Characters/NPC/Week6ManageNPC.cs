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
                Debug.Log("Following breadcrumb: " + breadcrumb.name);
                FollowTarget(breadcrumb);
                if (Vector3.Distance(transform.position, breadcrumb.transform.position) < 2.3)
                {
                    anim.SetBool("closeToPlayer", true);
                    breadcrumb = null;
                }
            }
            else if (playerBreadcrumb != null && Vector3.Distance(transform.position, playerBreadcrumb.transform.position) < 5.0f)
            {
                Debug.Log("Following player breadcrumb: " + playerBreadcrumb.name);
                FollowTarget(playerBreadcrumb);
                if (Vector3.Distance(transform.position, playerBreadcrumb.transform.position) < 2.3)
                {
                    anim.SetBool("closeToPlayer", true);
                    playerBreadcrumb = null;
                }
            }
            else
            {
                anim.SetBool("closeToPlayer", false);
                GetComponent<NavMeshAgent>().isStopped = true;
            }
        }
        else if (info.IsName("ATTACK_PLAYER"))
        {
            // Ensure AI only attacks when close enough to the player and can see the player
            if (anim.GetBool("canSeePlayer") && Vector3.Distance(transform.position, player.transform.position) <= 2.3)
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
            else
            {
                anim.SetBool("closeToPlayer", false);
                GetComponent<NavMeshAgent>().isStopped = true;
            }
        }
        else
        {
            anim.SetBool("closeToPlayer", false);
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
        float minDistance = 5.0f; // Adjusted for larger detection range
        float minPlayerBCDistance = 5.0f; // Adjusted for larger detection range
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
                    playerBreadcrumb = allBCs[i];
                }
            }
            else if (distanceToBC < minDistance)
            {
                minDistance = distanceToBC;
                breadcrumb = allBCs[i];
            }
        }

        if (breadcrumb != null)
        {
            Debug.Log("Detected breadcrumb: " + breadcrumb.name + " at distance: " + minDistance);
            anim.SetBool("canSmellPlayer", true);
        }
        else if (playerBreadcrumb != null && Vector3.Distance(transform.position, playerBreadcrumb.transform.position) < 5.0f)
        {
            Debug.Log("Detected player breadcrumb: " + playerBreadcrumb.name + " at distance: " + minPlayerBCDistance);
            anim.SetBool("canSmellPlayer", true);
        }
        else
        {
            anim.SetBool("canSmellPlayer", false);
            target = null;
        }
    }


    void LookDotProduct()
    {
        if (player != null)
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
                GetComponent<NavMeshAgent>().isStopped = true;
            }
        }
    }

}
