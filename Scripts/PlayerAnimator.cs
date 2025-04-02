using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody2D rb;

    void Update()
    {

        float moveInput = InputManager.Movement.x;

        animator.SetFloat("HorizontalInput", Mathf.Abs(moveInput));


        // Jumping state
        animator.SetBool("IsJumping", rb.linearVelocity.y > 0.1f);
        animator.SetBool("IsFalling", rb.linearVelocity.y < -0.1f);
    }
}
