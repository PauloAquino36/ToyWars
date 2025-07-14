using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Transform weaponHolder;

    [Header("UI Elements")]
    public TextMeshProUGUI textoVidaUI;
    public TextMeshProUGUI textoMunicaoUI;
    public Image imagemCoracaoUI;
    public Image imagemBalaUI;

    // Player Stats
    public int life = 100;

    // Referências
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private WeaponController weaponController;
    private WeaponPickup nearbyWeapon = null; // Guarda a arma que pode ser trocada com 'E'

    // Inventário de Armas
    private List<WeaponData> weaponInventory = new List<WeaponData>();
    private int currentWeaponIndex = -1;
    private const int maxWeapons = 2;

    // Inventário de Munição
    private Dictionary<AmmoType, int> ammoInventory = new Dictionary<AmmoType, int>();

    // Movimento
    private Vector2 movement;

    void Start()
    {
        // Pega os componentes necessários
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        weaponController = GetComponentInChildren<WeaponController>();

        AtualizarVidaUI();
        
        // Garante que o jogador comece sem nenhuma arma visível
        weaponController.UnequipWeapon();
    }

    void Update()
    {
        if (life > 0)
        {
            // --- INPUTS ---
            movement.x = Input.GetAxis("Horizontal");
            movement.y = Input.GetAxis("Vertical");

            // Trocar de arma com a tecla Q
            if (Input.GetKeyDown(KeyCode.Q))
            {
                SwitchWeapon();
            }

            // Pegar ou trocar de arma com a tecla E
            if (Input.GetKeyDown(KeyCode.E) && nearbyWeapon != null)
            {
                PickupOrSwapWeapon(nearbyWeapon);
            }
            
            // --- LÓGICA DE ANIMAÇÃO E MOVIMENTO ---
            animator.SetBool("IsMoving", movement.magnitude > 0.1f);
            HandleSpriteFlipping();

            // Exemplo para testar o dano
            if (Input.GetKeyDown(KeyCode.T))
            {
                TomarDano(10);
            }
        }
        else
        {
            animator.SetBool("IsDead", true);
            rb.linearVelocity = Vector2.zero; // Para o movimento do jogador
            Debug.Log("Jogador Morreu!");
        }
    }

    void FixedUpdate()
    {
        Vector2 move = movement;
        if (move.magnitude > 1f)
            move = move.normalized;

        rb.MovePosition(rb.position + move * moveSpeed * Time.fixedDeltaTime);
    }

    // --- LÓGICA DE ARMAS ---

    void PickupOrSwapWeapon(WeaponPickup weaponToPickup)
    {
        // Se o inventário está cheio, dropa a arma atual antes de pegar a nova
        if (weaponInventory.Count >= maxWeapons)
        {
            // Pega os dados da arma que será dropada
            WeaponData weaponToDrop = weaponInventory[currentWeaponIndex];
            
            // Verifica se a arma a ser dropada tem um prefab de chão associado
            if (weaponToDrop.groundWeaponPrefab != null)
            {
                // Instancia a arma antiga na posição do jogador
                Instantiate(weaponToDrop.groundWeaponPrefab, transform.position, Quaternion.identity);
                Debug.Log("Dropou a arma: " + weaponToDrop.weaponName);
            }

            // Substitui a arma antiga pela nova no inventário
            weaponInventory[currentWeaponIndex] = weaponToPickup.weaponData;
            weaponController.EquipNewWeapon(weaponInventory[currentWeaponIndex]);
        }
        // Se ainda há espaço no inventário, apenas adiciona a nova arma
        else
        {
            weaponInventory.Add(weaponToPickup.weaponData);
            currentWeaponIndex = weaponInventory.Count - 1; // Equipa a nova arma
            weaponController.EquipNewWeapon(weaponInventory[currentWeaponIndex]);
        }
        
        // Destrói o objeto da arma que foi pega do chão
        Destroy(weaponToPickup.gameObject);
        nearbyWeapon = null;
    }

    void SwitchWeapon()
    {
        // Só troca de arma se tiver mais de uma
        if (weaponInventory.Count > 1)
        {
            currentWeaponIndex++;
            if (currentWeaponIndex >= weaponInventory.Count)
            {
                currentWeaponIndex = 0;
            }

            weaponController.EquipNewWeapon(weaponInventory[currentWeaponIndex]);
            Debug.Log("Trocou para: " + weaponInventory[currentWeaponIndex].weaponName);
        }
    }

    // --- LÓGICA DE DETECÇÃO ---

    private void OnTriggerEnter2D(Collider2D other)
    {
        WeaponPickup pickup = other.GetComponent<WeaponPickup>();
        if (pickup != null)
        {
            // Verifica se o jogador já possui uma arma do mesmo tipo no inventário
            bool alreadyHasWeapon = weaponInventory.Any(w => w.weaponName == pickup.weaponData.weaponName);

            // Se já tem a arma, apenas pega munição
            if (alreadyHasWeapon)
            {
                AddAmmo(pickup.weaponData.ammoType, pickup.weaponData.capacity);
                if (weaponController.weaponData != null)
                {
                   weaponController.UpdateAmmoUI();
                }
                Destroy(other.gameObject);
            }
            // Se for uma arma diferente, permite a coleta
            else
            {
                Debug.Log("Pressione 'E' para pegar " + pickup.weaponData.weaponName);
                nearbyWeapon = pickup;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // Limpa a referência da arma próxima quando o jogador se afasta
        if (other.GetComponent<WeaponPickup>() == nearbyWeapon)
        {
            nearbyWeapon = null;
        }
    }

    // --- MÉTODOS DE GERENCIAMENTO (Vida e Munição) ---

    public void TomarDano(int dano)
    {
        life -= dano;
        if (life < 0) life = 0;
        AtualizarVidaUI();

        if (life <= 0)
        {
            // A lógica de morte já está no Update, aqui apenas garantimos o estado
        }
    }

    public void AtualizarVidaUI()
    {
        if (textoVidaUI != null)
        {
            textoVidaUI.text = life.ToString();
        }
    }

    public int GetAmmo(AmmoType type)
    {
        return ammoInventory.ContainsKey(type) ? ammoInventory[type] : 0;
    }

    public void UseAmmo(AmmoType type, int amount)
    {
        if (ammoInventory.ContainsKey(type))
        {
            ammoInventory[type] -= amount;
        }
    }

    public void AddAmmo(AmmoType type, int amount)
    {
        if (!ammoInventory.ContainsKey(type))
        {
            ammoInventory[type] = 0;
        }
        ammoInventory[type] += amount;
        Debug.Log($"Pegou {amount} de munição {type}. Total agora: {GetAmmo(type)}");
    }

    // --- LÓGICA VISUAL ---
    void HandleSpriteFlipping()
    {
        if (movement.sqrMagnitude > 0.1f) // Usamos sqrMagnitude para otimização
        {
            float angle = Mathf.Atan2(movement.y, movement.x) * Mathf.Rad2Deg;
            weaponHolder.rotation = Quaternion.Euler(0f, 0f, angle);

            if (angle > 90 || angle < -90)
            {
                spriteRenderer.flipX = true;
                weaponHolder.localScale = new Vector3(0.6666667f, -0.6666667f, 0);
            }
            else
            {
                spriteRenderer.flipX = false;
                weaponHolder.localScale = new Vector3(0.6666667f, 0.6666667f, 0);
            }
        }
    }
}