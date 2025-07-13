using UnityEngine;

// Adicione este enum ANTES da classe WeaponData
public enum AmmoType
{
    None, // Para objetos que não usam munição
    Pistol,
    Rifle,
    Shotgun
    // Adicione mais tipos conforme precisar
}

[CreateAssetMenu(fileName = "NewWeapon", menuName = "Weapons/Weapon Data")]
public class WeaponData : ScriptableObject
{
    public string weaponName;
    public float damage;
    public float range;
    public float cooldown;
     [Header("Precision")]
    [Tooltip("O ângulo máximo de dispersão do tiro. 0 para precisão perfeita.")]
    public float spreadAngle; // <<< NOVA VARIÁVEL
    public int capacity;
    public float reloadTime;
    public FireType fireType;

    [Header("Ammo")]
    public AmmoType ammoType; // <<-- NOVA VARIÁVEL

    [Header("Sprites & Effects")]
    public Sprite weaponSprite;
    public Sprite shootingSprite;
    public AudioClip fireSound;

    public enum FireType
    {
        None,
        SemiAutomatic,
        Automatic,
        Burst
    }
}