using UnityEngine;
using System.Collections;
using TMPro;

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
    private bool isShooting = false;

    private PlayerController playerController;
    private AudioSource audioSource;

    void Awake()
    {
        playerController = GetComponentInParent<PlayerController>();
        audioSource = GetComponent<AudioSource>();
        if (weaponData != null)
        {
            EquipNewWeapon(weaponData);
        }
    }

    void Update()
    {
        if (weaponData == null || isShooting) return;

        switch (weaponData.fireType)
        {
            case WeaponData.FireType.Automatic:
                if (Input.GetKey(KeyCode.Space)) TryShoot();
                break;

            case WeaponData.FireType.SemiAutomatic:
            case WeaponData.FireType.Burst:
                if (Input.GetKeyDown(KeyCode.Space)) TryShoot();
                break;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            Reload();
        }
    }

    void TryShoot()
    {
        if (Time.time < lastFireTime + weaponData.cooldown) return;

        if (currentAmmo <= 0)
        {
            Debug.Log("Sem munição! Recarregue.");
            return;
        }

        lastFireTime = Time.time;

        switch (weaponData.fireType)
        {
            case WeaponData.FireType.Burst:
                StartCoroutine(FireBurst());
                break;
            default:
                FireSingle();
                break;
        }
    }

    void FireSingle()
    {
        currentAmmo--;
        InstantiateBullet();
        UpdateAmmoUI();
    }
    
    // ✅ LÓGICA DE MUNIÇÃO CORRIGIDA AQUI
    IEnumerator FireBurst()
    {
        isShooting = true;

        // Consome apenas 1 de munição para o tiro inteiro, ANTES de disparar.
        currentAmmo--;
        UpdateAmmoUI(); // Atualiza a UI imediatamente

        // Loop para o número de DISPAROS na rajada (se for uma shotgun de 1 tiro, o loop roda 1 vez)
        for (int i = 0; i < weaponData.burstShotCount; i++)
        {
            // Loop para o número de PROJÉTEIS por disparo (o efeito shotgun)
            for (int j = 0; j < weaponData.projectilesPerShot; j++)
            {
                InstantiateBullet();
            }
            
            // Espera um pouco antes do próximo disparo da rajada (se houver)
            if (weaponData.burstShotCount > 1)
            {
                yield return new WaitForSeconds(weaponData.burstDelay);
            }
        }
        
        isShooting = false;
    }

    void InstantiateBullet()
    {
        float currentSpread = Random.Range(-weaponData.spreadAngle / 2, weaponData.spreadAngle / 2);
        Quaternion spreadRotation = Quaternion.Euler(new Vector3(0, 0, currentSpread));
        Quaternion finalRotation = firePoint.rotation * spreadRotation;

        GameObject bulletInstance = Instantiate(bulletPrefab, firePoint.position, finalRotation);
        Bullet bulletScript = bulletInstance.GetComponent<Bullet>();

        if (bulletScript != null)
        {
            bulletScript.damage = (int)weaponData.damage;
        }

        if (weaponData.fireSound != null)
        {
            audioSource.PlayOneShot(weaponData.fireSound);
        }

        StartCoroutine(ShootSpriteSwap());
    }
    
    // --- O restante do código (Reload, Equip, etc.) permanece o mesmo ---

    IEnumerator ShootSpriteSwap()
    {
        if (weaponData.shootingSprite != null)
        {
            weaponSpriteRenderer.sprite = weaponData.shootingSprite;
        }
        yield return new WaitForSeconds(0.08f);
        weaponSpriteRenderer.sprite = weaponData.weaponSprite;
    }

    public void EquipNewWeapon(WeaponData newWeaponData)
    {
        weaponData = newWeaponData;
        currentAmmo = weaponData.capacity;
        weaponSpriteRenderer.sprite = weaponData.weaponSprite;
        weaponSpriteRenderer.enabled = true;
        UpdateAmmoUI();
    }
    
    public void UnequipWeapon()
    {
        weaponData = null;
        weaponSpriteRenderer.enabled = false;
        if (playerController != null && playerController.textoMunicaoUI != null)
        {
            playerController.textoMunicaoUI.text = "";
        }
    }

    void Reload()
    {
        if (isShooting) return;
        if (currentAmmo < weaponData.capacity && playerController.GetAmmo(weaponData.ammoType) > 0)
        {
            Debug.Log("Recarregando...");
            Invoke(nameof(FinishReload), weaponData.reloadTime);
        }
    }

    void FinishReload()
    {
        int ammoNeeded = weaponData.capacity - currentAmmo;
        int ammoToReload = Mathf.Min(ammoNeeded, playerController.GetAmmo(weaponData.ammoType));
        currentAmmo += ammoToReload;
        playerController.UseAmmo(weaponData.ammoType, ammoToReload);
        Debug.Log("Arma recarregada!");
        UpdateAmmoUI();
    }
    
    public void UpdateAmmoUI()
    {
        if (weaponData != null && playerController != null && playerController.textoMunicaoUI != null)
        {
            int totalAmmo = playerController.GetAmmo(weaponData.ammoType);
            playerController.textoMunicaoUI.text = $"{currentAmmo} / {totalAmmo}";
        }
    }
}