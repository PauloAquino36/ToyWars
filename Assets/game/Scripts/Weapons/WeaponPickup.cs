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
    
    // O método Equip não é mais necessário aqui, pois o PlayerController
    // pega os dados diretamente do "weaponData".
    // Você pode remover o método Equip se quiser, ou deixá-lo caso
    // queira usá-lo para outra coisa no futuro.
}