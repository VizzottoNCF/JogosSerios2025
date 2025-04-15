using UnityEngine;
using Unity.Cinemachine;
using System.Collections;
using Unity.VisualScripting;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;

    [SerializeField] private CinemachineCamera[] _allVirtualCameras;

    [Header("Controls for lerping the Y Damping during player jump/fall")]
    [SerializeField] private float _fallPanAmount = 0.25f;
    [SerializeField] private float _fallYPanTime = 0.35f;
    public float _fallSpeedYDampingChangeTreshold = -15f;

    public bool IsLerpingYDamping { get; private set; }
    public bool LerpedFromPlayerFalling { get; set; }

    private Coroutine _lerpYPanCoroutine;
    private Coroutine _panCameraCoroutine;

    private Vector2 _startingTrackedObjectOffset;

    private CinemachineCamera _currentCamera;
    private CinemachinePositionComposer _PositionComposer;
    private CinemachineConfiner2D _Confiner2D;

    [Header("Controls for Camera Y Offset lerping")]
    public bool IsLerpingYOffset { get; private set; }
    private Coroutine _lerpYOffsetCoroutine;
    private Coroutine _lerpYOffsetCoroutineBack;
    [SerializeField] private float _YOffsetTime = 0.35f;

    public CinemachineCamera rCC_GetCurrentCamera() { return _currentCamera; }

    private float _normYPanAmount;

    private void Awake()
    {
        // create singleton instance
        if (instance == null) { instance = this; }

        for (int i = 0; i < _allVirtualCameras.Length; i++)
        {
            if (_allVirtualCameras[i].enabled)
            {
                // set the current active camera
                _currentCamera = _allVirtualCameras[i];

                // set the position composer and confiner 2d
                _PositionComposer = _currentCamera.GetComponent<CinemachinePositionComposer>();
                _Confiner2D = _currentCamera.GetComponent<CinemachineConfiner2D>();
            }
        }

        // set the Y Damping amount so it's based on inspector
        _normYPanAmount = _PositionComposer.Damping.y;

        // set the starting position of the tracked object offset
        _startingTrackedObjectOffset = _PositionComposer.TargetOffset;
    }


    ///TODO: METHOD TO CHANGE CAMERA BOUNDARIES ONCE YOU GET INTO A NEW SCENE 
    /// NEVERMIND. SCRAPPED IDEA, IT'LL BE DIFFERENT PHASES.
    ///
    //private void Start()
    //{
    //    _Confiner2D.BoundingShape2D = GameObject.FindWithTag("CameraBoundary").GetComponent<CompositeCollider2D>();
    //}

    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if (collision.CompareTag("CameraBoundary"))
    //    {
    //        _Confiner2D.BoundingShape2D = collision.GetComponent<CompositeCollider2D>();
    //    }
    //}

    #region SWAP Z TRANSPOSITION COMPOSER
    public void rf_TurnCameraZOffsetAround()
    {
        _PositionComposer.TargetOffset.z *= -1f;
    }
    #endregion

    #region Lerp Y Offset
    public void rf_LerpYOffset(float variation)
    {
        _lerpYOffsetCoroutine = StartCoroutine(rIE_LerpYOffset(variation));
    }

    private IEnumerator rIE_LerpYOffset(float variation)
    {
        IsLerpingYOffset = true;

        // grab stating offset amount
        float startOffsetAmount = _PositionComposer.TargetOffset.y;
        float endOffsetAmount;
            endOffsetAmount = _PositionComposer.TargetOffset.y + variation;
            float elapsedTime = 0f;
            while (elapsedTime < _YOffsetTime)
            {
                elapsedTime += Time.deltaTime;

                float lerpedOffsetAmount = Mathf.Lerp(startOffsetAmount, endOffsetAmount, (elapsedTime / _YOffsetTime));
                _PositionComposer.TargetOffset.y = lerpedOffsetAmount;

                yield return null;
            }
    }

    public void rf_LerpYOffsetToNormal()
    {
        _lerpYOffsetCoroutineBack = StartCoroutine(rIE_LerpYOffsetToNormal());
    }
    private IEnumerator rIE_LerpYOffsetToNormal()
    {
        // grab stating offset amount
        float startOffsetAmount = _PositionComposer.TargetOffset.y;
        float endOffsetAmount = 3f;

        float elapsedTime = 0f;
        while (elapsedTime < _YOffsetTime)
        {
            elapsedTime += Time.deltaTime;

            float lerpedOffsetAmount = Mathf.Lerp(startOffsetAmount, endOffsetAmount, (elapsedTime / _YOffsetTime));
            _PositionComposer.TargetOffset.y = lerpedOffsetAmount;

            yield return null;
        }

        IsLerpingYOffset = false;
    }

    #endregion

    #region Lerp the Y Damping
    public void rf_LerpYDamping(bool isPlayerFalling)
    {
        _lerpYPanCoroutine = StartCoroutine(rIE_LerpYAction(isPlayerFalling));
    }

    private IEnumerator rIE_LerpYAction(bool isPlayerFalling)
    {
        IsLerpingYDamping = true;

        // grab the starting damping amount
        float startDampAmount = _PositionComposer.Damping.y;
        float endDampAmount = 0f;

        // determine the end damping amount
        if (isPlayerFalling)
        {
            endDampAmount = _fallPanAmount;
            LerpedFromPlayerFalling = true;
        }
        else
        {
            endDampAmount = _normYPanAmount;
        }

        // lerp the pan amount
        float elapsedTime = 0f;
        while (elapsedTime < _fallYPanTime)
        {
            elapsedTime += Time.deltaTime;

            float lerpedPanAmount = Mathf.Lerp(startDampAmount, endDampAmount, (elapsedTime / _fallYPanTime));
            _PositionComposer.Damping.y = lerpedPanAmount;

            yield return null;
        }
        IsLerpingYDamping = false;
    }
    #endregion

    #region Pan Camera

    public void rf_PanCameraOnContact(float panDistance, float panTime, re_PanDirection panDirection, bool panToStartingPos)
    {
        _panCameraCoroutine = StartCoroutine(rIE_PanCamera(panDistance, panTime, panDirection, panToStartingPos));
    }

    private IEnumerator rIE_PanCamera(float panDistance, float panTime, re_PanDirection panDirection, bool panToStartingPos)
    {
        Vector2 endPos = Vector2.zero;
        Vector2 startingPos = Vector2.zero;

        // handle pan from trigger
        if (!panToStartingPos)
        {
            // set the direction and distance
            switch (panDirection)
            {
                case re_PanDirection.Up:
                    endPos = Vector2.up;
                    break;
                case re_PanDirection.Down:
                    endPos = Vector2.down;
                    break;
                case re_PanDirection.Left:
                    endPos = Vector2.left;
                    break;
                case re_PanDirection.Right:
                    endPos = Vector2.right;
                    break;
                default: break;
            }

            endPos *= panDistance;
            startingPos = _startingTrackedObjectOffset;
            endPos += startingPos;
        }

        // handle the pan back to starting position
        else
        {
            startingPos = _PositionComposer.TargetOffset;
            endPos = _startingTrackedObjectOffset;
        }

        // handle the actual panning of the camera
        float elapsedTime = 0f;
        while (elapsedTime < panTime)
        {
            elapsedTime += Time.deltaTime;

            Vector3 panLerp = Vector3.Lerp(startingPos, endPos, (elapsedTime / panTime));
            _PositionComposer.TargetOffset = panLerp;

            yield return null;
        }
    }
    #endregion

    #region Swap Cameras

    public void rf_SwapCamera(CinemachineCamera cameraFromLeft, CinemachineCamera cameraFromRight, Vector2 triggerExitDirection)
    {
        // if current camera is on the left and trigger exit direcion was on the right
        if (_currentCamera == cameraFromLeft && triggerExitDirection.x > 0f)
        {
            // activate the new camera
            cameraFromRight.enabled = true;

            // deactivate old camera
            cameraFromLeft.enabled = false;

            // set the new camera as the current camera
            _currentCamera = cameraFromRight;

            // update position composer variable and confiner 2d
            _PositionComposer = _currentCamera.GetComponent<CinemachinePositionComposer>();
            _Confiner2D = _currentCamera.GetComponent<CinemachineConfiner2D>();
        }

        // if current camera is on the right and trigger exit direcion was on the left
        if (_currentCamera == cameraFromLeft && triggerExitDirection.x < 0f)
        {
            // activate the new camera
            cameraFromLeft.enabled = true;

            // deactivate old camera
            cameraFromRight.enabled = false;

            // set the new camera as the current camera
            _currentCamera = cameraFromLeft;

            // update position composer variable and confiner 2d
            _PositionComposer = _currentCamera.GetComponent<CinemachinePositionComposer>();
            _Confiner2D = _currentCamera.GetComponent<CinemachineConfiner2D>();
        }
    }
    #endregion
}
