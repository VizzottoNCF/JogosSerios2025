using UnityEngine;

public class PlataformController : MonoBehaviour
{
    [Header("Configurações de Movimento")]
    public float speed = 2f;       // Velocidade de movimento da plataforma
    public float minY = 0f;        // Limite inferior da posição Y
    public float maxY = 10f;       // Limite superior da posição Y

    private bool _playerOnPlatform = false;  // Indica se o jogador está sobre a plataforma

    // Detecta quando o jogador entra na plataforma (deve ter a tag "Player")
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            _playerOnPlatform = true;
        }
    }

    // Detecta quando o jogador sai da plataforma
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            _playerOnPlatform = false;
        }
    }

    private void Update()
    {
        // Permite o movimento apenas se o jogador estiver na plataforma
        if (_playerOnPlatform)
        {
            // Usa o input vertical (por exemplo, W/S ou setas para cima/baixo)
            float verticalInput = Input.GetAxis("Vertical");
            if (Mathf.Abs(verticalInput) > 0.1f)
            {
                Vector3 newPosition = transform.position;
                newPosition.y += verticalInput * speed * Time.deltaTime;
                newPosition.y = Mathf.Clamp(newPosition.y, minY, maxY);
                transform.position = newPosition;
            }
        }
    }
}