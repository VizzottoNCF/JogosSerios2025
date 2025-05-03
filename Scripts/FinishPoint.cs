using UnityEngine;

public class FinishPoint : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // TODO: add heck if all objectives are complete
        if (collision.CompareTag("Player"))
        {
            // TODO: display finish level UI


            // TEMP
            rf_GoToNextLevel();
        }
    }

    public void rf_GoToNextLevel()
    {
        SceneController.Instance.rf_NextLevel();
    }
}
