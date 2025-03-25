using UnityEngine;

public class Bomb : MonoBehaviour
{
    public float explosionDelay = 2f; // Tempo at� explodir
    public float explosionRadius = 2f; // Raio da explos�o
    public float knockbackForce = 5f; // For�a do knockback
    private GameObject owner; // Dono da bomba (o inimigo que a soltou)

    void Start()
    {
        Invoke("Explode", explosionDelay); // Aguarda o tempo antes de explodir
    }

    public void SetOwner(GameObject enemy)
    {
        owner = enemy;
    }

    void Explode()
    {
        // Detecta todos os objetos dentro do raio da explos�o
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (Collider2D hit in colliders)
        {
            if (hit.gameObject == owner) continue; // Evita atingir o inimigo que soltou a bomba

            if (hit.CompareTag("Player"))
            {
                // Calcula a dire��o do knockback
                Vector2 knockbackDirection = (hit.transform.position - transform.position).normalized;
                Rigidbody2D rb = hit.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
                }
            }
        }

        // Destroi a bomba ap�s a explos�o
        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        // Desenha o raio da explos�o no editor
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
