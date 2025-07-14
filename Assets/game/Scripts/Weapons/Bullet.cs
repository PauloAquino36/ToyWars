using System;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f;
    public float lifeTime = 2f;
    public int damage; // <<-- NOVA VARIÁVEL para guardar o dano

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        rb.linearVelocity = transform.right * speed;
        Destroy(gameObject, lifeTime);
    }

    // Método chamado quando a bala colide com outro objeto que tem um Collider2D
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag != "Item")
        {
            // Tenta pegar o componente PlayerController do objeto em que colidiu
            PlayerController player = collision.GetComponent<PlayerController>();
            Console.WriteLine("Colidiu com: " + collision.gameObject.name);
            // Se o objeto for o jogador (ou qualquer outra coisa com o script PlayerController)
            if (player != null)
            {
                // Causa dano no jogador
                player.TomarDano(damage);
            }

            // --- IMPORTANTE ---
            // Adicione aqui a lógica para dano em inimigos no futuro. Exemplo:
            // EnemyController enemy = collision.GetComponent<EnemyController>();
            // if (enemy != null)
            // {
            //     enemy.TomarDano(damage);
            // }

            // Destrói a bala ao colidir com QUALQUER COISA que tenha um Collider2D
            Destroy(gameObject);
        }
    }
}