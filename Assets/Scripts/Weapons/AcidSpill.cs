using System.Collections;
using UnityEngine;

public class AcidSpill : MonoBehaviour
{
    private bool onCooldown = false;
    private float damageInterval = 0.5f;
    private float destroyAcidDelay = 10f;

    EnemyHealth enemyHealth;
    PlayerHealth phScript;
    [SerializeField] ThrowableWeaponSO throwableWeaponSO;

    private void Start() 
    {
        phScript = GameObject.Find("Player").GetComponent<PlayerHealth>();
        Invoke("DestroyAcid", destroyAcidDelay);
    }
    
    private void OnTriggerStay(Collider other)
    {
        if (!onCooldown)
        {
            enemyHealth = other.gameObject.GetComponent<EnemyHealth>();
            phScript.health -= throwableWeaponSO.ChemicalDamage;
            StartCoroutine(DamageCooldown());
        }
    }

    IEnumerator DamageCooldown()
    {
        onCooldown = true;
        enemyHealth?.TakeDamage(throwableWeaponSO.ChemicalDamage, EnemyHealth.DamageType.Chemical);
        yield return new WaitForSeconds(damageInterval);
        onCooldown = false;
    }

    void DestroyAcid()
    {
        Destroy(this.gameObject);
    }

}
