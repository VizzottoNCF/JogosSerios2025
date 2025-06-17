using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Unity.Collections.AllocatorManager;

public class CookieEnemy : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private float _timer = 0f;
    [SerializeField] private float _timeToDie = 0.85f;
    [SerializeField] private Laser _PlayerLaser;
    [SerializeField] private GameObject _healthCanvas;
    [SerializeField] private Slider _hackSlider;
    [SerializeField] private bool _buttonsVisible = false;
    private float _dissolveAmount = 0f;
    private const float COMPLETE_DISSOLVE_AMOUNT = 1.1f;
    public float speed = 2f;
    public float floatHeight = 1f;
    public float floatSpeed = 2f;
    public LayerMask GroundLayer;
    private int direction = 1;
    private float startY;
    private bool _canMove = true;

    [Header("UI QUESTION STUFF")]
    [SerializeField][Range(0, 1)] private int _questionType = 0;
    [SerializeField] private TMP_Text _question;
    [SerializeField] private List<Button> _selectableButtons = new List<Button>();
    [SerializeField] private GameObject _questionGO;
    [SerializeField] private string[] _optionText;
    [SerializeField] private string[] _optionText2;
    [SerializeField] private List<GameObject> _originalButtons = new List<GameObject>();

    private void Start()
    {
        // SET QUESTION TYPE
        _questionType = Random.Range(0, 2);
        if (_questionType == 0 ) { _question.text = "Como se livrar de cookies no seu computador"; }
        else { _question.text = "Como diminuir a incidência de cookies ao visitar sites"; }

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
                if (_questionType == 0) { _selectableButtons[0].gameObject.GetComponentInChildren<TMP_Text>().text = _optionText[i]; }
                else { _selectableButtons[0].gameObject.GetComponentInChildren<TMP_Text>().text = _optionText2[i]; }

                _selectableButtons[0].onClick.AddListener(rf_UpdateCookiesLeft);

            }
            else
            {
                if (_questionType == 0) { _selectableButtons[0].gameObject.GetComponentInChildren<TMP_Text>().text = _optionText[i]; }
                else { _selectableButtons[0].gameObject.GetComponentInChildren<TMP_Text>().text = _optionText2[i]; }

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

        startY = transform.position.y;

        // 50/50 chance they go left or right
        if (Random.Range(0, 2) == 0) { Flip(); }

        //complete scuff, may need further fixes
        //TODO: make less scuff
        _PlayerLaser = FindFirstObjectByType<Laser>(); // currently works as it is the sole laser in the scene, as soon as another is made, this is doomed
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


        if (_canMove)
        {
            // Movimento horizontal
            transform.position += new Vector3(speed * Time.deltaTime * direction, 0, 0);

            // Movimento vertical (flutuação)
            float newY = startY + Mathf.Sin(Time.time * floatSpeed) * floatHeight;
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);

            // Verifica se colidiu com uma parede
            if (Physics2D.Raycast(transform.position, Vector2.right * direction, 0.1f, GroundLayer))
            {
                Flip();
            }
        }

        // death logic
        if (_PlayerLaser.IsTurnedOn && HackingModeManager.Instance.IsHackingModeActive)
        {
            _healthCanvas.SetActive(true);

            Vector2 laserStart = _PlayerLaser.LaserStart;
            Vector2 laserEnd = _PlayerLaser.LaserEnd;

            RaycastHit2D hit = Physics2D.Linecast(laserStart, laserEnd, LayerMask.GetMask("Enemy"));

            //if (hit.collider != null) { print(hit.collider.gameObject.name); }
            if (hit.collider != null && hit.collider.gameObject == this.gameObject)
            {
                _timer += Time.deltaTime;

                // wheel effect
                _dissolveAmount = _timer / _timeToDie;
                _hackSlider.value = _dissolveAmount;
                if (_timer >= _timeToDie)
                {
                    _canMove = false;
                    _buttonsVisible = true;
                    //rf_UpdateCookiesLeft();
                    //Destroy(gameObject);
                }
            }
            else
            {
                _timer = 0f;
            }
        }
        else
        {
            _healthCanvas.SetActive(false);
            _timer = 0f;
        }
    }

    private void Flip()
    {
        direction *= -1;
        Vector3 scale = transform.localScale;
        //scale.x *= -1;
        transform.localScale = scale;
    }

    /// <summary>
    ///  Call when cookie is destroyed/deactivated
    /// </summary>
    private void rf_UpdateCookiesLeft()
    {
        AudioManager.Instance.rf_PlaySFX("CorrectChoice");

        // gets finish point script and reduces the ammount of cookies left to be collected
        FinishPoint finishPoint = GameObject.FindWithTag("Finish").GetComponent<FinishPoint>();

        // decreases cookies left by 1
        finishPoint.LevelObjectives.CookiesLeft--;

        // Try to get the Key component and call the function if it exists
        if (TryGetComponent<Key>(out Key goKey)) { goKey.rf_CompleteKey(); }

        Destroy(gameObject);
    }
    private void rf_WrongChoice()
    {
        AudioManager.Instance.rf_PlaySFX("WrongChoice");
    }

}
