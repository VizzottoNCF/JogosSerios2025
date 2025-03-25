using UnityEngine;

public class KeyllogerEnemy : MonoBehaviour
{
    public float speed = 2f;
    public float floatHeight = 1f;
    public float floatSpeed = 2f;
    public LayerMask GroundLayer;

    private int direction = 1;
    private float startY;

    void Start()
    {
        startY = transform.position.y;
    }

    void Update()
    {
        // Movimento horizontal
        transform.position += new Vector3(speed * Time.deltaTime * direction, 0, 0);

        // Movimento vertical (flutuação)
        //float newY = startY + Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        //transform.position = new Vector3(transform.position.x, newY, transform.position.z);

        // Verifica se colidiu com uma parede
        if (Physics2D.Raycast(transform.position, Vector2.right * direction, 0.1f, GroundLayer))
        {
            Flip();
        }
    }

    void Flip()
    {
        direction *= -1;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
}

