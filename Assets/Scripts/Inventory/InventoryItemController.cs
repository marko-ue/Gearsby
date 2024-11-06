using UnityEngine;
using UnityEngine.UI;

public class InventoryItemController : MonoBehaviour
{
    public Item item;
    public GameObject Buttons; // Parent GameObject containing UseButton and DropButton

    private void Start()
    {
    }

    public void AddItem(Item newItem)
    {
        item = newItem;
    }

    public void UseItem()
    {
        switch (item.itemType)
        {
            case Item.ItemType.Health_Pack:
                ManagePlayerHealth.instance.AddHealth(item.value);
                break;
        }

        RemoveItem();
    }

    public void OnItemClicked() 
    { 
        // Enable the buttons on click 
        Buttons.SetActive(!Buttons.activeSelf); 
    }

    public void RemoveItem()
    {
        InventoryManager.Instance.Remove(item);
        Destroy(gameObject);
    }
}
