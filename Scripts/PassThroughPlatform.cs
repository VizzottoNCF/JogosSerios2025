using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PassThroughPlatform : MonoBehaviour
{
    private Collider2D _collider;
    private bool _playerOnPlatform;

    private void Start()
    {
        _collider = GetComponent<Collider2D>();
    }

    private void rf_SetPlayerOnPlatform(Collision2D collider, bool value)
    {
        var player = collider.gameObject.GetComponent<PlayerMovement>();
        if (player != null)// && player.transform.Find("Feet").GetComponent<BoxCollider2D>().bounds.min.y > _collider.bounds.max.y)
        {
            _playerOnPlatform = value;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        rf_SetPlayerOnPlatform(collision, value:true);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        rf_SetPlayerOnPlatform(collision, value: true);
    }

    private void Update()
    {
        if ((_playerOnPlatform && InputManager.Movement.y < 0))// || (GameObject.FindWithTag("Player").GetComponent<PlayerMovement>().rf_IsFalling() && GameObject.FindWithTag("Player").transform.Find("Feet").GetComponent<BoxCollider2D>().bounds.min.y < _collider.bounds.max.y))
        {
            _collider.enabled = false;
            StartCoroutine(rIE_EnableCollider());
        }
    }

    

    private IEnumerator rIE_EnableCollider()
    {
        yield return new WaitForSeconds(0.5f);
        _collider.enabled = true;
    }
}
