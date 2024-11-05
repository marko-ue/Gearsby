using UnityEngine;

public class AcidFlask : MonoBehaviour
{
    EnemyHealth enemyHealth;
    [SerializeField] ThrowableWeaponSO throwableWeaponSO;
    ThrowWeapon throwWeaponScript;

    [SerializeField] GameObject acidSpill;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        throwWeaponScript = GetComponent<ThrowWeapon>();
    }

    // Update is called once per frame
    void Update()
    {
        if (throwWeaponScript != null && throwWeaponScript.isThrown == false && Input.GetMouseButtonDown(0))
        {
            throwWeaponScript.ThrowHeldWeapon();
        }
    }

    private void OnCollisionEnter(Collision other) 
    {
        if (!throwWeaponScript.isThrown) return;
        throwWeaponScript.isThrown = false;

        enemyHealth = other.gameObject.GetComponent<EnemyHealth>();
        enemyHealth?.TakeDamage(throwableWeaponSO.PhysicalDamage, EnemyHealth.DamageType.Physical);

        // Raycast downwards to find the ground position
        RaycastHit groundHit;
        Vector3 spillPosition = transform.position;

        if (Physics.Raycast(transform.position, Vector3.down, out groundHit))
        {
            spillPosition = groundHit.point;  // Set spill position to the ground
        }

        Instantiate(acidSpill, spillPosition, Quaternion.identity);

        Destroy(this.gameObject);
    }
}
