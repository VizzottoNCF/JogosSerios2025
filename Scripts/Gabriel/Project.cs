using UnityEngine;

public class Project : MonoBehaviour
{

    public float speed = 5f;
    public float lifetime = 3f;
    [HideInInspector]
    public Vector2 direction; // Será definida pelo inimigo no momento do disparo

    void Start()
    {
        // Garante que o projétil seja destruído mesmo se não atingir nada
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        // Move o projétil na direção definida
        transform.Translate(direction * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Se colidir com o player, destrói o projétil
        if (other.CompareTag("Player"))
        {
            // Aqui você pode adicionar lógica para causar dano ao player
            Destroy(gameObject);
        }
    }
}

