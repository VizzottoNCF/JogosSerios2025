using System.Collections;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [Header("Stats")]
    public static GameController Instance;
    public bool CanPlayerMove = true;
    public bool IsPlayerGrounded = true;

    [Header("References")]
    [SerializeField] private GameObject _SpriteReference;
    [SerializeField] private Rigidbody2D _rb;
    private Vector2 _startPos;

    private void Awake()
    {
        // singleton instance
        if (Instance == null) { Instance = this; }

        // grabs player spawn point in level and rigidbody component
        _startPos = transform.position;
        _rb = GetComponent<Rigidbody2D>();
    }

    // call this function on health script when necessary
    [ContextMenu("Cause Player Death")]
    public void rf_CausePlayerDeath() { rf_PlayerDeath(); }

    private void rf_PlayerDeath()
    {
        // TODO: effects on player death

        // respawns 
        rf_Respawn();
    }

    private void rf_Respawn()
    {
        // TODO: call for health to be fully restored in death

        StartCoroutine(rIE_Respawn(0.5f));
    }


    private IEnumerator rIE_Respawn(float duration)
    {
        // disables player mobility and gameobject that shows sprite
        _rb.simulated = false;
        CanPlayerMove = false;
        _SpriteReference.SetActive(false);

        // waits for half a second, teleports to spawn, reenables movement and reactivates sprite gameobject
        yield return new WaitForSeconds(duration);
        transform.position = _startPos;

        _rb.simulated = true;
        CanPlayerMove = true;
        _SpriteReference.SetActive(true);
    }
}
