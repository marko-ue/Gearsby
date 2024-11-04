using UnityEngine;

[CreateAssetMenu(fileName = "ThrowableWeaponSO", menuName = "Scriptable Objects/ThrowableWeaponSO")]
public class ThrowableWeaponSO : ScriptableObject
{
    public float Damage;
    public bool IsThrowable;
    public float ThrowForce;
}
