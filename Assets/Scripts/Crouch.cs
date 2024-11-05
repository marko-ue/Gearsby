using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;

public class Crouch : MonoBehaviour
{
    StarterAssetsInputs starterAssetsInputs;
    private Vector3 crouchScale = new Vector3(1, 0.5f, 1);
    private Vector3 normalScale = new Vector3(1, 1, 1);
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake() 
    {
        starterAssetsInputs = GetComponentInChildren<StarterAssetsInputs>();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       Crouching(); 
    }
    void Crouching()
    {
        if (starterAssetsInputs.crouch)
        {
            transform.localScale = crouchScale;
            
        }
        else if(!starterAssetsInputs.crouch)
        {
            transform.localScale = normalScale;
        }
    }
}
