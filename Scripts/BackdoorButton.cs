using UnityEngine;

public class BackdoorButton : MonoBehaviour
{
    [SerializeField] private BackdoorSpawner _spawner;
    private bool _activated = false;

    private void OnMouseDown()
    {
        if (HackingModeManager.Instance.IsHackingModeActive && Vector2.Distance(gameObject.transform.position, HackingModeManager.Instance.gameObject.transform.position) <= HackingModeManager.Instance._Range && !_activated)
        {
            // when clicked, activates the button
            _activated = true;
            _spawner.rf_CloseBackdoor();

            // Try to get the Key component and call the function if it exists
            if (TryGetComponent<Key>(out Key goKey)) { goKey.rf_CompleteKey(); }
        }
    }
}
