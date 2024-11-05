using UnityEngine;

public class AcidFlask : MonoBehaviour
{
    EnemyHealth enemyHealth;
    [SerializeField] ThrowableWeaponSO throwableWeaponSO;
    ThrowWeapon throwWeaponScript;
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
        enemyHealth?.TakeDamage(throwableWeaponSO.ChemicalDamage, EnemyHealth.DamageType.Physical);
        Debug.Log("entered");
        // TODO: make glass animation to play when the flask collides
        Destroy(this.gameObject);
    }
}
