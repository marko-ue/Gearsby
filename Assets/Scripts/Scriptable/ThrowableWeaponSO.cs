using UnityEngine;

[CreateAssetMenu(fileName = "ThrowableWeaponSO", menuName = "Scriptable Objects/ThrowableWeaponSO")]
public class ThrowableWeaponSO : ScriptableObject
{
    public int PhysicalDamage;
    public int ChemicalDamage;
    public bool IsThrowable;
    public float ThrowForce;
}
