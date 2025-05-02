using Unity.Cinemachine;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera _camera;
    //[SerializeField] private CinemachineVirtualCamera _camera;
    [SerializeField] private LineRenderer _lineRenderer;
    [SerializeField] private Transform _firePoint;

    [Header("Settings Variables")]
    [SerializeField] private bool IsPlayerLaser = false;
    [SerializeField] private bool IsTurnedOn = true;


    private void Start()
    {
        if (IsPlayerLaser || !IsTurnedOn) { rf_DisableLaser(); }
        else { rf_EnableLaser(); }
    }

    private void Update()
    {
        if (IsPlayerLaser)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                rf_EnableLaser();
            }

            if (Input.GetKey(KeyCode.Q))
            {
                rf_UpdateLaser();
            }

            if (Input.GetKeyUp(KeyCode.Q))
            {
                rf_DisableLaser();
            }
        }
    }

    // turns on line renderer
    private void rf_EnableLaser()
    {
        _lineRenderer.enabled = true;
    }

    // call to update position of line renderer
    private void rf_UpdateLaser()
    {

        // sets up laser trajectory
        var mousePos = (Vector3)_camera.ScreenToWorldPoint(Input.mousePosition);

        _lineRenderer.SetPosition(0, _firePoint.position);

        _lineRenderer.SetPosition(1, mousePos);

        // stops laser in case it collides with scenery
        Vector2 direction = mousePos - (Vector3)transform.position;
        RaycastHit2D hit = Physics2D.Raycast((Vector2)transform.position, direction.normalized, direction.magnitude);

        if (hit) { _lineRenderer.SetPosition(1, hit.point); }
    }

    // turns off line renderer
    private void rf_DisableLaser()
    {
        _lineRenderer.enabled = false;
    }
}
