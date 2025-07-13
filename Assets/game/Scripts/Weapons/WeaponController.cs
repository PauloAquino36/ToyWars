using UnityEngine;
using System.Collections;
using TMPro; // Necessário para TextMeshPro

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

    // Referência para o controlador do jogador
    private PlayerController playerController;
    private AudioSource audioSource;

    void Start()
    {
        // Pega a referência do PlayerController no objeto pai
        playerController = GetComponentInParent<PlayerController>();

        // Pega o componente AudioSource para tocar sons
        audioSource = GetComponent<AudioSource>();
        // Configura a arma inicial
        if (weaponData != null)
        {
            EquipNewWeapon(weaponData);
        }
    }

    void Update()
    {
        // Se não houver arma, não faz nada
        if (weaponData == null) return;

        // Tenta atirar
        if (Input.GetKey(KeyCode.Space))
        {
            TryShoot();
        }

        // Tenta recarregar
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
            // Futuramente, pode tocar um som de "clique" de arma vazia aqui
            return;
        }

        Fire();
        currentAmmo--;
        lastFireTime = Time.time;
        UpdateAmmoUI(); // Atualiza a UI sempre que atirar
    }

    void Fire()
    {
        // --- LÓGICA DE PRECISÃO ---
        // 1. Calcula um ângulo de desvio aleatório dentro do cone de dispersão da arma.
        float currentSpread = Random.Range(-weaponData.spreadAngle / 2, weaponData.spreadAngle / 2);

        // 2. Cria uma rotação que representa esse desvio.
        Quaternion spreadRotation = Quaternion.Euler(new Vector3(0, 0, currentSpread));
        
        // 3. Combina a rotação original do ponto de tiro com a rotação de desvio para obter a direção final.
        Quaternion finalRotation = firePoint.rotation * spreadRotation;
        
        // --- FIM DA LÓGICA DE PRECISÃO ---

        // Instancia a bala usando a rotação final, que agora inclui a imprecisão.
        GameObject bulletInstance = Instantiate(bulletPrefab, firePoint.position, finalRotation);

        // Pega o script da bala recém-criada para passar informações.
        Bullet bulletScript = bulletInstance.GetComponent<Bullet>();

        // Passa o dano da arma para a bala.
        if (bulletScript != null)
        {
            bulletScript.damage = (int)weaponData.damage;
        }
        // --- TOCAR O SOM DO TIRO ---
        // 1. Verifica se existe um clipe de áudio definido na arma
        if (weaponData.fireSound != null)
        {
            // 2. Toca o som uma vez
            audioSource.PlayOneShot(weaponData.fireSound);
        }

        // Inicia a corrotina para o efeito visual do tiro.
        StartCoroutine(ShootSpriteSwap());
    }

    // Corrotina para trocar o sprite da arma temporariamente ao atirar
    IEnumerator ShootSpriteSwap()
    {
        // 1. Troca para o sprite de "tiro", se ele existir
        if (weaponData.shootingSprite != null)
        {
            weaponSpriteRenderer.sprite = weaponData.shootingSprite;
        }

        // 2. Espera um curto período de tempo
        yield return new WaitForSeconds(0.08f);

        // 3. Volta para o sprite normal (idle)
        weaponSpriteRenderer.sprite = weaponData.weaponSprite;
    }

    void Reload()
    {
        // Verifica se a arma não está cheia e se o jogador tem munição reserva para ela
        if (currentAmmo < weaponData.capacity && playerController.GetAmmo(weaponData.ammoType) > 0)
        {
            Debug.Log("Recarregando...");
            Invoke(nameof(FinishReload), weaponData.reloadTime);
        }
        else
        {
            Debug.Log("Não é possível recarregar: Pente cheio ou sem munição reserva!");
        }
    }

    void FinishReload()
    {
        int ammoNeeded = weaponData.capacity - currentAmmo;
        int ammoToReload = Mathf.Min(ammoNeeded, playerController.GetAmmo(weaponData.ammoType));

        currentAmmo += ammoToReload;
        playerController.UseAmmo(weaponData.ammoType, ammoToReload);

        Debug.Log("Arma recarregada!");
        UpdateAmmoUI(); // Atualiza a UI após terminar a recarga
    }

    public void EquipNewWeapon(WeaponData newWeaponData)
    {
        weaponData = newWeaponData;
        currentAmmo = weaponData.capacity;
        weaponSpriteRenderer.sprite = weaponData.weaponSprite;
        // Torna o sprite visível ao equipar
        weaponSpriteRenderer.enabled = true;
        UpdateAmmoUI(); // Atualiza a UI para a nova arma
    }
    
    // Atualiza o texto da munição na tela
    public void UpdateAmmoUI()
    {
        if (playerController != null && playerController.textoMunicaoUI != null)
        {
            int totalAmmo = playerController.GetAmmo(weaponData.ammoType);
            playerController.textoMunicaoUI.text = $"{currentAmmo} / {totalAmmo}";
        }
    }
}