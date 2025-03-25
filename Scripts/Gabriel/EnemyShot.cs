using UnityEngine;

public class EnemyShot : MonoBehaviour
{
    [Header("Patrulha")]
    public Transform pointA;
    public Transform pointB;
    public float speed = 2f;
    private Vector2 posA;
    private Vector2 posB;
    private Vector2 targetPos;
    private float fixedY;

    [Header("Detec��o e Tiro")]
    public float rayDistance = 5f;
    public LayerMask playerLayer;      // Configure o layer do Player no Inspector
    public GameObject projectilePrefab; // Prefab do proj�til
    public Transform firePoint;         // Objeto filho que deve estar posicionado na frente do inimigo
    public float shootCooldown = 1f;    // Intervalo entre tiros
    private float lastShootTime = 0f;

    void Start()
    {
        // Mant�m a posi��o Y fixa para patrulha
        fixedY = transform.position.y;

        posA = pointA.position;
        posB = pointB.position;
        posA.y = fixedY;
        posB.y = fixedY;
        targetPos = posB;
    }

    void Update()
    {
        Patrol();
        DetectAndShoot();
    }

    void Patrol()
    {
        // Movimento entre os pontos A e B
        float newX = Mathf.MoveTowards(transform.position.x, targetPos.x, speed * Time.deltaTime);
        transform.position = new Vector3(newX, fixedY, transform.position.z);

        // Quando chega perto do destino, inverte o alvo e a dire��o
        if (Mathf.Abs(transform.position.x - targetPos.x) < 0.1f)
        {
            targetPos = (targetPos == posB) ? posA : posB;
            Flip();
        }
    }

    void Flip()
    {
        // Inverte a escala X para mudar a dire��o
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
        // Se necess�rio, voc� pode ajustar a posi��o do firePoint aqui
        // Exemplo: firePoint.localPosition = new Vector3(Mathf.Abs(firePoint.localPosition.x) * (scale.x > 0 ? 1 : -1), firePoint.localPosition.y, firePoint.localPosition.z);
    }

    void DetectAndShoot()
    {
        // Define a dire��o de disparo com base na escala:
        // Se localScale.x > 0, a frente � para a direita; caso contr�rio, para a esquerda.
        Vector2 shootDirection = transform.localScale.x > 0 ? Vector2.right : Vector2.left;

        // Realiza o raycast para detectar o player na dire��o da frente
        RaycastHit2D hit = Physics2D.Raycast(transform.position, shootDirection, rayDistance, playerLayer);
        if (hit.collider != null && hit.collider.CompareTag("Player"))
        {
            // Se o player estiver atr�s, inverte a dire��o antes de disparar
            bool playerInFront = (transform.localScale.x > 0 && hit.collider.transform.position.x > transform.position.x) ||
                                 (transform.localScale.x < 0 && hit.collider.transform.position.x < transform.position.x);

            if (!playerInFront)
            {
                Flip();
                return; // Aguarda o pr�ximo frame para tentar disparar novamente
            }

            // Se o cooldown permitir, dispara o proj�til
            if (Time.time - lastShootTime >= shootCooldown)
            {
                Shoot();
                lastShootTime = Time.time;
            }
        }
    }

    void Shoot()
    {
        if (projectilePrefab != null && firePoint != null)
        {
            // Instancia o proj�til na posi��o do firePoint
            GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

            // Define a dire��o do proj�til com base na escala do inimigo
            Vector2 shootDirection = transform.localScale.x > 0 ? Vector2.right : Vector2.left;
            Project projectileScript = proj.GetComponent<Project>();
            if (projectileScript != null)
            {
                projectileScript.direction = shootDirection;
            }

            // Impede que o proj�til colida com o inimigo que o disparou
            Collider2D enemyCollider = GetComponent<Collider2D>();
            Collider2D projCollider = proj.GetComponent<Collider2D>();
            if (enemyCollider != null && projCollider != null)
            {
                Physics2D.IgnoreCollision(enemyCollider, projCollider);
            }
        }
    }
}