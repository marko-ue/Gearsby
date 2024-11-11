using StarterAssets;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private int pickupRange = 2;
    private bool canMove = true;

    [Header("Stamina Stats")]
    [SerializeField] float sprintSpeed; // walk speed multiplied by sprint speed value
    [SerializeField] float stamina = 100f;
    [SerializeField] float staminaDeduct = 2f;
    [SerializeField] float staminaReturn = 3f;
    [SerializeField] float maxStamina = 100f;
    [SerializeField] float minStamina = 0f;
    [SerializeField] float returnDelay = 1f;
    [SerializeField] float returnTimer = 0f;

    [Header("Collectible Amount")]
    StarterAssetsInputs starterAssetsInputs;
    private CharacterController characterController;

    [Header("Door")]
    public float interactRange = 3f; // Range within which the player can interact
    public LayerMask openableLayer; // LayerMask for interactable objects

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        starterAssetsInputs = GetComponentInChildren<StarterAssetsInputs>();
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (!canMove) return;

        MovePlayer();
        PickupRay();
        Stamina();
        OpenDoor();
    }

    private void MovePlayer()
    {
        Vector3 move = new Vector3(starterAssetsInputs.move.x, 0, starterAssetsInputs.move.y);
        move = transform.TransformDirection(move) * starterAssetsInputs.moveSpeed;

        characterController.Move(move * sprintSpeed * Time.deltaTime);
    }

    private void PickupRay()
    {
        RaycastHit hit;
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * pickupRange, Color.red);
        bool pickupRayHit = Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, pickupRange, ~0, QueryTriggerInteraction.Collide);

        if (pickupRayHit && hit.collider.CompareTag("Pickup") && Input.GetKeyDown(KeyCode.E))
        {
            ItemPickup itemPickup = hit.collider.GetComponent<ItemPickup>();
            if (itemPickup != null)
            {
                itemPickup.Pickup();
            }
        }
    }

    void Stamina()
    {
        bool isMoving = starterAssetsInputs.move.magnitude > 0f; //Checks if the player is moving
        // Deduct stamina when sprinting
        if (starterAssetsInputs.sprint && isMoving)
        {
            stamina -= staminaDeduct * Time.deltaTime;
            stamina = Mathf.Max(stamina, minStamina);
            sprintSpeed = 1.25f;
            returnTimer = 0; // Reset the return timer when sprinting
        }
        else
        {
            sprintSpeed = 1f;
            returnTimer += Time.deltaTime; //return timer when not sprinting
            if (returnTimer >= returnDelay)
            {
                // Return stamina only after delay
                stamina += staminaReturn * Time.deltaTime;
                stamina = Mathf.Min(stamina, maxStamina);
            }
        }
    }

    public void EnableMovement(bool enable)
    {
        canMove = enable;
    }

    void OpenDoor()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward); // Raycast from camera's position
            RaycastHit hit;

            // Debug ray to see if it's hitting the door
            Debug.DrawRay(ray.origin, ray.direction * interactRange, Color.red);

            if (Physics.Raycast(ray, out hit, interactRange, openableLayer))
            {
                IOpenable openable = hit.collider.GetComponent<IOpenable>();
                if (openable != null)
                {
                    openable.ToggleOpen(); // Toggle the door open/close
                }
            }
        }
    }
}
