using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishPoint : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator _Animator;
    [SerializeField] private TMP_Text _Cookies;
    [SerializeField] private TMP_Text _PrivateImg;
    [SerializeField] private TMP_Text _Backdoor;
    [SerializeField] private TMP_Text _AllDone;
    [SerializeField] private GameObject _SystemShiftImageOn;
    [SerializeField] private GameObject _SystemShiftImageOff;
    [SerializeField] private GameObject _FinishUI;
    public LevelObjectives LevelObjectives;

    //[SerializeField] private GameObject _UI_Canvas;
    [SerializeField] private bool _canFinish;

    private void Start()
    {
        // disables all objective complete text (will be reenabled in update)
        _AllDone.gameObject.SetActive(false);

        // disables finish level UI
        _FinishUI.gameObject.SetActive(false);
    }

    private void Update()
    {
        // if current objectives is less than total
        if (LevelObjectives.CurrentObjectives < LevelObjectives.TotalObjectives)
        {
            // - PRIVATE IMAGES
            if (LevelObjectives.PrivateImagesLeft == 0) { LevelObjectives.CurrentObjectives++; LevelObjectives.PrivateImagesLeft = -1; _PrivateImg.gameObject.SetActive(false); }
            else if (LevelObjectives.PrivateImagesLeft > 0) { _PrivateImg.gameObject.SetActive(true); _PrivateImg.text = "Privatize " + LevelObjectives.PrivateImagesLeft + " Imagens"; }

            // - COOKIES
            if (LevelObjectives.CookiesLeft == 0) { LevelObjectives.CurrentObjectives++; LevelObjectives.CookiesLeft = -1; _Cookies.gameObject.SetActive(false); }
            else if (LevelObjectives.CookiesLeft > 0) { _Cookies.gameObject.SetActive(true); _Cookies.text = "Negue " + LevelObjectives.CookiesLeft + " Cookies"; }

            // - BACKDOORS
            if (LevelObjectives.BackdoorLeft == 0) { LevelObjectives.CurrentObjectives++; LevelObjectives.BackdoorLeft = -1; _Backdoor.gameObject.SetActive(false); }
            else if (LevelObjectives.BackdoorLeft > 0) { _Backdoor.gameObject.SetActive(true); _Backdoor.text = "Feche " + LevelObjectives.BackdoorLeft + " Backdoors"; }
        }
        else
        {
            _canFinish = true;

            // update canvas
            _AllDone.gameObject.SetActive(true);
        }

        // update canvas
        if (HackingModeManager.Instance.IsHackingModeActive) { _SystemShiftImageOn.SetActive(true); _SystemShiftImageOff.SetActive(false); }
        else { _SystemShiftImageOn.SetActive(false); _SystemShiftImageOff.SetActive(true); }

        // update animator
        _Animator.SetBool("Open", _canFinish);

    }



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && _canFinish)
        {
            // calls to unlock level in menu
            rf_UnlockNewLevel();

            // enables and displays finish level UI
            _FinishUI.SetActive(true);
            
            GameController.Instance.CanPlayerMove = false;
        }
    }

    public void rf_GoToMenu()
    {
        SceneManager.LoadScene("MAIN_MENU");
    }



    public void rf_GoToNextLevel()
    {
        SceneController.Instance.rf_NextLevel();
    }

    private void rf_UnlockNewLevel()
    {
        if (SceneManager.GetActiveScene().buildIndex >= PlayerPrefs.GetInt("ReachedIndex"))
        {
            PlayerPrefs.SetInt("ReachedIndex", SceneManager.GetActiveScene().buildIndex + 1);
            PlayerPrefs.SetInt("UnlockedLevel", PlayerPrefs.GetInt("UnlockedLevel", 1) + 1);
            PlayerPrefs.Save();
        }
    }
}


// TYPES OF OBJECTIVES
// - PRIVATE IMAGES
// - COOKIES
// - BACKDOORS
[System.Serializable]
public class LevelObjectives
{
    public int TotalObjectives;
    public int CurrentObjectives = 0;
    //----------------------------------------
    public int CookiesLeft = 0;
    public int PrivateImagesLeft = 0;
    public int BackdoorLeft = 0;
}