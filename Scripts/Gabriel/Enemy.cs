using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed = 3f;  // Velocidade de movimentação do inimigo

    private Transform player; // Referência ao transform do player
    private Rigidbody2D rb;   // Componente Rigidbody2D do inimigo

    void Start()
    {
        // Procura o player pela tag "Player"
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        else
        {
            Debug.LogError("Player não encontrado! Certifique-se de que o objeto do player tenha a tag 'Player'.");
        }

        // Obtém o componente Rigidbody2D
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D não encontrado! Adicione um componente Rigidbody2D ao inimigo.");
        }
    }

    void FixedUpdate()
    {
        if (player != null && rb != null)
        {
            // Calcula a direção horizontal em relação ao player
            float direction = player.position.x - transform.position.x;
            float moveDirection = Mathf.Sign(direction); // -1 para esquerda, 1 para direita

            // Atualiza a velocidade horizontal mantendo a velocidade vertical inalterada
            rb.linearVelocity = new Vector2(moveDirection * speed, rb.linearVelocity.y);
        }
    }
}



