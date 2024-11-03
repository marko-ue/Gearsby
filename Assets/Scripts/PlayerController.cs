using System.Runtime.InteropServices;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private int pickupRange = 100;

    [Header("Collectible Amount")]
    public int cogwheelCount = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        PickupRay();
    }

    private void PickupRay()
    {
        RaycastHit hit;
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * pickupRange, Color.red);
        bool pickupRayHit = Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, pickupRange, ~0, QueryTriggerInteraction.Collide);

        if (pickupRayHit && hit.collider.CompareTag("Pickup") && Input.GetKeyDown(KeyCode.E))
        {
            if (hit.collider.name.Contains("Cogwheel"))
            {
                cogwheelCount++;
                Debug.Log(cogwheelCount);
            }
        }
    }
}
