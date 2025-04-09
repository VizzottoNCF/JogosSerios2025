using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private GameObject ScreenPanel;
    [SerializeField] private GameObject PhasesMenu;
    [SerializeField] private Sprite Face;
    [SerializeField] private Sprite BlankScreen;



    private void Awake()
    {
        ScreenPanel.GetComponent<Image>().sprite = Face;
        PhasesMenu.SetActive(false);
    }

    public void OnStartButtonClicked()
    {
        ScreenPanel.SetActive(true);
        PhasesMenu.SetActive(true);
        ScreenPanel.GetComponent<Image>().sprite = BlankScreen;
    }

    public void OnCloseLevelSelect()
    {
        // Reabre o menu principal e fecha o pop-up
        PhasesMenu.SetActive(false);
        ScreenPanel.GetComponent<Image>().sprite = Face;
    }

    public void rf_SendToLevel(int _Level)
    {
        switch (_Level)
        {
            default: break;
            case 01:
                // load scene code
                break;
        }
    }

}
