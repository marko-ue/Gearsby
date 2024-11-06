using UnityEngine;

public class ItemDrop : MonoBehaviour
{
    public void DropItem(Vector3 dropPosition, Vector3 force)
    {
        // Set the item's position to the drop position
        transform.position = dropPosition;

        // Add necessary components if not already present
        if (!TryGetComponent<Rigidbody>(out var rb))
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }

        // Apply a forward force to simulate dropping the item
        rb.AddForce(force);
    }
}
