using UnityEngine;
using System.Collections;
public class WormEnemy : MonoBehaviour
{
    public float speed = 1f;              // Velocidade de movimento
    public float moveTime = 2f;           // Tempo em que o inimigo se move normalmente
    public float pauseTime = 1f;          // Tempo de pausa
    public float detectionRange = 5f;     // Distância para detecção do player
    public LayerMask playerLayer;         // Layer usada para detectar o player
    public LayerMask GroundLayer;         // Layer para detectar paredes

    private int direction = 1;
    private bool isMoving = true;
    private Transform playerTransform;

    void Start()
    {
        StartCoroutine(MovementRoutine());
        // Procura o objeto do player usando a tag "Player"
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
    }

    IEnumerator MovementRoutine()
    {
        while (true)
        {
            // Período de movimento normal
            isMoving = true;
            yield return new WaitForSeconds(moveTime);
            // Período de pausa
            isMoving = false;
            yield return new WaitForSeconds(pauseTime);
        }
    }

    void Update()
    {
        // Se o player existir, verifica se ele está no caminho com um raycast
        if (playerTransform != null)
        {
            Vector2 directionToPlayer = (playerTransform.position - transform.position).normalized;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, detectionRange, playerLayer);
            if (hit.collider != null && hit.collider.CompareTag("Player"))
            {
                // Se o player for detectado, o inimigo vai em sua direção
                transform.position = Vector2.MoveTowards(transform.position, playerTransform.position, speed * Time.deltaTime);
                return; // Pula o movimento padrão enquanto persegue o player
            }
        }

        // Movimento normal: andar e pausar
        if (isMoving)
        {
            transform.position += new Vector3(speed * Time.deltaTime * direction, 0, 0);
        }

        // Verifica colisão com parede usando raycast
        if (Physics2D.Raycast(transform.position, Vector2.right * direction, 0.1f, GroundLayer))
        {
            direction *= -1;
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
    }
}

