using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int startingHealth;
    public int currentHealth;
    // armorPercentage reduces the amount of damage the enemy takes. Armor of 25 will reduce damage taken by 25%
    // This can later be expanded by adding different types of damage (physical, chemical) and enemies having specific armors
    public int armorPercentage;

    private void Awake() 
    {
        currentHealth = startingHealth;
    }

    public void TakeDamage(int damageAmount)
    {
        int effectiveDamage = damageAmount * (100 - armorPercentage) / 100;
        currentHealth -= effectiveDamage;

        if (currentHealth <= 0)
        {
            Destroy(this.gameObject);
        }
    }
}
