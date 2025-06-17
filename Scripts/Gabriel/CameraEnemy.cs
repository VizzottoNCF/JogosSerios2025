using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CameraEnemy : MonoBehaviour
{

    [Header("References")]
    [SerializeField] private Laser _PlayerLaser;
    [SerializeField] private GameObject _healthCanvas;
    [SerializeField] private GameObject _camera;
    [SerializeField] private GameObject _dropdown;
    [SerializeField] private TMP_Dropdown _privacy;
    [SerializeField] private bool _cameraDestroyed = false;
    private bool _hasUpdated = false;
    public LayerMask GroundLayer;

    [Header("UI QUESTION STUFF")]
    [SerializeField] private List<Button> _selectableButtons = new List<Button>();
    [SerializeField] private string[] _optionText;
    [SerializeField] private List<GameObject> _originalButtons = new List<GameObject>();
    [SerializeField] private bool _buttonsVisible = false;

    private void Start()
    {
        // FOR LOOP WITH THE BUTTONS TO ASSIGN TEXT AND WHICH ONE IS CORRECT
        for (int i = 0; _selectableButtons.Count > 0; i++)
        {
            // RANDOMIZE _selectableButtons list, so that answers arer always on different places
            _selectableButtons = _selectableButtons.Randomize().ToList();

            // ASSIGN FUNCTION AND THEN TEXT
            // _selectableButtons[] is always index [0] because first of the array is always popped, thus making index [1] the new index [0]

            if (i == 0)
            {
                // _optionText[i = 0] IS ALWAYS THE CORRECT OPTION TO PICK
                _selectableButtons[0].gameObject.GetComponentInChildren<TMP_Text>().text = _optionText[i];
                _selectableButtons[0].onClick.AddListener(rf_DestroyCamera);

            }
            else
            {
                _selectableButtons[0].gameObject.GetComponentInChildren<TMP_Text>().text = _optionText[i];
                _selectableButtons[0].onClick.AddListener(rf_WrongChoice);
            }

            // remove button that was already assigned and resume for loop
            _selectableButtons.RemoveAt(0);
        }




        // gets dropdown option
        //_privacy = _dropdown.gameObject.GetComponent<Dropdown>();

        //complete scuff, may need further fixes
        //TODO: make less scuff
        _PlayerLaser = FindFirstObjectByType<Laser>(); // currently works as it is the sole laser in the scene, as soon as another is made, this is doomed
    }

    private void rf_WrongChoice()
    {
        AudioManager.Instance.rf_PlaySFX("WrongChoice");
    }
    private void rf_DestroyCamera()
    {
        if (!_cameraDestroyed)
        {

            // disables UI buttons for quiz
            for (int i = 0; i < _originalButtons.Count; i++)
            {
                _originalButtons[i].gameObject.SetActive(false);
            }


            AudioManager.Instance.rf_PlaySFX("CorrectChoice");
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


