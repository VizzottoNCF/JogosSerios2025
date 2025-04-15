using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;
using System.Collections;
using System.IO.Pipes;
using System;

public class DialogManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject _MainSpeakerPanel;
    [SerializeField] private TMP_Text _MainSpeakerText;
    [SerializeField] private GameObject _OtherSpeakerPanel;
    [SerializeField] private TMP_Text _OtherSpeakerText;
    [SerializeField] private TMP_Text _DialogText;
    [SerializeField] private Image _mainPortraitImage;
    [SerializeField] private Image _otherPortraitImage;
    [SerializeField] private Canvas _dialogCanvas;

    [Header("Dialog Info")]
    [SerializeField] private float _textSpeed = 0.015f;
    private int _index = 0;

    [Header("Dialog Content")]
    [SerializeField] private string[] _speaker;
    [SerializeField][TextArea] private string[] _lines = new string[0];
    [SerializeField] private Sprite[] _portrait;

    private float _skipTimer = 0f;


    public static DialogManager Instance;

    private Coroutine _currentRoutine;

    private bool _IsDialogHappening = false;


    private void Awake()
    {
        // singleton instance
        if (Instance == null) { Instance = this; }

        // make sure it awakes with no text
        _lines[_index] = string.Empty;
    }

    private void Update()
    {
        // if there is NO dialog, disable canvas
        if (!_IsDialogHappening) { _dialogCanvas.gameObject.SetActive(false); }

        _skipTimer += Time.deltaTime;
        /// ------------------ TEXT BOX
        if (InputManager.DialogSkipPressed && _IsDialogHappening && _skipTimer >= 0.5f)
        {
            _skipTimer = 0f;
            // if text is not finished, jumps to finished state
            
            if (_DialogText.text != _lines[_index])
            {
                // stop coroutine
                if (_currentRoutine != null) { StopCoroutine(_currentRoutine); }
                _DialogText.text = _lines[_index];
            }
            // go to next line
            else if (_DialogText.text == _lines[_index])
            {
                _DialogText.text = string.Empty;
                _index++;

                if (_index >= _lines.Length)
                {
                    // if its the last line of text, deactivate object
                    _dialogCanvas.gameObject.SetActive(false);
                    _IsDialogHappening = false;
                    GameController.Instance.CanPlayerMove = true;
                }
                else { rf_NextLine(_speaker, _portrait); }
            }
        }
    }
    /// <summary>
    ///  Starts a dialog canvas
    /// </summary>
    /// <param name="speaker"> The person speaking the equivalent line array. </param>
    /// <param name="dialog"> The text displayed on screen. </param>
    /// <param name="portrait"> The image displayed on screen </param>
    public void rf_StartDialog(string[] speaker, string[] dialog, Sprite[] portrait)
    {
        // reset index and enables dialog canvas
        _index = 0;
        _dialogCanvas.gameObject.SetActive(true);
        GameController.Instance.CanPlayerMove = false;
        _IsDialogHappening = true;

        // clear arrays and resize it with new ones
        Array.Clear(_lines, 0, _lines.Length);
        Array.Clear(_speaker, 0, _speaker.Length);
        Array.Clear(_portrait, 0, _portrait.Length);

        _lines = new string[dialog.Length];
        _speaker = new string[speaker.Length];
        _portrait = new Sprite[portrait.Length];

        for (int i = 0; i < dialog.Length; i++)
        {
            _speaker[i] = speaker[i];
            _lines[i] = dialog[i];
            _portrait[i] = portrait[i];
        }

        // erases text already written
        _DialogText.text = string.Empty;
        rf_NextLine(speaker, portrait);
    }

    private void rf_NextLine(string[] speaker, Sprite[] portrait)
    {
        // moves to next text line
        if (_index < _lines.Length)
        {
            // arrange textbox shape
            if (speaker[_index] == "PIP")
            {
                // activate main portrait + set its image + speaker name
                _MainSpeakerPanel.gameObject.SetActive(true);
                _mainPortraitImage.gameObject.SetActive(true);
                _mainPortraitImage.sprite = portrait[_index];
                _MainSpeakerText.text = speaker[_index];

                // setup rect transform
                RectTransform _rect = _DialogText.GetComponent<RectTransform>();
                _rect.offsetMin = new Vector2(420, _rect.offsetMin.y);
                _rect.offsetMax = new Vector2(-20, _rect.offsetMax.y);

                // deactivate other portrait
                _otherPortraitImage.gameObject.SetActive(false);
                _OtherSpeakerPanel.gameObject.SetActive(false);
            }
            else
            {
                // activate other portrait + set its image + speaker name
                _otherPortraitImage.gameObject.SetActive(true);
                _OtherSpeakerPanel.gameObject.SetActive(true);
                _otherPortraitImage.sprite = portrait[_index];
                _OtherSpeakerText.text = speaker[_index];

                // setup rect transform
                RectTransform _rect = _DialogText.GetComponent<RectTransform>();
                _rect.offsetMin = new Vector2(20, _rect.offsetMin.y);
                _rect.offsetMax = new Vector2(-420, _rect.offsetMax.y);

                // deactivate main portrait
                _MainSpeakerPanel.gameObject.SetActive(false);
                _mainPortraitImage.gameObject.SetActive(false);
            }

            // if the text is blanked out by the page skip, it will start writing the next
            if (_DialogText.text == "")
            {
                if (_currentRoutine != null) { StopCoroutine(_currentRoutine); }
                _currentRoutine = StartCoroutine(rIE_TypeLine(_index));
            }

        }
        else
        {
            // if its the last line of text, deactivate object
            _dialogCanvas.gameObject.SetActive(false);
            _IsDialogHappening = false;
            GameController.Instance.CanPlayerMove = true;
        }
    }

    private IEnumerator rIE_TypeLine(int index)
    {

        // goes through characters in the lines and types them individualy
        foreach (char c in _lines[index].ToCharArray())
        {
            _DialogText.text += c;

            yield return new WaitForSeconds(_textSpeed);

            if (_DialogText.text == _lines[index].ToString()) { StopCoroutine(_currentRoutine); }
        }
    }
}
