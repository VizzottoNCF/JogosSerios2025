using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    public static PlayerAnimator Instance;

    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody2D rb;


    private void Awake()
    {
        // singleton instance
        if (Instance == null) { Instance = this; }
    }
    private void Update()
    {

        float _horizontalMoveInput = InputManager.Movement.x;
        float _verticalMoveInput = InputManager.Movement.y;

        // system shift stats
        animator.SetBool("SystemShift", HackingModeManager.Instance.IsHackingModeActive);

        // idle stats
        animator.SetBool("IsStill", HackingModeManager.Instance.IsHackingModeActive  || (_horizontalMoveInput == 0 && GameController.Instance.IsPlayerGrounded));


        // is Grounded
        animator.SetBool("IsGrounded", transform.parent.gameObject.GetComponent<PlayerMovement>().rf_PlayerGrounded()); // rf_PlayerGround returns _isGrounded value
        print(transform.parent.gameObject.GetComponent<PlayerMovement>().rf_PlayerGrounded());
    }
}
