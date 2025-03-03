using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    #region vars
    [Header("References")]
    public PlayerMovementStats moveStats;
    [SerializeField] private Collider2D _feetCollider;
    [SerializeField] private Collider2D _bodyCollider;

    private Rigidbody2D _rb;

    [Header("Movement Variables")]
    private Vector2 _moveVelocity;
    private bool _isFacingRight = true;

    [Header("Collision Check Variables")]
    private RaycastHit2D _groundHit;
    private RaycastHit2D _headHit;
    private RaycastHit2D _edgeDetectionLeft;
    private RaycastHit2D _edgeDetectionRight;
    private RaycastHit2D _bodyDetectionLeft;
    private RaycastHit2D _bodyDetectionRight;
    private bool _isGrounded;
    private bool _bumpedHead;

    [Header("Jump Variables")]
    public float VerticalVelocity { get; private set; }
    private bool _isJumping;
    private bool _isFalling;
    private bool _isFastFalling;
    private float _fastFallTime;
    private float _fastFallReleaseSpeed;
    private int _numberOfJumpsUsed;

    [Header("Jump Apex Variables")]
    private float _apexPoint;
    private float _timePastApexThreshold;
    private bool _isPastApexThreshold;

    [Header("Jump Buffer Variables")]
    private float _jumpBufferTimer;
    private bool _jumpReleasedDuringBuffer;

    [Header("Coyote Time Variables")]
    private float _coyoteTimer;

    [Header("Camera Tracking Variables")]
    [SerializeField] private GameObject _cameraFollowGO;
    private CameraFollowObject _cameraFollowObject;
    private float _fallSpeedYDampingChangeThreshold;
    private float _upHeldTimer = 0f;
    private float _downHeldTimer = 0f;

    #endregion
    private void Start()
    {
        _isFacingRight = true;

        _rb = GetComponent<Rigidbody2D>();

        _cameraFollowObject = _cameraFollowGO.GetComponent<CameraFollowObject>();
        _fallSpeedYDampingChangeThreshold = CameraManager.instance._fallSpeedYDampingChangeTreshold;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.L)) { Time.timeScale = 0.2f; } else { Time.timeScale = 1f; }

        if (Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.S)) { _upHeldTimer += Time.deltaTime; } else { _upHeldTimer = 0f; }
        if (Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.W)) { _downHeldTimer += Time.deltaTime; } else { _downHeldTimer = 0f; }

        if (_upHeldTimer > 0.45f && !CameraManager.instance.IsLerpingYOffset) { CameraManager.instance.rf_LerpYOffset(2f); }
        if (_downHeldTimer > 0.45f && !CameraManager.instance.IsLerpingYOffset) { CameraManager.instance.rf_LerpYOffset(-3f); }

        if ((Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.W)) && CameraManager.instance.IsLerpingYOffset) { CameraManager.instance.rf_LerpYOffsetToNormal(); }

        rf_JumpChecks();
        rf_CountTimers();

        if (CameraManager.instance != null )
        {
            // camera adjustment if we are falling past a certain speed threshold
            if (_rb.linearVelocity.y < _fallSpeedYDampingChangeThreshold && !CameraManager.instance.IsLerpingYDamping && !CameraManager.instance.LerpedFromPlayerFalling && !CameraManager.instance.IsLerpingYOffset)
            {
                CameraManager.instance.rf_LerpYDamping(true);
                CameraManager.instance.rf_LerpYOffset(-1f);
            }

            // camera adjustment if we are standing still or moving up
            if (_rb.linearVelocity.y >= 0f && !CameraManager.instance.IsLerpingYDamping && CameraManager.instance.LerpedFromPlayerFalling && CameraManager.instance.IsLerpingYOffset)
            {
                // reset so it can be called again
                CameraManager.instance.LerpedFromPlayerFalling = false;

                CameraManager.instance.rf_LerpYDamping(false);
                CameraManager.instance.rf_LerpYOffsetToNormal();
            }
        } else { Debug.LogWarning("Camera Manager is Null");  }
    }

    private void FixedUpdate()
    {
        rf_CollisionChecks();
        rf_Jump();
        rf_LedgeAssist();

        if (_isGrounded)
        {
            rf_Move(moveStats.groundAcceleration, moveStats.groundDeceleration, InputManager.Movement);
        }
        else
        {
            rf_Move(moveStats.airAcceleration, moveStats.airDeceleration, InputManager.Movement);
        }
    }


    #region Movement

    /// <summary>
    /// Function called to make the player move around
    /// </summary>
    /// <param name="acceleration"> Rate of which the character accelerates.</param>
    /// <param name="deceleration"> Rate of which the character decelerates.</param>
    /// <param name="moveInput"> The Input Direction being given by the player.</param>
    private void rf_Move(float acceleration, float deceleration, Vector2 moveInput)
    {
        if (moveInput != Vector2.zero)
        {
            // first check if player needs to turn around
            rf_TurnCheck(moveInput);

            Vector2 targetVelocity = Vector2.zero;
            if (InputManager.runIsHeld) { targetVelocity = new Vector2(moveInput.x, 0f) * moveStats.maxRunSpeed; }
            else { targetVelocity = new Vector2(moveInput.x, 0f) * moveStats.maxWalkSpeed; }

            // lerp move velocity from current to target velocity and then apply to rigidbody
            _moveVelocity = Vector2.Lerp(_moveVelocity, targetVelocity, acceleration * Time.fixedDeltaTime);
            _rb.linearVelocity = new Vector2(_moveVelocity.x, _rb.linearVelocity.y);
        }

        // if there is no movement input, change move velocity to nothing
        else if (moveInput == Vector2.zero)
        {
            _moveVelocity = Vector2.Lerp(_moveVelocity, Vector2.zero, deceleration * Time.fixedDeltaTime);
            _rb.linearVelocity = new Vector2(_moveVelocity.x, _rb.linearVelocity.y);
        }

    }

    private void rf_TurnCheck(Vector2 moveInput)
    {
        if (_isFacingRight && moveInput.x < 0) { rf_Turn(false); }
        else if (!_isFacingRight && moveInput.x > 0) { rf_Turn(true); }
    }

    private void rf_Turn(bool turnRight)
    {
        if (turnRight)
        {
            // flips player to the right side
            _isFacingRight = true;
            Vector3 rotator = new Vector3(transform.rotation.x, 0f, transform.rotation.z);
            transform.rotation = Quaternion.Euler(rotator);

            // turn the camera follow object
            _cameraFollowObject.rf_CallTurn();
        }
        else
        {
            // flips player to the left side
            _isFacingRight = false;
            Vector3 rotator = new Vector3(transform.rotation.x, -180f, transform.rotation.z);
            transform.rotation = Quaternion.Euler(rotator);

            // turn the camera follow object
            _cameraFollowObject.rf_CallTurn();
        }
    }

    public bool rf_CheckDirection() { return _isFacingRight; }

    #endregion

    #region Jump
    /// <summary> What happens when the jump button is pressed. </summary>
    private void rf_JumpChecks()
    {
        // WHEN JUMP BUTTON IS PRESSED
        if (InputManager.jumpWasPressed)
        {
            _jumpBufferTimer = moveStats.JumpBufferTime;
            _jumpReleasedDuringBuffer = false;
        }

        // WHEN JUMP BUTTON IS RELEASED
        if (InputManager.jumpWasReleased)
        {
            if (_jumpBufferTimer > 0f)
            {
                _jumpReleasedDuringBuffer = true;
            }

            if (_isJumping && VerticalVelocity > 0f)
            {
                if (_isPastApexThreshold)
                {
                    _isPastApexThreshold = false;
                    _isFastFalling = true;
                    _fastFallTime = moveStats.TimeForUpwardsCancel;
                    VerticalVelocity = 0f;
                }
                else
                {
                    _isFastFalling = true;
                    _fastFallReleaseSpeed = VerticalVelocity;
                }
            }
        }

        // INITIATE JUMP WITH JUMP BUFFERING AND COYOTE TIME
        if (_jumpBufferTimer > 0f && !_isJumping && (_isGrounded || _coyoteTimer > 0f))
        {
            rf_InitiateJump(1);

            if (_jumpReleasedDuringBuffer)
            {
                _isFastFalling = true;
                _fastFallReleaseSpeed = VerticalVelocity;
            }
        }


        // DOUBLE JUMP (AND MORE)
        else if (_jumpBufferTimer > 0f && _isJumping && _numberOfJumpsUsed < moveStats.NumberOfJumpsAllowed)
        {
            _isFastFalling = false;
            rf_InitiateJump(1);
        }

        // AIR JUMP AFTER COYOTE TIME LAPSED (take off an extra jump so we don't get a bonus jump)
        else if (_jumpBufferTimer > 0f && _isFalling && _numberOfJumpsUsed < moveStats.NumberOfJumpsAllowed - 1)
        {
            rf_InitiateJump(2);
            _isFastFalling = false;
        }

        // LANDED
        if ((_isJumping || _isFalling) && _isGrounded && VerticalVelocity <= 0f)
        {
            _isJumping = false;
            _isFalling = false;
            _isFastFalling = false;
            _isPastApexThreshold = false;
            _numberOfJumpsUsed = 0;

            VerticalVelocity = Physics2D.gravity.y;
        }
    }

    private void rf_InitiateJump(int numberOfJumpsUsed)
    {
        if (!_isJumping)
        {
            _isJumping = true;
        }

        _jumpBufferTimer = 0f;
        _numberOfJumpsUsed += numberOfJumpsUsed;
        VerticalVelocity = moveStats.InitialJumpVelocity;
    }
    /// <summary>
    /// Jump logic 
    /// </summary>
    private void rf_Jump()
    {
        // APPLY GRAVITY WHILE JUMPING
        if (_isJumping)
        {
            // CHECK FOR HEAD BUMP
            if (_bumpedHead)
            {
                _isFastFalling = true;
            }

            // GRAVITY ON ASCENDING
            if (VerticalVelocity >= 0f)
            {
                // APEX CONTROLS
                _apexPoint = Mathf.InverseLerp(moveStats.InitialJumpVelocity, 0f, VerticalVelocity);

                if (_apexPoint > moveStats.ApexThreshold)
                {
                    if (!_isPastApexThreshold)
                    {
                        _isPastApexThreshold = true;
                        _timePastApexThreshold = 0f;
                    }

                    if (_isPastApexThreshold)
                    {
                        _timePastApexThreshold += Time.fixedDeltaTime;
                        if (_timePastApexThreshold < moveStats.ApexHangTime)
                        {
                            VerticalVelocity = 0f;
                        }
                        else
                        {
                            VerticalVelocity = -0.01f;
                        }
                    }
                }
                // GRAVITY ON ASCENDING, BUT NOT PAST APEX THRESHOLD
                else
                {
                    VerticalVelocity += moveStats.Gravity * Time.fixedDeltaTime;
                    if (_isPastApexThreshold)
                    {
                        _isPastApexThreshold = false;
                    }
                }
            }
            // GRAVITY ON DESCENDING
            else if (!_isFastFalling)
            {
                VerticalVelocity += moveStats.Gravity * moveStats.GravityOnReleaseMultiplier * Time.fixedDeltaTime;
            }

            else if (VerticalVelocity < 0f)
            {
                if (!_isFalling)
                {
                    _isFalling = true;
                }
            }
        }

        // JUMP CUT
        if (_isFastFalling)
        {
            if (_fastFallTime >= moveStats.TimeForUpwardsCancel)
            {
                VerticalVelocity += moveStats.Gravity * moveStats.GravityOnReleaseMultiplier * Time.fixedDeltaTime;
            }
            else if (_fastFallTime < moveStats.TimeForUpwardsCancel)
            {
                VerticalVelocity = Mathf.Lerp(_fastFallReleaseSpeed, 0f, (_fastFallTime / moveStats.TimeForUpwardsCancel));
            }

            _fastFallTime += Time.fixedDeltaTime;
        }

        // NORMAL GRAVITY WHILE FALLING 
        if (!_isGrounded && !_isJumping)
        {
            if (_isFalling)
            {
                _isFalling = true;
            }

            VerticalVelocity += moveStats.Gravity * Time.fixedDeltaTime;
        }

        // CLAMP FALL SPEED APPLY TO THE RIGID BODY VELOCITY
        VerticalVelocity = Mathf.Clamp(VerticalVelocity, -moveStats.MaxFallSpeed, 50f);

        _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, VerticalVelocity);
    }

    #endregion

    #region Timers
    private void rf_CountTimers()
    {
        _jumpBufferTimer -= Time.deltaTime;
        if (!_isGrounded) { _coyoteTimer -= Time.deltaTime; }
        else { _coyoteTimer = moveStats.JumpCoyoteTime; }
    }
    #endregion

    #region Collision Checks

    private void rf_IsGrounded()
    {
        Vector2 boxCastOrigin = new Vector2(_feetCollider.bounds.center.x, _feetCollider.bounds.min.y);
        Vector2 boxCastSize = new Vector2(_feetCollider.bounds.size.x, moveStats.GroundDetectionRayLength);

        _groundHit = Physics2D.BoxCast(boxCastOrigin, boxCastSize, 0f, Vector2.down, moveStats.GroundDetectionRayLength, moveStats.GroundLayer);
        if (_groundHit.collider != null) { _isGrounded = true; }
        else { _isGrounded = false; }

        #region Debug Visualisation
        if (moveStats.DebugShowIsGroundedBox)
        {
            Color rayColor;
            if (_isGrounded) { rayColor = Color.blue; }
            else { rayColor = Color.magenta; }

            Debug.DrawRay(new Vector2(boxCastOrigin.x - boxCastSize.x / 2, boxCastOrigin.y), Vector2.down * moveStats.GroundDetectionRayLength, rayColor);
            Debug.DrawRay(new Vector2(boxCastOrigin.x + boxCastSize.x / 2, boxCastOrigin.y), Vector2.down * moveStats.GroundDetectionRayLength, rayColor);
            Debug.DrawRay(new Vector2(boxCastOrigin.x - boxCastSize.x / 2, boxCastOrigin.y - moveStats.GroundDetectionRayLength), Vector2.right * boxCastSize.x, rayColor);
        }
        #endregion
    }

    private void rf_BumpedHead()
    {
        // Makes a 80% head-width hitbox to detect head collisions
        Vector2 boxCastOrigin = new Vector2(_bodyCollider.bounds.center.x, _bodyCollider.bounds.max.y);
        Vector2 boxCastSize = new Vector2(_bodyCollider.bounds.size.x * moveStats.HeadWidth * 0.8f, moveStats.HeadDetectionRayLength);

        // Body cast hitboxes to prevent edge detection happening against straight walls
        Vector2 bodyCastOriginLeft = new Vector2(_bodyCollider.bounds.min.x, _bodyCollider.bounds.max.y * 0.8f);
        Vector2 bodyCastOriginRight = new Vector2(_bodyCollider.bounds.max.x, _bodyCollider.bounds.max.y * 0.8f);


        // Edge detection hitboxes (10% of the head width on each side)
        Vector2 edgeCastSize = new Vector2(_bodyCollider.bounds.size.x * moveStats.HeadWidth * 0.1f, moveStats.HeadDetectionRayLength);

        // Origins for left and right edge detection
        Vector2 edgeCastOriginLeft = new Vector2(_bodyCollider.bounds.min.x, _bodyCollider.bounds.max.y);
        Vector2 edgeCastOriginRight = new Vector2(_bodyCollider.bounds.max.x, _bodyCollider.bounds.max.y);


        // Perform edge detection box casts
        _edgeDetectionLeft = Physics2D.BoxCast(edgeCastOriginLeft, edgeCastSize, 0f, Vector2.up, moveStats.HeadDetectionRayLength, moveStats.GroundLayer);
        _edgeDetectionRight = Physics2D.BoxCast(edgeCastOriginRight, edgeCastSize, 0f, Vector2.up, moveStats.HeadDetectionRayLength, moveStats.GroundLayer);

        _bodyDetectionLeft = Physics2D.BoxCast(bodyCastOriginLeft, edgeCastSize, 0f, Vector2.up, moveStats.HeadDetectionRayLength, moveStats.GroundLayer);
        _bodyDetectionRight = Physics2D.BoxCast(bodyCastOriginRight, edgeCastSize, 0f, Vector2.up, moveStats.HeadDetectionRayLength, moveStats.GroundLayer);

        // Perform head collision box cast
        _headHit = Physics2D.BoxCast(boxCastOrigin, boxCastSize, 0f, Vector2.up, moveStats.HeadDetectionRayLength, moveStats.GroundLayer);

        // Edge correction logic
        if (((_edgeDetectionLeft && !_bodyDetectionLeft) || (_edgeDetectionRight && !_bodyDetectionRight) && !_headHit && !_isGrounded))
        {
            // Shift the player to the side to avoid the edge collision
            rf_EdgeCorrection();
            _bumpedHead = false; // Prevent head bump flag from being set
        }
        if (_headHit.collider != null) { _bumpedHead = true; } // regular head bump
        else { _bumpedHead = false; } // no collision


        #region Debug Visualisation
        if (moveStats.DebugShowHeadBumpBox)
        {

            // Draw edge detection box casts
            Debug.DrawRay(edgeCastOriginLeft, Vector2.up * moveStats.HeadDetectionRayLength, Color.yellow);
            Debug.DrawRay(edgeCastOriginRight, Vector2.up * moveStats.HeadDetectionRayLength, Color.yellow);
            Debug.DrawRay(bodyCastOriginLeft, Vector2.up * moveStats.HeadDetectionRayLength, Color.yellow);
            Debug.DrawRay(bodyCastOriginRight, Vector2.up * moveStats.HeadDetectionRayLength, Color.yellow);

            // Draw head collision box cast
            Debug.DrawRay(boxCastOrigin, Vector2.up * moveStats.HeadDetectionRayLength, Color.red);


            float headWidth = moveStats.HeadWidth;

            Color rayColor;
            if (_bumpedHead) { rayColor = Color.green; }
            else { rayColor = Color.red; }

            Debug.DrawRay(new Vector2(boxCastOrigin.x - ((boxCastSize.x / 2) * headWidth), boxCastOrigin.y), Vector2.up * moveStats.HeadDetectionRayLength, rayColor);
            Debug.DrawRay(new Vector2(boxCastOrigin.x + ((boxCastSize.x / 2) * headWidth), boxCastOrigin.y), Vector2.up * moveStats.HeadDetectionRayLength, rayColor);
            Debug.DrawRay(new Vector2(boxCastOrigin.x - boxCastSize.x / 2 * headWidth, _bodyCollider.bounds.max.y + moveStats.HeadDetectionRayLength), Vector2.right * boxCastSize.x * headWidth, rayColor);
        }
        #endregion
    }

    private void rf_EdgeCorrection()
    {
        // Determine which edge is colliding
        if (_edgeDetectionLeft)
        {
            // Shift the player slightly to the right
            transform.position += Vector3.right * moveStats.EdgeCorrectionDistance;
        }
        else if (_edgeDetectionRight)
        {
            // Shift the player slightly to the left
            transform.position += Vector3.left * moveStats.EdgeCorrectionDistance;
        }
    }

    private void rf_LedgeAssist()
    {

        // Check if the player is in the air and moving upward (mid-jump)
        if (!_isGrounded && VerticalVelocity > -0.1f)
        {
            // Calculate the origin for the ledge detection raycast
            Vector2 rayOrigin = new Vector2(_feetCollider.bounds.center.x, _feetCollider.bounds.max.y);

            // Perform a raycast upward to detect platforms within the vertical threshold
            RaycastHit2D ledgeHitRight = Physics2D.Raycast(rayOrigin, Vector2.right, moveStats.LedgeSnapHorizontalRange, moveStats.GroundLayer);
            RaycastHit2D ledgeHitLeft = Physics2D.Raycast(rayOrigin, Vector2.left, moveStats.LedgeSnapHorizontalRange, moveStats.GroundLayer);
            Debug.DrawRay(rayOrigin, Vector2.right * moveStats.LedgeSnapHorizontalRange, Color.red, 2f);
            Debug.DrawRay(rayOrigin, Vector2.left * moveStats.LedgeSnapHorizontalRange, Color.red, 2f);


            // If a platform is detected within the vertical threshold 
            if (ledgeHitRight.collider != null)
            {
                // Check if the player is horizontally aligned with the platform
                float platformHeight = ledgeHitRight.collider.bounds.max.y;
                float playerFeetHeight = _feetCollider.bounds.min.y;


                // Check if the player is within the vertical threshold of the platform
                if (playerFeetHeight > platformHeight - moveStats.LedgeSnapVerticalThreshold)
                {
                    // Snap the player to the platform
                    float snapYPosition = platformHeight - playerFeetHeight;

                    transform.position = new Vector2(transform.position.x, transform.position.y + snapYPosition);


                    // Reset vertical velocity and set grounded state
                    VerticalVelocity = 0f;
                    _isGrounded = true;
                    _isJumping = false;
                    _isFalling = false;
                }
            }
            // left side
            if (ledgeHitLeft.collider != null)
            {
                // Check if the player is horizontally aligned with the platform
                float platformHeight = ledgeHitLeft.collider.bounds.max.y;
                float playerFeetHeight = _feetCollider.bounds.min.y;


                // Check if the player is within the vertical threshold of the platform
                if (playerFeetHeight > platformHeight - moveStats.LedgeSnapVerticalThreshold)
                {
                    // Snap the player to the platform
                    float snapYPosition = platformHeight - playerFeetHeight;

                    transform.position = new Vector2(transform.position.x, transform.position.y + snapYPosition);


                    // Reset vertical velocity and set grounded state
                    //VerticalVelocity = 0f;
                    //_isGrounded = true;
                    //_isJumping = false;
                    //_isFalling = false;
                }
            }
        }
    }

    private void rf_CollisionChecks()
    {
        rf_IsGrounded();
        rf_BumpedHead();
    }


    #endregion
}