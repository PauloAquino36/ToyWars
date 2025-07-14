using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

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

        // Se o jogador já começa com uma arma, atualiza a UI de munição
        if (weaponController != null && weaponController.weaponData != null)
        {
            weaponController.UpdateAmmoUI();
        }
    }

    void Update()
    {
        if (life > 0)
        {
            movement.x = Input.GetAxis("Horizontal");
            movement.y = Input.GetAxis("Vertical");

            animator.SetBool("IsMoving", movement.magnitude > 0.1f);
            HandleSpriteFlipping();
            if (Input.GetKeyDown(KeyCode.E) && nearbyWeapon != null)
            {
                weaponController.EquipNewWeapon(nearbyWeapon.weaponData);
                Destroy(nearbyWeapon.gameObject);
                nearbyWeapon = null;
            }
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

    // --- LÓGICA DE DETECÇÃO DE ARMA ---
    private void OnTriggerEnter2D(Collider2D other)
    {
        WeaponPickup pickup = other.GetComponent<WeaponPickup>();
        if (pickup != null)
        {
            // Verifica se o jogador tem uma arma equipada e se a arma no chão é do mesmo tipo
            if (weaponController.weaponData != null && pickup.weaponData.name == weaponController.weaponData.name)
            {
                // Adiciona a munição da arma do chão à reserva do jogador
                AddAmmo(pickup.weaponData.ammoType, pickup.weaponData.capacity);

                // Atualiza a UI para refletir a nova quantidade de munição
                weaponController.UpdateAmmoUI();

                // Destrói a arma do chão
                Destroy(other.gameObject);
            }
            // Se for uma arma DIFERENTE, armazena para uma possível troca com 'E'
            else
            {
                Debug.Log("Pressione 'E' para trocar para " + pickup.weaponData.weaponName);
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
        if (movement.sqrMagnitude > 0.1f) // Usamos sqrMagnitude para evitar calcular a raiz quadrada
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