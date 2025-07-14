using UnityEngine;

public enum AmmoType
{
    None,
    Pistol,
    Rifle,
    Shotgun
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
    public float spreadAngle;
    public int capacity;
    public float reloadTime;
    public FireType fireType;

    [Header("Burst / Shotgun Settings")]
    [Tooltip("Número de DISPAROS em uma rajada (Burst).")]
    public int burstShotCount = 1; // Renomeado para clareza

    [Tooltip("Número de PROJÉTEIS por disparo (para efeito shotgun).")] // <-- NOVA VARIÁVEL
    public int projectilesPerShot = 1;

    [Tooltip("O atraso entre cada disparo em uma rajada (apenas para Burst).")]
    public float burstDelay = 0.05f;

    [Header("Ammo")]
    public AmmoType ammoType;

    [Header("Sprites & Effects")]
    public Sprite weaponSprite;
    public Sprite shootingSprite;
    public AudioClip fireSound;

    public enum FireType
    {
        None,
        SemiAutomatic,
        Automatic,
        Burst // Shotgun foi removido, pois agora é parte do Burst
    }
}