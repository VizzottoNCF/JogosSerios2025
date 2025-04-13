using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static PlayerInput PlayerInput;

    public static Vector2 Movement;
    public static bool jumpWasPressed;
    public static bool jumpIsHeld;
    public static bool jumpWasReleased;
    public static bool runIsHeld;
    public static bool DialogSkipPressed;

    private InputAction _moveAction;
    private InputAction _jumpAction;
    private InputAction _runAction;
    private InputAction _DialogSkipAction;

    private void Awake()
    {
        PlayerInput = GetComponent<PlayerInput>();

        _moveAction = PlayerInput.actions["Move"];
        _jumpAction = PlayerInput.actions["Jump"];
        _runAction = PlayerInput.actions["Run"];
        _DialogSkipAction = PlayerInput.actions["DialogSkip"];

    }

    private void Update()
    {
        Movement = _moveAction.ReadValue<Vector2>();

        jumpWasPressed = _jumpAction.WasPressedThisFrame();
        jumpIsHeld = _jumpAction.IsPressed();
        jumpWasReleased = _jumpAction.WasReleasedThisFrame();

        runIsHeld = _runAction.IsPressed();


        DialogSkipPressed = _DialogSkipAction.WasPressedThisFrame();
    }
}
