using UnityEngine;

public class PLAYERTEST : MonoBehaviour
{
    [Header("Movimentacao")]
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private bool isGrounded;

    [Header("Jump")]
    public float JumpHeight = 6.5f;
    [Range(1f, 1.1f)] public float JumpHeightCompensationFactor = 1.054f;
    public float TimeTillJumpApex = 0.35f;
    [Range(0.01f, 5f)] public float GravityOnReleaseMultiplier = 2f;
    public float MaxFallSpeed = 26f;
    [Range(1, 5)] public int NumberOfJumpsAllowed = 2;

    private float jumpForce;
    private float jumpTime;
    private bool isJumping;
    private int jumpsLeft;

    [Header("Verificacao de Chao")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        CalculateJumpForce();
        jumpsLeft = NumberOfJumpsAllowed;
    }

    void Update()
    {
        Move();
        Jump();
    }

    void Move()
    {
        float moveInput = Input.GetAxis("Horizontal"); // Move com A/D ou setas
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
    }

    void Jump()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // Caso o jogador esteja no chão e ainda tenha pulos disponíveis
        if (isGrounded)
        {
            jumpsLeft = NumberOfJumpsAllowed;
        }

        // Pular (ou pular novamente se houver saltos duplos)
        if (Input.GetButtonDown("Jump") && jumpsLeft > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            isJumping = true;
            jumpsLeft--;
            jumpTime = Time.time;
        }

        // Modificar a gravidade enquanto o jogador está no ar
        if (isJumping)
        {
            float timeSinceJump = Time.time - jumpTime;
            if (timeSinceJump > TimeTillJumpApex)
            {
                rb.gravityScale = GravityOnReleaseMultiplier;
            }

            // Impede que o jogador caia mais rápido que a velocidade máxima de queda
            if (rb.linearVelocity.y < -MaxFallSpeed)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, -MaxFallSpeed);
            }
        }
        else
        {
            rb.gravityScale = 1;
        }
    }

    void CalculateJumpForce()
    {
        // A fórmula para a força do pulo baseada no tempo até o pico
        jumpForce = JumpHeight / TimeTillJumpApex;
        rb.gravityScale = JumpHeightCompensationFactor;
    }
}
