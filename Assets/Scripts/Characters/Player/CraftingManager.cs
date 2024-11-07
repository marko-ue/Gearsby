using NUnit.Framework;
using UnityEditor.Rendering;
using UnityEngine;
using System.Linq;

public class CraftingManager : MonoBehaviour
{
    public int cogwheelCount;
    public int metalScrapCount;
    InventoryManager inventory;

    void Start()
    {
        inventory = GameObject.Find("InventoryManager").GetComponent<InventoryManager>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateItemCount();
    }

    private void UpdateItemCount()
    {
        if (Input.GetKeyDown(KeyCode.C)) // will later be replaced by accessing a sort of crafting table with a prompt
        {
            cogwheelCount = inventory.Items.Count(item => item.itemName.Contains("Cog Wheel"));
            Debug.Log("Cogwheel count: " + cogwheelCount);
            metalScrapCount = inventory.Items.Count(item => item.itemName.Contains("Metal Scrap"));
            Debug.Log("Metal Scrap count: " + metalScrapCount);
        }
    }
}
