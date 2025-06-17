using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class BackdoorButton : MonoBehaviour
{
    [SerializeField] private BackdoorSpawner _spawner;
    private bool _activated = false;

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
                _selectableButtons[0].onClick.AddListener(rf_UpdateOS);

            }
            else
            {
                _selectableButtons[0].gameObject.GetComponentInChildren<TMP_Text>().text = _optionText[i];
                _selectableButtons[0].onClick.AddListener(rf_WrongChoice);
            }

            // remove button that was already assigned and resume for loop
            _selectableButtons.RemoveAt(0);
        }

        if (_buttonsVisible == false)
        {
            // disables UI buttons for quiz
            for (int i = 0; i < _originalButtons.Count; i++)
            {
                _originalButtons[i].gameObject.SetActive(false);
            }
        }
    }

    private void Update()
    {
        if (_buttonsVisible == true)
        {
            // reenables UI buttons for quiz
            for (int i = 0; i < _originalButtons.Count; i++)
            {
                _originalButtons[i].gameObject.SetActive(true);
            }
        }
        else
        {
            // disables UI buttons for quiz
            for (int i = 0; i < _originalButtons.Count; i++)
            {
                _originalButtons[i].gameObject.SetActive(false);
            }
        }

        // update button visibility
        _buttonsVisible = HackingModeManager.Instance.IsHackingModeActive; // get only bool

    }



    private void rf_WrongChoice()
    {
        AudioManager.Instance.rf_PlaySFX("WrongChoice");
    }


    private void rf_UpdateOS()
    {
        // when clicked, activates the button
        _activated = true;
        _spawner.rf_CloseBackdoor();
        AudioManager.Instance.rf_PlaySFX("CorrectChoice");

        // Try to get the Key component and call the function if it exists
        if (TryGetComponent<Key>(out Key goKey)) { goKey.rf_CompleteKey(); }

    }
}
