using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class HackingModeManager : MonoBehaviour
{
    public static HackingModeManager Instance;
    public bool IsHackingModeActive;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) { IsHackingModeActive = !IsHackingModeActive; }

        if (IsHackingModeActive)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 mousePosition = Input.mousePosition;
                Camera activeCamera = CameraManager.instance.rCC_GetCurrentCamera().GetComponent<CinemachineBrain>().OutputCamera;

                if (activeCamera != null)
                {
                    Ray ray = activeCamera.ScreenPointToRay(mousePosition);
                    if (Physics.Raycast(ray, out RaycastHit hit))
                    {
                        // Use the hit variable to determine what was clicked on.
                    }
                }
            }
        }
    }

}
