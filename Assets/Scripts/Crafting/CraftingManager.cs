using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class CraftingManager : MonoBehaviour
{
    public int cogwheelCount;
    public int metalScrapCount;
    InventoryManager inventory;
    public List<CraftingRecipe> craftingRecipes;

    void Start()
    {
        inventory = GameObject.Find("InventoryManager").GetComponent<InventoryManager>();
    }

    void Update()
    {
        CheckRecipe();
    }

    public void CheckRecipe()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            // first update amount of owned items
            cogwheelCount = inventory.Items.Count(item => item.itemName.Contains("Cog Wheel"));
            Debug.Log("Cogwheel count: " + cogwheelCount);
            metalScrapCount = inventory.Items.Count(item => item.itemName.Contains("Metal Scrap"));
            Debug.Log("Metal Scrap count: " + metalScrapCount);

            // checks if you can craft that recipe via a bool function, if you can, craft it, else grey out button
            foreach (var recipe in craftingRecipes)
            {
                if (recipe != null)
                {
                    if (CanCraftRecipe(recipe))
                    {     
                        CraftRecipe(recipe);
                    }
                    else
                    {
                        Debug.Log("Not enough materials for: " + recipe.recipeName); 
                    }
                }
            }
        }
    }


    public bool CanCraftRecipe(CraftingRecipe recipe)
    {
        // checks the required items for that recipe
        foreach (var requirement in recipe.materialsRequired)
        {
            // checks if and how many items that are required are in the inventory
            int materialCount = inventory.Items.Count(item => item.itemName.Contains(requirement.materialName));

            // if there are not enough materials, return false (don't run craft function)
            if (materialCount < requirement.quantityRequired)
            {
                return false;
            }
        }
        // if there are enough, return true and run the craft function
        return true;
    }

    public bool CraftRecipe(CraftingRecipe recipe)
    {
        if (CanCraftRecipe(recipe))
        {
            Debug.Log("Crafting: " + recipe.recipeName);

            // remove materials from inventory
            foreach (var requirement in recipe.materialsRequired)
            {
                // iterate for each item required
                for (int i = 0; i < requirement.quantityRequired; i++)
                {
                    var itemToRemove = inventory.Items.FirstOrDefault(item => item.itemName.Contains(requirement.materialName));
                    if (itemToRemove != null)
                    {
                        inventory.Items.Remove(itemToRemove);        
                    }
                }
            }
            // Add the crafted item to the inventory
            inventory.Items.Add(recipe.resultingItem);
            return true;
        }
        else
        {
            Debug.Log("Not enough materials for: " + recipe.recipeName);
            return false;
        }
    }
}