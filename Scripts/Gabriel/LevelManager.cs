using UnityEngine;
using UnityEngine.UI;


public class LevelManager : MonoBehaviour
{
    public GameObject levelButtonPrefab; 
    public Transform buttonParent;       
    public int numberOfLevels = 2;       

    void Start()
    {
        // Cria os botoes ao iniciar
        for (int i = 1; i <= numberOfLevels; i++)
        {
            GameObject newButton = Instantiate(levelButtonPrefab, buttonParent);

           
            Text btnText = newButton.GetComponentInChildren<Text>();
            if (btnText != null)
            {
                btnText.text = "Fase " + i;
            }

            
            int levelIndex = i; 
            Button btn = newButton.GetComponent<Button>();
            //btn.onClick.AddListener(() => OnLevelButtonClicked(levelIndex));
        }
    }

   /* void OnLevelButtonClicked(int levelIndex)
    {
        // Carrega a cena da fase, ex.: "Level1", "Level2", ...
        // Aqui depende de como você nomeou as cenas
        UnityEngine.SceneManagement.SceneManager.LoadScene("Level" + levelIndex);
    }
   */
}
