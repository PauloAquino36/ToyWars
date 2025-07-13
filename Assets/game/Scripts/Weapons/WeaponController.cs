using System.Collections;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [Header("Weapon Settings")]
    public WeaponData weaponData;
    public Transform firePoint;
    public GameObject bulletPrefab;

    [Header("Visuals")]
    public SpriteRenderer weaponSpriteRenderer;

    private int currentAmmo;
    private float lastFireTime;

    void Start()
    {
        currentAmmo = weaponData.capacity;
        weaponSpriteRenderer.sprite = weaponData.weaponSprite;
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Space))
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
        if (Time.time - lastFireTime < weaponData.cooldown) return;
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
        // Dispara a bala
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        // Inicia o efeito de troca de sprite
        StartCoroutine(ShootSpriteSwap());
    }

    // Corrotina para trocar o sprite da arma
    IEnumerator ShootSpriteSwap()
    {
        // 1. Troca para o sprite de "tiro", se ele existir
        if (weaponData.shootingSprite != null)
        {
            weaponSpriteRenderer.sprite = weaponData.shootingSprite;
        }

        // 2. Espera um curto período de tempo
        yield return new WaitForSeconds(0.08f); // Pode ajustar este tempo

        // 3. Volta para o sprite normal (idle)
        weaponSpriteRenderer.sprite = weaponData.weaponSprite;
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