using UnityEngine;

public class Rock : MonoBehaviour
{
    EnemyHealth enemyHealth;
    [SerializeField] ThrowableWeaponSO throwableWeaponSO;
    GameObject heldWeapon;
    ThrowWeapon throwWeaponScript;

    private void Start() 
    {
        throwWeaponScript = GetComponent<ThrowWeapon>();
    }
    
    private void Update() 
    {
        if (heldWeapon != null && Input.GetMouseButtonDown(0))
        {
            throwWeaponScript.ThrowHeldWeapon();
        }
    }
    private void OnCollisionEnter(Collision other) 
    {
        enemyHealth = other.gameObject.GetComponent<EnemyHealth>();
        
        if (other.gameObject.CompareTag("Enemy"))
        {
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(throwableWeaponSO.PhysicalDamage);
                Debug.Log(enemyHealth.currentHealth);
            }
            
        }
    }
}
