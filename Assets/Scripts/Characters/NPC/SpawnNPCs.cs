using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnNPCs : MonoBehaviour
{
    public GameObject NPC;
    float timer;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > 10){
            Instantiate(NPC, gameObject.transform);
            timer = 0;
        }
    }
}
