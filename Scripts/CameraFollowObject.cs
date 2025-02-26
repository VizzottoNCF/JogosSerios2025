using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class CameraFollowObject : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _playerTransform;

    [Header("Flip Rotation Stats")]
    [SerializeField] private float _flipYRotationTime = 0.5f;

    private Coroutine _turnCoroutine;

    private PlayerMovement _player;

    private bool _isFacingRight;

    private void Awake()
    {
        // get player facing direction
        _player = _playerTransform.gameObject.GetComponent<PlayerMovement>();
        _isFacingRight = _player.rf_CheckDirection();
    }

    private void Update()
    {
        // make cameraFollowObject follow the player's position
        transform.position = _playerTransform.position;
    }
    public void rf_CallTurn()
    {
        LeanTween.rotateY(gameObject, rfl_DetermineEndRotation(), _flipYRotationTime).setEaseInOutSine();

        // brute force method with coroutine
        // _turnCoroutine = StartCoroutine(rIE_FlipYLerp());
    }

    /*private IEnumerator rIE_FlipYLerp()
    {
        float startRotation = transform.localEulerAngles.y;
        float endRotationAmount = rfl_DetermineEndRotation();
        float yRotation = 0f;

        float elapsedTime = 0f;
        while(elapsedTime < _flipYRotationTime)
        {
            elapsedTime += Time.deltaTime;

            // lerp Y rotation
            yRotation = Mathf.Lerp(startRotation, endRotationAmount, (elapsedTime/_flipYRotationTime));
            transform.rotation = Quaternion.Euler(0f, yRotation, 0f);
            yield return null;
        }
    } */

    private float rfl_DetermineEndRotation()
    {
        _isFacingRight = !_isFacingRight;
        if (_isFacingRight) { return 0f; }
        else { return -180f; }
    }
}
