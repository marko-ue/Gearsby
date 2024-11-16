using UnityEngine;

public class AcidFlask : MonoBehaviour
{

    EnemyHealth enemyHealth;
    [SerializeField] ThrowableWeaponSO throwableWeaponSO;
    ThrowWeapon throwWeaponScript;

    [SerializeField] GameObject acidSpillGround;
    [SerializeField] GameObject acidSpillEnemy;

    public AK.Wwise.Event glassShatteringSound;
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
        glassShatteringSound.Post(this.gameObject);
        throwWeaponScript.isThrown = false;

        enemyHealth = other.gameObject.GetComponent<EnemyHealth>();
        enemyHealth?.TakeDamage(throwableWeaponSO.PhysicalDamage, EnemyHealth.DamageType.Physical);

        // Raycast downwards to find the ground position
        RaycastHit groundHit;
        Vector3 spillPosition = transform.position;

        if (other.gameObject.CompareTag("Enemy"))
        {
            Instantiate(acidSpillEnemy, other.gameObject.transform.position, Quaternion.identity);
        }

        else if (Physics.Raycast(transform.position, Vector3.down, out groundHit))
        {
            spillPosition = groundHit.point;  // Set spill position to the ground
            Instantiate(acidSpillGround, spillPosition, Quaternion.identity);
        }

        Destroy(this.gameObject);
    }
}
