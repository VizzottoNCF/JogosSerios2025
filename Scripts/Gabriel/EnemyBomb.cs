using UnityEngine;

public class EnemyBomb : MonoBehaviour
{
    [Header("Patrulha")]
    public Transform pointA;
    public Transform pointB;
    public float speed = 2f;
    private Vector2 posA;
    private Vector2 posB;
    private Vector2 targetPos;
    private float fixedY;

    [Header("Bombas")]
    public GameObject bombPrefab;   // Prefab da bomba
    public Transform dropPoint;     // Ponto onde a bomba será solta
    public float dropInterval = 2f; // Tempo entre cada bomba
    private float lastDropTime = 0f;

    void Start()
    {
        // Mantém a posição Y fixa para patrulha
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
        DropBomb();
    }

    void Patrol()
    {
        // Movimento entre os pontos A e B
        float newX = Mathf.MoveTowards(transform.position.x, targetPos.x, speed * Time.deltaTime);
        transform.position = new Vector3(newX, fixedY, transform.position.z);

        // Quando chega perto do destino, inverte o alvo e a direção
        if (Mathf.Abs(transform.position.x - targetPos.x) < 0.1f)
        {
            targetPos = (targetPos == posB) ? posA : posB;
            Flip();
        }
    }

    void Flip()
    {
        // Inverte a escala X para mudar a direção
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    void DropBomb()
    {
        // Verifica se já passou o tempo necessário para soltar uma bomba
        if (Time.time - lastDropTime >= dropInterval)
        {
            if (bombPrefab != null && dropPoint != null)
            {
                GameObject bomb = Instantiate(bombPrefab, dropPoint.position, Quaternion.identity);
                bomb.GetComponent<Bomb>().SetOwner(gameObject); // Define o inimigo como dono da bomba
                lastDropTime = Time.time;
            }
        }
    }
}