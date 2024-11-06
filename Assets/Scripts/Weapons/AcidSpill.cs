using System.Collections;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class AcidSpill : MonoBehaviour
{
    private bool onCooldown = false;
    private float damageInterval = 0.5f;
    private float destroyAcidDelay = 10f;

    EnemyHealth enemyHealth;
    PlayerHealth phScript;
    [SerializeField] ThrowableWeaponSO throwableWeaponSO;

    private void Update() {
        
    }

    private void Start() 
    {
        phScript = GameObject.Find("Player").GetComponent<PlayerHealth>();
        Invoke("DestroyAcid", destroyAcidDelay);
    }
    
    private void OnTriggerStay(Collider other)
    {
        enemyHealth = other.gameObject.GetComponent<EnemyHealth>();

        if (other.gameObject.CompareTag("Enemy"))
        {
            transform.position = other.transform.position;
        }
// When we get particles for the flask it will look good, for now the spill will have to float after the enemy dies
        if (!onCooldown)
        {
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

    void SetToGround()
    {   
        RaycastHit groundHit;
        if (Physics.Raycast(transform.position, Vector3.down, out groundHit))
        {
            transform.position = groundHit.point;
        }
    }
    void DestroyAcid()
    {
        Destroy(this.gameObject);
    }

}
