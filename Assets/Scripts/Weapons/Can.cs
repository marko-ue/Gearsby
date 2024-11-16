using UnityEngine;

public class Can : MonoBehaviour
{
    EnemyHealth enemyHealth;
    [SerializeField] ThrowableWeaponSO throwableWeaponSO;
    public AK.Wwise.Event canHitSound;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision other) 
    {
        enemyHealth = other.gameObject.GetComponent<EnemyHealth>();
        canHitSound.Post(this.gameObject);
        
        if (other.gameObject.CompareTag("Enemy"))
        {
            enemyHealth?.TakeDamage(throwableWeaponSO.PhysicalDamage, EnemyHealth.DamageType.Physical);
        } 
    }
}
