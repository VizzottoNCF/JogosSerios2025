using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public static SceneController Instance;

    private void Awake()
    {
        // singleton instance
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); } else { Destroy(gameObject); }
    }

    public void rf_NextLevel()
    {
        // loads next scene in the index
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void rf_LoadScene(string sceneName)
    {
        SceneManager.LoadSceneAsync(sceneName);
    }
}
