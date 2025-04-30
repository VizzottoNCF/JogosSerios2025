using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class MovingPlatform : MonoBehaviour
{
    private Vector3 _lastPosition; // position vars used for player to move along moving platform
    public Vector3 DeltaPosition { get; private set; }


    private void Start()
    {
        // sets last position player on moving platform (wall hack)
        _lastPosition = transform.position;
    }

    private void FixedUpdate()
    {
        // delta position is added onto players position when players is on top of platform
        DeltaPosition = transform.position - _lastPosition;
        _lastPosition = transform.position;
    }
}
