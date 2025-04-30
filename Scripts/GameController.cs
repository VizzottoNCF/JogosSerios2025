using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Instance;

    public bool CanPlayerMove = true;
    public bool IsPlayerGrounded = true;

    private void Awake()
    {
        // singleton instance
        if (Instance == null) { Instance = this; }
    }
}
