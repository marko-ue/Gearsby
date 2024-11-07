using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Week6ManageNPC : MonoBehaviour
{
    Animator anim;
    Ray ray;
    RaycastHit hit;
    AnimatorStateInfo info;
    GameObject target;
    string objectInSight;
    Vector3 direction;
    bool isInTheFieldOfView;
    bool noObjectBetweenNPCAndPlayer = false;
    Vector3 rightAngle, leftAngle;
    float distance;


    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        target = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        info = anim.GetCurrentAnimatorStateInfo(0);
        // if (Input.GetKeyDown (KeyCode.I))
        // {
        //     anim.SetBool("canSeePlayer", true);
        // }
        // if (Input.GetKeyDown (KeyCode.J))
        // {
        //     anim.SetBool("canSeePlayer", false);
        // }

        //Look();
        LookDotProduct();

        //Listen();
        Smell();

        if ((info.IsName("IDLE")))
        {
            //print("We are in an IDLE state");
            GetComponent<NavMeshAgent>().isStopped = true;
        }
        else if ((info.IsName("FOLLOW_PLAYER")))
        {
            //print("Following Player");
            GetComponent<NavMeshAgent>().isStopped = false;
            GetComponent<NavMeshAgent>().SetDestination(target.transform.position);
            //print("We are in the FOLLOW_PLAYER state");

            if (Vector3.Distance(transform.position, target.transform.position) < 2.3)
            {
                anim.SetBool("closeToPlayer", true);
            }
        }
        else if (info.IsName("ATTACK_PLAYER"))
        {
            GetComponent<NavMeshAgent>().isStopped = true;
            if (info.normalizedTime%1.0 >= .98)
            {
                target.GetComponent<ManagePlayerHealth>().DecreaseHealth();
            }
            if (Vector3.Distance(transform.position, target.transform.position) > 2.3)
            {
                anim.SetBool("closeToPlayer", false);
            }
        }
    }

    public void SetGotHitParameter()
    {
        anim.SetTrigger("gotHit");

    }

    public void SetLowHealthParameter(bool newValue)
    {
        anim.SetBool("lowHealth", newValue);
        
    }

    void Listen()
    {
        distance = Vector3.Distance(transform.position, target.transform.position);
        if (distance < 3)
        {
            anim.SetBool("canHearPlayer", true);
        }
        else
        {
            anim.SetBool("canHearPlayer", false);
        }
    }

    void Smell()
    {
        //print ("Smelling....");
        GameObject[] allBCs = GameObject.FindGameObjectsWithTag ("BC");
        float minDistance = 2;
        bool detectedBC = false;
        for (int i = 0; i < allBCs.Length; i ++)
        {
            if (Vector3.Distance(gameObject.transform.position, allBCs[i].transform.position) < minDistance)
            {
                detectedBC = true; break;
            }
        }
        if (detectedBC)
            anim.SetBool ("canSmellPlayer", true);
        else
            anim.SetBool ("canSmellPlayer", false);
    }

    void LookDotProduct()
    {
        
        
        
        //COS 60  degrees = 0.5
        direction = (GameObject. Find("Player").transform.position - transform.position).normalized;        
        isInTheFieldOfView = (Vector3.Dot(transform.forward.normalized, direction) > 0.5);
        
        Debug.DrawRay(transform.position, direction *  100, Color.green);
        Debug.DrawRay(transform.position, transform.forward * 100, Color.blue);    
        
        rightAngle = Quaternion.Euler(0, 60, 0) * transform.forward;
        Debug.DrawRay(transform.position, rightAngle * 100, Color.red);
        
        leftAngle = Quaternion.Euler(0, -60, 0) * transform.forward;  
        Debug.DrawRay(transform.position, leftAngle * 100, Color.red);
        
        
        if (Physics.Raycast(transform.position, direction * 100, out hit))
        {
            if (hit.collider.gameObject.tag == "Player") noObjectBetweenNPCAndPlayer = true;
            else noObjectBetweenNPCAndPlayer = false;
        }
        if (isInTheFieldOfView && noObjectBetweenNPCAndPlayer)
        {
            anim.SetBool ("canSeePlayer", true);
            GetComponent<NavMeshAgent>().isStopped = false;
            
            //transform.LookAt(GameObject.Find("playerMiddle").transform);

        }
        else anim.SetBool ("canSeePlayer", false);

    }
}
