using UnityEngine;

public class HouseAreaTrigger : MonoBehaviour
{
    public AK.Wwise.State enterState;
    public AK.Wwise.State exitState;

    private void OnTriggerEnter(Collider other)
    {
        // Check if the other collider is the player
        if (other.CompareTag("Player"))
        {
            enterState.SetValue();  // Change state when the player enters the house
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Check if the other collider is the player
        if (other.CompareTag("Player"))
        {
            exitState.SetValue();  // Change state when the player exits the house
        }
    }
}