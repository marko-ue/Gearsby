using UnityEngine;
using UnityEngine.UI;

public class CraftingButton : MonoBehaviour
{
    public CraftingRecipe craftingRecipe;
    public Button button;


    private CraftingManager craftingManager;
    private Image buttonImage;

    public AK.Wwise.Event solidCraftSound; // there will be a liquid craft sound added later

    void Start()
    {
        craftingManager = GameObject.Find("CraftingManager").GetComponent<CraftingManager>();

        // set up the button onClick event to trigger the crafting process
        button.onClick.AddListener(CraftItem);
    }

    private void CraftItem()
    {
        // trigger the crafting process using the assigned recipe
        if (craftingManager.CraftRecipe(craftingRecipe))
        {
            if (craftingRecipe.recipeType == "Solid")
            {
                solidCraftSound.Post(this.gameObject);
            }
            else
            {
                // play liquid crafting sound, if there end up being more types of recipes add else if
            }
        }
        else
        {
            // play sound for not enough materials
        }     
    }
}
