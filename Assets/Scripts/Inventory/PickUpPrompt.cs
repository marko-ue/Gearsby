using UnityEngine;

public class PromptHandler : MonoBehaviour
{
    public GameObject promptPrefab; // Assign your "E" prompt prefab in the Inspector
    public Transform playerCamera; // Assign your camera transform
    public float pickupRange = 2f; // Range for detecting items
    public float promptDistance = 0.5f; // Distance in front of the object to place the prompt

    private GameObject currentPrompt; // Active prompt object
    private GameObject currentTarget; // The item currently being looked at

    private void Update()
    {
        CheckForItem();
    }

    private void CheckForItem()
    {
        Ray ray = new Ray(playerCamera.position, playerCamera.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, pickupRange))
        {
            // Check if the object has the correct tag
            if (hit.collider.CompareTag("Pickup") || hit.collider.CompareTag("Throwable Pickup"))
            {
                if (currentTarget != hit.collider.gameObject)
                {
                    // If this is a new target, show a prompt
                    currentTarget = hit.collider.gameObject;
                    ShowPrompt(hit.collider.gameObject);
                }
                return;
            }
        }

        // If no valid item is detected, hide the prompt
        HidePrompt();
    }

    private void ShowPrompt(GameObject target)
    {
        // If there's no active prompt, instantiate one
        if (currentPrompt == null && promptPrefab != null)
        {
            currentPrompt = Instantiate(promptPrefab);
        }

        if (currentPrompt != null)
        {
            // Position the prompt in front of the target item based on its forward direction
            Vector3 directionToFront = target.transform.forward; // Get the forward direction of the target
            Vector3 promptPosition = target.transform.position + directionToFront * promptDistance + Vector3.up * 0.5f; // Slightly above to avoid collision

            currentPrompt.transform.position = promptPosition;

            // Make the prompt face the camera by rotating only on the Y-axis (ignoring vertical rotation)
            Vector3 directionToFace = currentPrompt.transform.position - playerCamera.position;
            directionToFace.y = 0; // Ignore vertical rotation to avoid tilting
            currentPrompt.transform.rotation = Quaternion.LookRotation(directionToFace);

            // Scale the prompt based on the size of the target object
            float scaleFactor = target.GetComponent<Renderer>().bounds.size.magnitude / 2f; // Use object size for scaling
            currentPrompt.transform.localScale = Vector3.one * Mathf.Clamp(scaleFactor, 0.5f, 2f); // Clamp size for visual consistency
        }
    }

    private void HidePrompt()
    {
        // Destroy the prompt and clear the target
        if (currentPrompt != null)
        {
            Destroy(currentPrompt);
        }
        currentPrompt = null;
        currentTarget = null;
    }
}
