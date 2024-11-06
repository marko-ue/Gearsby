using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManageTargetHealth : MonoBehaviour
{
    int health = 100;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void GotHit()
    {
        print("Got hit by a bullet!");
        health -= 50;
        if (health <= 0)
        {
            GetComponent<Week6ManageNPC>().SetLowHealthParameter(true);
            Destroy(gameObject, 4);
        }

        GetComponent<Week6ManageNPC>().SetGotHitParameter();
    }

    public void GotHitByGrenade()
    {
        print("Hit by Grenade");
        health = 0;
        GetComponent<Week6ManageNPC>().SetGotHitParameter();
        GetComponent<Week6ManageNPC>().SetLowHealthParameter(true);
        Destroy (gameObject, 4);
    }

}
