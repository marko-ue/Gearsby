using UnityEngine;
using UnityEngine.UI;

public class CraftingButton : MonoBehaviour
{
    public CraftingRecipe craftingRecipe;
    public Button button;


    private CraftingManager craftingManager;
    private Image buttonImage;

    public AK.Wwise.Event solidCraftSound;
    public AK.Wwise.Event liquidCraftSound;
    public AK.Wwise.Event insufficientMaterialsSound;

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
            else if (craftingRecipe.recipeType == "Liquid")
            {
                liquidCraftSound.Post(this.gameObject);
            }
        }
        else
        {
            insufficientMaterialsSound.Post(this.gameObject);
        }     
    }
}
