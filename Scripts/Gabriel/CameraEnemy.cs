using TMPro;
using UnityEngine;

public class CameraEnemy : MonoBehaviour
{

    [Header("References")]
    [SerializeField] private Laser _PlayerLaser;
    [SerializeField] private GameObject _healthCanvas;
    [SerializeField] private GameObject _camera;
    [SerializeField] private GameObject _dropdown;
    [SerializeField] private TMP_Dropdown _privacy;
    private bool _cameraDestroyed = false;
    private bool _hasUpdated = false;
    public LayerMask GroundLayer;


    private void Start()
    {
        // gets dropdown option
        //_privacy = _dropdown.gameObject.GetComponent<Dropdown>();

        //complete scuff, may need further fixes
        //TODO: make less scuff
        _PlayerLaser = FindFirstObjectByType<Laser>(); // currently works as it is the sole laser in the scene, as soon as another is made, this is doomed
    }

    private void OnMouseDown()
    {
        if (!_cameraDestroyed)
        {
            // destroy camera and enable drop down
            Destroy(_camera);
            _dropdown.SetActive(true);

            _cameraDestroyed = true;
        }
    }


    private void Update()
    {

        // canvas logic
        if (_PlayerLaser.IsTurnedOn && HackingModeManager.Instance.IsHackingModeActive)
        {
            _healthCanvas.SetActive(true);
            if (_cameraDestroyed) { _dropdown.SetActive(true); }
        }
        else
        {
            _healthCanvas.SetActive(false);
            _dropdown.SetActive(false);
        }


        if (_privacy.options[_privacy.value].text == "Privada" && !_hasUpdated)
        {
            _hasUpdated = true;
            _privacy.interactable = false;
            gf_UpdatePrivateLeft();
        }

    }


    /// <summary>
    ///  Call when cookie is destroyed/deactivated
    /// </summary>
    private void gf_UpdatePrivateLeft()
    {
        // gets finish point script and reduces the ammount of cookies left to be collected
        FinishPoint finishPoint = GameObject.FindWithTag("Finish").GetComponent<FinishPoint>();

        // decreases camera left by 1
        finishPoint.LevelObjectives.PrivateImagesLeft--;

        // Try to get the Key component and call the function if it exists
        if (TryGetComponent<Key>(out Key goKey)) { goKey.rf_CompleteKey(); }
    }

}


