using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Transform weaponHolder;

    private Rigidbody2D rb;
    private Vector2 movement;

    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private int life = 100;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        animator.SetBool("IsMoving", movement != Vector2.zero);

        if (movement != Vector2.zero)
        {
            // 1. Calcula o ângulo da direção do movimento
            float angle = Mathf.Atan2(movement.y, movement.x) * Mathf.Rad2Deg;
            
            // 2. Aplica a rotação ao WeaponHolder
            weaponHolder.rotation = Quaternion.Euler(0f, 0f, angle);

            // 3. Vira o sprite do jogador E a arma para corrigir a orientação
            if (angle > 90 || angle < -90)
            {
                // Vira o jogador para a esquerda
                spriteRenderer.flipX = true;
                // Vira a arma na vertical para compensar (o truque está aqui!)
                weaponHolder.localScale = new Vector3(0.6666667f, -0.6666667f, 0);
            }
            else
            {
                // Volta à orientação normal para a direita
                spriteRenderer.flipX = false;
                weaponHolder.localScale = new Vector3(0.6666667f, 0.6666667f, 0);
            }
        }
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement.normalized * moveSpeed * Time.fixedDeltaTime);
    }
}