using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Enemy Stats")]
    public int startingHealth;
    public int currentHealth;
    public int physicalArmorPercentage;
    public int chemicalArmorPercentage;

    private void Awake() 
    {
        currentHealth = startingHealth;
    }

    public enum DamageType
    {
        Physical,
        Chemical
    }

    public void TakeDamage(int damageAmount, DamageType damageType)
    {
        int effectiveDamage = 0;

        // Apply appropriate armor based on damage type
        if (damageType == DamageType.Physical)
        {
            effectiveDamage = damageAmount * (100 - physicalArmorPercentage) / 100;
        }
        else if (damageType == DamageType.Chemical)
        {
            effectiveDamage = damageAmount * (100 - chemicalArmorPercentage) / 100;
        }

        currentHealth -= effectiveDamage;

        if (currentHealth <= 0)
        {
            Destroy(this.gameObject);
        }
    }
}
