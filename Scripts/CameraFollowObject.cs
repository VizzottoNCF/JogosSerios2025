using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class CameraFollowObject : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _playerTransform;

    [Header("Flip Rotation Stats")]
    [SerializeField] private float _flipYRotationTime = 0.5f;

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
    }

    private float rfl_DetermineEndRotation()
    {
        _isFacingRight = !_isFacingRight;
        if (_isFacingRight) { return 0f; }
        else { return -180f; }
    }
}
