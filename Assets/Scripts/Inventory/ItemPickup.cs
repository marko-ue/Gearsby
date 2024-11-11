using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public Item Item;

    public AK.Wwise.Event pickupSound;

    public void Pickup()
    {
        InventoryManager.Instance.Add(Item);
        pickupSound.Post(this.gameObject);
        Destroy(gameObject);
    }
}
