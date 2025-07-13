using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [Header("Weapon Settings")]
    public WeaponData weaponData;
    public Transform firePoint;
    public GameObject bulletPrefab;

    [Header("Visual")]
    public Transform weaponHolder;  // O Empty
    public SpriteRenderer weaponSpriteRenderer; // O SpriteRenderer da arma

    private int currentAmmo;
    private float lastFireTime;

    void Start()
    {
        currentAmmo = weaponData.capacity;

        // Aplica o sprite da arma equipada
        weaponSpriteRenderer.sprite = weaponData.weaponSprite;
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            TryShoot();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            Reload();
        }
    }

    void TryShoot()
    {
        if (Time.time - lastFireTime < weaponData.cooldown)
        {
            Debug.Log("Aguardando cooldown...");
            return;
        }

        if (currentAmmo <= 0)
        {
            Debug.Log("Sem munição! Recarregue.");
            return;
        }

        Fire();
        currentAmmo--;
        lastFireTime = Time.time;
    }

    void Fire()
    {
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Debug.Log($"Disparou {weaponData.weaponName}!");
    }

    void Reload()
    {
        Debug.Log("Recarregando...");
        Invoke(nameof(FinishReload), weaponData.reloadTime);
    }

    void FinishReload()
    {
        currentAmmo = weaponData.capacity;
        Debug.Log("Arma recarregada!");
    }
}
