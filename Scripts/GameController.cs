using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Instance;

    public bool CanPlayerMove = true;

    private void Awake()
    {
        // singleton instance
        if (Instance == null) { Instance = this; }
    }
}
