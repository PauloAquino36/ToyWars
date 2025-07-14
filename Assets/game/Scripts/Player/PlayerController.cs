using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq; // Adicione esta linha

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
    private WeaponPickup nearbyWeapon = null;

    // --- NOVO INVENTÁRIO DE ARMAS ---
    private List<WeaponData> weaponInventory = new List<WeaponData>();
    private int currentWeaponIndex = -1;
    private const int maxWeapons = 2;

    // Inventário de Munição
    private Dictionary<AmmoType, int> ammoInventory = new Dictionary<AmmoType, int>();

    // Movimento
    private Vector2 movement;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        weaponController = GetComponentInChildren<WeaponController>();

        AtualizarVidaUI();
        
        // Desequipa a arma no início para começar sem nada
        weaponController.UnequipWeapon();
    }

    void Update()
    {
        if (life > 0)
        {
            // --- INPUTS ---
            movement.x = Input.GetAxis("Horizontal");
            movement.y = Input.GetAxis("Vertical");

            // Trocar de arma (ex: com a tecla Q)
            if (Input.GetKeyDown(KeyCode.Q))
            {
                SwitchWeapon();
            }

            // Pegar/Trocar arma (E)
            if (Input.GetKeyDown(KeyCode.E) && nearbyWeapon != null)
            {
                PickupOrSwapWeapon(nearbyWeapon);
            }
            
            // --- LÓGICA DE MOVIMENTO E ANIMAÇÃO ---
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
            rb.linearVelocity = Vector2.zero;
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        WeaponPickup pickup = other.GetComponent<WeaponPickup>();
        if (pickup != null)
        {
            // Verifica se a arma no chão é uma que o jogador já possui no inventário
            bool alreadyHasWeapon = weaponInventory.Any(w => w.weaponName == pickup.weaponData.weaponName);

            if (alreadyHasWeapon)
            {
                AddAmmo(pickup.weaponData.ammoType, pickup.weaponData.capacity);
                if (weaponController.weaponData != null)
                {
                   weaponController.UpdateAmmoUI();
                }
                Destroy(other.gameObject);
            }
            else
            {
                Debug.Log("Pressione 'E' para pegar " + pickup.weaponData.weaponName);
                nearbyWeapon = pickup;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<WeaponPickup>() == nearbyWeapon)
        {
            nearbyWeapon = null;
        }
    }

    // --- NOVOS MÉTODOS DE ARMA ---
    void PickupOrSwapWeapon(WeaponPickup weaponToPickup)
    {
        // Se ainda não temos 2 armas, apenas adiciona a nova
        if (weaponInventory.Count < maxWeapons)
        {
            weaponInventory.Add(weaponToPickup.weaponData);
            currentWeaponIndex = weaponInventory.Count - 1; // Equipa a nova arma
            weaponController.EquipNewWeapon(weaponInventory[currentWeaponIndex]);
        }
        // Se o inventário está cheio, troca a arma atual pela nova
        else
        {
            // Opcional: Dropar a arma antiga no chão
            // ...

            weaponInventory[currentWeaponIndex] = weaponToPickup.weaponData;
            weaponController.EquipNewWeapon(weaponInventory[currentWeaponIndex]);
        }
        
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


    // --- MÉTODOS DE GERENCIAMENTO ---

    public void TomarDano(int dano)
    {
        life -= dano;
        if (life < 0) life = 0;
        AtualizarVidaUI();

        if (life <= 0)
        {
            Debug.Log("Jogador Morreu!");
            // Aqui você pode adicionar a lógica de morte (ex: reiniciar a cena)
        }
    }

    public void AtualizarVidaUI()
    {
        if (textoVidaUI != null)
        {
            textoVidaUI.text = life.ToString();
        }
    }

    // --- MÉTODOS DE MUNIÇÃO (usados pelo WeaponController) ---
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
        Debug.Log($"Pegou {amount} de munição {type}. Total agora: {ammoInventory[type]}");
    }

    // --- LÓGICA VISUAL ---
    void HandleSpriteFlipping()
    {
        if (movement.sqrMagnitude > 0.1f)
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