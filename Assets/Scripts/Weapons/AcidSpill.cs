using System.Collections;
using UnityEngine;

public class AcidSpill : MonoBehaviour
{
    private bool onCooldown = false;

    EnemyHealth enemyHealth;
    [SerializeField] ThrowableWeaponSO throwableWeaponSO;
    
    private void OnTriggerStay(Collider other)
    {
        if (!onCooldown && enemyHealth != null)
        {
            enemyHealth = other.gameObject.GetComponent<EnemyHealth>();
            StartCoroutine(DamageCooldown());
            Debug.Log(enemyHealth.currentHealth);
        }
    }

    IEnumerator DamageCooldown()
    {
        onCooldown = true;
        enemyHealth?.TakeDamage(throwableWeaponSO.ChemicalDamage, EnemyHealth.DamageType.Chemical);
        yield return new WaitForSeconds(1);
        onCooldown = false;
    }

}
