using UnityEngine;
using StarterAssets;

public class Crouch : MonoBehaviour
{
    StarterAssetsInputs starterAssetsInputs;
    CharacterController characterController;
    Climbing climbing;
    public Transform PlayerCameraRoot;
    public Transform heldItem; // Assign Active weapon so the item follows the player when hes crouched

    private float normalHeight = 2.0f;
    private float crouchHeight = 1.0f;
    private Vector3 normalCenter = new Vector3(0, 1.0f, 0);
    private Vector3 crouchCenter = new Vector3(0, 0.5f, 0);

    private Vector3 itemNormalPos;
    private Vector3 itemCrouchPos;
    private Vector3 cameraRootNormalPos;
    private Vector3 cameraRootCrouchPos;

    private float checkRange = 1f;

    private float crouchSpeed = 10f;
    private bool crouchOnCooldown = false;

    public AK.Wwise.Event crouchSound;

    void Awake()
    {
        starterAssetsInputs = GetComponentInChildren<StarterAssetsInputs>();
        characterController = GetComponent<CharacterController>();
        climbing = GetComponent<Climbing>();

        // Initialize item positions for crouching
        itemNormalPos = heldItem.localPosition;
        itemCrouchPos = itemNormalPos + Vector3.down * 0.5f;

        // Initialize PlayerCameraRoot crouch positions
        cameraRootNormalPos = PlayerCameraRoot.localPosition;
        cameraRootCrouchPos = cameraRootNormalPos + Vector3.down * 0.5f;
    }

    void Update()
    {
        Crouching();
    }

    void Crouching()
    {
        RaycastHit hit;
        bool checkCeilingRayHit = Physics.Raycast(Camera.main.transform.position, Camera.main.transform.up, out hit, checkRange);
        
        if (starterAssetsInputs.crouch)
        {   
            climbing.climbingAllowed = false;
            starterAssetsInputs.jump = false;
            starterAssetsInputs.sprint = false;
            starterAssetsInputs.moveSpeed = 0.5f;

            // Adjust CharacterController height and center
            characterController.height = Mathf.Lerp(characterController.height, crouchHeight, Time.deltaTime * crouchSpeed);
            characterController.center = Vector3.Lerp(characterController.center, crouchCenter, Time.deltaTime * crouchSpeed);

            // Move held item to crouch position
            heldItem.localPosition = Vector3.Lerp(heldItem.localPosition, itemCrouchPos, Time.deltaTime * crouchSpeed);

            // Move Playercameraroot to crouch position
            PlayerCameraRoot.localPosition = Vector3.Lerp(PlayerCameraRoot.localPosition, cameraRootCrouchPos, Time.deltaTime * crouchSpeed);

            if (!crouchOnCooldown)
            {
                crouchSound.Post(this.gameObject);
                crouchOnCooldown = true;
            }
        }
        else
        {   
            if (checkCeilingRayHit)
            {
                starterAssetsInputs.crouch = true;
            }
            else
            {
                crouchOnCooldown = false;
                climbing.climbingAllowed = true;
                starterAssetsInputs.moveSpeed = 1f;

                // Return to standing positions
                characterController.height = Mathf.Lerp(characterController.height, normalHeight, Time.deltaTime * crouchSpeed);
                characterController.center = Vector3.Lerp(characterController.center, normalCenter, Time.deltaTime * crouchSpeed);

                // Move held item back to normal position
                heldItem.localPosition = Vector3.Lerp(heldItem.localPosition, itemNormalPos, Time.deltaTime * crouchSpeed);

                // Reset Playercameraroot to normal position
                PlayerCameraRoot.localPosition = Vector3.Lerp(PlayerCameraRoot.localPosition, cameraRootNormalPos, Time.deltaTime * crouchSpeed);
            }  
        }
    }
}
