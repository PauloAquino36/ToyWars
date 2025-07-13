using UnityEngine;

[CreateAssetMenu(fileName = "NewWeapon", menuName = "Weapons/Weapon Data")]
public class WeaponData : ScriptableObject
{
    public string weaponName;       // Nome da arma
    public float damage;            // Dano por disparo
    public float range;             // Alcance do disparo
    public float cooldown;          // Intervalo entre disparos
    public int capacity;            // Munição no pente
    public float reloadTime;        // Tempo de recarga
    public FireType fireType;       // Tipo de disparo
    
    [Header("Sprites & Effects")] // Cabeçalho para organizar
    public Sprite weaponSprite;     // Sprite da arma parada (idle)
    public Sprite shootingSprite;   // <<< NOVO: Sprite da arma ao atirar
    public AudioClip fireSound;     // Som do disparo

    public enum FireType
    {
        SemiAutomatic,
        Automatic,
        Burst
    }
}