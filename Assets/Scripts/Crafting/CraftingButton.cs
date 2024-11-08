using UnityEngine;
using UnityEngine.UI;

public class CraftingButton : MonoBehaviour
{
    public CraftingRecipe craftingRecipe;
    public Button button;


    private CraftingManager craftingManager;
    private Image buttonImage;

    void Start()
    {
        craftingManager = GameObject.Find("CraftingManager").GetComponent<CraftingManager>();

        // set up the button onClick event to trigger the crafting process
        button.onClick.AddListener(CraftItem);
    }

    private void CraftItem()
    {
        // trigger the crafting process using the assigned recipe
        craftingManager.CraftRecipe(craftingRecipe);
    }
}
