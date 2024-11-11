using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CraftingRecipes", menuName = "Scriptable Objects/CraftingRecipes")]
public class CraftingRecipe : ScriptableObject
{
    public string recipeName;
    [Header("Solid/liquid")]
    [Tooltip("Specify the recipe type in either Solid or Liquid")]
    public string recipeType;
    public List<MaterialsRequired> materialsRequired;
    public Item resultingItem;

    [System.Serializable]
    public class MaterialsRequired
    {
        public string materialName;
        public int quantityRequired;
    }
}
