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

    private void CheckRecipe()
    {
        if (Input.GetKeyDown(KeyCode.C)) // will later be replaced by clicking a craft button on a recipe
                                         // the button will have a scriptable object recipe attached to it
        {
            // first updates amount of owned items
            cogwheelCount = inventory.Items.Count(item => item.itemName.Contains("Cog Wheel"));
            Debug.Log("Cogwheel count: " + cogwheelCount);
            metalScrapCount = inventory.Items.Count(item => item.itemName.Contains("Metal Scrap"));
            Debug.Log("Metal Scrap count: " + metalScrapCount);

            // checks if you can craft that recipe via a bool function, if you can, craft it, else grey out button
            foreach (var recipe in craftingRecipes)
            {
                if (CanCraftRecipe(recipe))
                {
                    CraftRecipe(recipe);
                }
                else
                {
                    Debug.Log("Not enough materials for: " + recipe.recipeName); // grey out button
                }
            }
        }
    }

    private bool CanCraftRecipe(CraftingRecipe recipe)
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

    private void CraftRecipe(CraftingRecipe recipe)
    {
        Debug.Log("Crafting: " + recipe.recipeName);

        // gets the requirements for the recipe
        foreach (var requirement in recipe.materialsRequired)
        {
            // for each crafting item required,
            for (int i = 0; i < requirement.quantityRequired; i++)
            {   // remove those crafting items if not null
                var itemToRemove = inventory.Items.FirstOrDefault(item => item.itemName.Contains(requirement.materialName));
                if (itemToRemove != null)
                {
                    inventory.Items.Remove(itemToRemove);
                }
            }
        }

        // add the crafted item to the inventory
        inventory.Items.Add(recipe.resultingItem);
    }
}
