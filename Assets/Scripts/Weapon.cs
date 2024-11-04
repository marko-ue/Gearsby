using UnityEngine;

public class Weapon : MonoBehaviour
{
    private int pickupRange = 2;
    private Vector3 offset = new Vector3(0.46f, 0.7f, 1.86f);

    GameObject activeWeapon;
    GameObject heldWeapon;
    [SerializeField] ThrowableWeaponSO throwableWeaponSO;

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
        if (heldWeapon != null && Input.GetMouseButtonDown(0))
        {
            ThrowWeapon();
        }
    }

    private void PickupWeapon()
    {
        RaycastHit hit;
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * pickupRange, Color.red);
        bool pickupRayHit = Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, pickupRange, ~0, QueryTriggerInteraction.Collide);

        if (pickupRayHit && hit.collider.CompareTag("Throwable Pickup") && Input.GetKeyDown(KeyCode.E))
        {
            if (hit.collider.name.Contains("Rock"))
            {
                // Pick up the rock and make it a child of activeWeapon
                heldWeapon = hit.collider.gameObject;
                heldWeapon.GetComponent<Rigidbody>().useGravity = false;
                heldWeapon.transform.SetParent(activeWeapon.transform);
                heldWeapon.transform.localPosition = offset;
            }
        }
    }

    private void ThrowWeapon()
    {
        // Detach the held weapon from the parent
        heldWeapon.transform.SetParent(null);

        // Get the Rigidbody component and apply force
        Rigidbody rb = heldWeapon.GetComponent<Rigidbody>();
        rb.useGravity = true;
        rb.isKinematic = false;

        // Apply force in the direction the camera is facing
        rb.AddForce(Camera.main.transform.forward * throwableWeaponSO.ThrowForce, ForceMode.Impulse);

        // Clear heldWeapon reference since it’s thrown
        heldWeapon = null;
    }
}
