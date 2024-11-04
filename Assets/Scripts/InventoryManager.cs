using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;
    public List<Item> Items = new List<Item>();
    public Transform ItemContent;
    public GameObject InventoryItem;
    public GameObject inventoryUI;
    private bool inventoryOpen = false;
    private PlayerController playerController;

    private void Awake()
    {
        Instance = this;

        playerController = FindAnyObjectByType<PlayerController>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleInventory();
        }
    }

    public void Add(Item item)
    {
        Items.Add(item);
    }

    public void Remove(Item item)
    {
        Items.Remove(item);
    }

    public void ListItems()
    {
        foreach (Transform item in ItemContent)
        {
            Destroy(item.gameObject);
        }

        foreach (var item in Items)
        {
            GameObject obj = Instantiate(InventoryItem, ItemContent);
            var itemName = obj.transform.Find("ItemName").GetComponent<TextMeshProUGUI>();
            var itemIcon = obj.transform.Find("ItemIcon").GetComponent<Image>();

            itemName.text = item.itemName;
            itemIcon.sprite = item.icon;
        }
    }
    
    public void ToggleInventory() 
    { 
        inventoryOpen = !inventoryOpen; 
        inventoryUI.SetActive(inventoryOpen); 
        Cursor.lockState = inventoryOpen ? CursorLockMode.None : CursorLockMode.Locked; 
        Cursor.visible = inventoryOpen; 
        playerController.EnableMovement(!inventoryOpen); 
        
        if (inventoryOpen) 
        { 
            ListItems(); 
        } 
    }
}
