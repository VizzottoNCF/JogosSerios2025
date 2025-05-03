using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private GameObject ScreenPanel;
    [SerializeField] private GameObject PhasesMenu;
    [SerializeField] private Sprite Face;
    [SerializeField] private Sprite BlankScreen;
    [SerializeField] private Button[] MenuButtons;
    [SerializeField] private Button[] LevelButtons;


    private void Awake()
    {
        // disables "Voltar" button
        for (int i = 0; i < MenuButtons.Length; i++)
        {
            // disables voltar
            if (MenuButtons[i].gameObject.name == "Voltar") { MenuButtons[i].gameObject.SetActive(false); }
        }



        // TODO: JSON THAT SAVES GAME PROGRESS
        // gets playerpref value of levels unlocked, if value is null, defaults to 1
        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);


        // gathers every level button via code
        rf_LevelButtonsToArray();

        // disables interaction with all level buttons
        for (int i = 0; i < LevelButtons.Length; i++)
        {
            LevelButtons[i].interactable = false;
        }
        // enables interaction with the amount of levels of UnlockedLevel PlayerPref
        for (int i = 0; i < unlockedLevel; i++)
        {
            LevelButtons[i].interactable = true;
        }

        // sets default image to face sprite
        ScreenPanel.GetComponent<Image>().sprite = Face;

        // disables phase select canvas menu on load
        PhasesMenu.SetActive(false);
    }

    public void OnStartButtonClicked()
    {
        ScreenPanel.SetActive(true);
        PhasesMenu.SetActive(true);

        for (int i = 0; i < MenuButtons.Length;i++)
        {
            // disables start and enables voltar
            if (MenuButtons[i].gameObject.name == "Start") { MenuButtons[i].gameObject.SetActive(false); }
            if (MenuButtons[i].gameObject.name == "Voltar") { MenuButtons[i].gameObject.SetActive(true); }
        }
        ScreenPanel.GetComponent<Image>().sprite = BlankScreen;
    }

    public void OnCloseLevelSelect()
    {
        // Reabre o menu principal e fecha o pop-up
        PhasesMenu.SetActive(false);
        ScreenPanel.GetComponent<Image>().sprite = Face;

        for (int i = 0; i < MenuButtons.Length; i++)
        {
            // enables start and controles and disables voltar
            if (MenuButtons[i].gameObject.name == "Start") { MenuButtons[i].gameObject.SetActive(true); }
            if (MenuButtons[i].gameObject.name == "Voltar") { MenuButtons[i].gameObject.SetActive(false); }
        }
    }

    public void rf_OpenLevel(string levelID)
    {
        string _levelName = "Level_" + levelID;

        SceneController.Instance.rf_LoadScene(_levelName);
    }

    private void rf_LevelButtonsToArray()
    {
        int childcount = PhasesMenu.transform.childCount;
        LevelButtons = new Button[childcount];

        for (int i = 0; i < childcount; i++)
        {
            LevelButtons[i] = PhasesMenu.transform.GetChild(i).gameObject.GetComponent<Button>();
        }
    }

}
