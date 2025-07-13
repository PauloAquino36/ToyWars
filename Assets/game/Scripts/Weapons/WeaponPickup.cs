using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class WeaponPickup : MonoBehaviour
{
    // O ScriptableObject que contém os dados desta arma
    public WeaponData weaponData;

    void Awake()
    {
        // Garante que o sprite mostrado no chão corresponde ao da arma
        if (weaponData != null)
        {
            GetComponent<SpriteRenderer>().sprite = weaponData.weaponSprite;
        }
    }

    // Este método é chamado pelo jogador para equipar a arma
    public void Equip(WeaponController playerWeaponController)
    {
        playerWeaponController.EquipNewWeapon(weaponData);
        // A arma no chão desaparece depois de ser pega
        Destroy(gameObject);
    }
}