using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CraftingRecipes", menuName = "Scriptable Objects/CraftingRecipes")]
public class CraftingRecipe : ScriptableObject
{
    public string recipeName;
    public List<MaterialsRequired> materialsRequired;
    public Item resultingItem;

    [System.Serializable]
    public class MaterialsRequired
    {
        public string materialName;
        public int quantityRequired;
    }
}
