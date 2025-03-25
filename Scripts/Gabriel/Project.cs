using UnityEngine;

public class Project : MonoBehaviour
{

    public float speed = 5f;
    public float lifetime = 3f;
    [HideInInspector]
    public Vector2 direction; // Ser� definida pelo inimigo no momento do disparo

    void Start()
    {
        // Garante que o proj�til seja destru�do mesmo se n�o atingir nada
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        // Move o proj�til na dire��o definida
        transform.Translate(direction * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Se colidir com o player, destr�i o proj�til
        if (other.CompareTag("Player"))
        {
            // Aqui voc� pode adicionar l�gica para causar dano ao player
            Destroy(gameObject);
        }
    }
}

