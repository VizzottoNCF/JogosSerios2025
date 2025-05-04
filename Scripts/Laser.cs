using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera _camera;
    [SerializeField] private LineRenderer _lineRenderer;
    [SerializeField] private Transform _firePoint;
    [SerializeField] private PlayerMovementStats moveStats;

    [Header("Settings Variables")]
    [SerializeField] private bool IsPlayerLaser = false;
    [SerializeField] public bool IsTurnedOn = true;
    [SerializeField] public float _LaserRange = 8f;
    public Vector3 LaserStart { get; private set; }
    public Vector3 LaserEnd { get; private set; }


    private void Start()
    {
        _camera = FindFirstObjectByType<Camera>();

        if (IsPlayerLaser || !IsTurnedOn) { rf_DisableLaser(); }
        else { rf_EnableLaser(); }
    }

    private void Update()
    {
        if (IsPlayerLaser && HackingModeManager.Instance.IsHackingModeActive)
        {
            //if (Input.GetKeyDown(KeyCode.Q)) { rf_EnableLaser(); }
            //if (Input.GetKey(KeyCode.Q)) { rf_UpdateLaser(); }
            if (Input.GetMouseButtonDown(0)) { rf_EnableLaser(); }
            if (Input.GetMouseButton(0)) { rf_UpdateLaser(); }
        }
        if (IsPlayerLaser)
        {
            //if (Input.GetKeyUp(KeyCode.Q)) { rf_DisableLaser(); }
            if (Input.GetMouseButtonUp(0)) { rf_DisableLaser(); }
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
        // Get mouse position in screen space and apply Z-depth
        Vector3 mouseScreenPos = Input.mousePosition;
        mouseScreenPos.z = Mathf.Abs(_camera.transform.position.z - _firePoint.position.z);
        Vector3 mouseWorldPos = _camera.ScreenToWorldPoint(mouseScreenPos);

        // Set laser start point
        _lineRenderer.SetPosition(0, _firePoint.position);

        // Direction from fire point to mouse
        Vector2 direction = (mouseWorldPos - _firePoint.position).normalized;

        // Calc distance to mouse, if over distance cap, reduce
        float distance = Vector2.Distance(_firePoint.position, mouseWorldPos);
        if (distance > _LaserRange) 
        { 
            // make sure that it stays in range
            distance = _LaserRange;
            mouseWorldPos = _firePoint.position + (Vector3)(direction * distance);
        }

        LaserStart = _firePoint.position;
        LaserEnd = mouseWorldPos;

        // Raycast to detect collision and stop laser 
        RaycastHit2D hit = Physics2D.Raycast(_firePoint.position, direction, distance, moveStats.GroundLayer);

        if (hit) { _lineRenderer.SetPosition(1, hit.point); } 
        else 
        { 
            _lineRenderer.SetPosition(1, mouseWorldPos); 
        }
    }

    // turns off line renderer
    private void rf_DisableLaser()
    {
        _lineRenderer.enabled = false;
    }
}
