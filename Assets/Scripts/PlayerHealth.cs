using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float health = 100f;
    public float maxHealth = 100f;
    public float minHealth = 0f;

    void Update()
    {
        // Clamp the health value between 0 and maxHealth
        health = Mathf.Clamp(health, minHealth, maxHealth);
    }
}
