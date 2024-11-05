using Unity.VisualScripting;
using UnityEngine;

public class ThrowWeapon : MonoBehaviour
{
    [SerializeField] private float standardWeaponThrowForce = 10;

    public bool isHeld = false;
    public bool isThrown = false;

    private int pickupRange = 2;
    private Vector3 offset = new Vector3(0.46f, 0.7f, 1.86f);
    GameObject activeWeapon;
    public GameObject heldWeapon;

    RaycastHit hit;

    // Start is called before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        activeWeapon = GameObject.Find("Active Weapon");
    }

    // Update is called once per frame
    void Update()
    {
        PickupWeapon();

        // If there is a weapon held
        if (heldWeapon != null)
        {
            heldWeapon.GetComponent<Collider>().enabled = false;

            if (Input.GetMouseButtonDown(0))
            {
                ThrowHeldWeapon();
            }
        }
    }

    private void PickupWeapon()
    {
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * pickupRange, Color.red);
        bool pickupRayHit = Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, pickupRange, ~0, QueryTriggerInteraction.Collide);

        if (pickupRayHit && hit.collider.CompareTag("Throwable Pickup") && Input.GetKeyDown(KeyCode.E))
        {
            if (hit.collider.name.StartsWith("Rock"))
            {
                HoldWeapon();
            }

            if (hit.collider.name.StartsWith("Acid Flask"))
            {
                HoldWeapon();
            }
        }
    }

    public void ThrowHeldWeapon()
    {
        isThrown = true;

        heldWeapon.GetComponent<Collider>().enabled = true;
        // Detach the held weapon from the parent
        heldWeapon.transform.SetParent(null);

        // Get the Rigidbody component and apply force
        Rigidbody rb = heldWeapon.GetComponent<Rigidbody>();
        rb.useGravity = true;
        rb.isKinematic = false;

        // Apply force in the direction the camera is facing
        rb.AddForce(Camera.main.transform.forward * standardWeaponThrowForce, ForceMode.Impulse);

        // Apply random torque
        Vector3 randomTorque = new Vector3(Random.Range(0.01f, 0.3f), Random.Range(0.01f, 0.3f), Random.Range(0.01f, 0.3f));
        rb.AddTorque(randomTorque, ForceMode.Impulse);

        // Clear heldWeapon reference since it’s thrown
        heldWeapon = null;
    }

    void HoldWeapon()
    {
        // Pick up the rock and make it a child of activeWeapon
        heldWeapon = hit.collider.gameObject;
        heldWeapon.GetComponent<Rigidbody>().useGravity = false;
        heldWeapon.GetComponent<Rigidbody>().isKinematic = false;
        heldWeapon.transform.SetParent(activeWeapon.transform);
        heldWeapon.transform.localPosition = offset;
    }
}