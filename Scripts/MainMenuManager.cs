using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;

public class MainMenuManager : MonoBehaviour
{
    [Header("Main Menu Objects")]
    [SerializeField] private GameObject _loadingBarObject;
    [SerializeField] private Image _loadingBar;
    [SerializeField] private GameObject[] _objectsToHide;

    [Header("Scenes to Load")]
    [SerializeField] private string _persistentGameplay = "PersistentGameplay";
    [SerializeField] private string _levelScene = "TEST_SCENE";

    private List<AsyncOperation> _scenesToLoad = new List<AsyncOperation>();

    private void Awake()
    {
        _loadingBarObject.SetActive(false);
    }

    public void rf_StartGame()
    {
        // hide button and text
        rf_HideMenu();

        // start loading the scene
        _loadingBarObject.SetActive(true);

        // uses load scene async so that scene is being loaded while it's deloading the main menu
        SceneManager.LoadSceneAsync(_persistentGameplay);
        SceneManager.LoadSceneAsync(_levelScene, LoadSceneMode.Additive); // additive makes it so it loads the scene inside the current scene

        // update loading bar
        StartCoroutine(rIE_ProgressLoadingBar());
    }

    private void rf_HideMenu()
    {
        for (int i = 0; i < _objectsToHide.Length; i++)
        {
            _objectsToHide[i].SetActive(false);
        }
    }

    private IEnumerator rIE_ProgressLoadingBar()
    {
        float loadProgress = 0f;

        // progresses loading bar dependant on how many scenes have already been loaded
        for (int i = 0; i < _scenesToLoad.Count; i++)
        {
            while (!_scenesToLoad[i].isDone)
            {
                loadProgress += _scenesToLoad[i].progress;
                _loadingBar.fillAmount = loadProgress/ _scenesToLoad.Count;
                yield return null;
            }
        }
    }
}
