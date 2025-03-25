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
        // Alterna o modo de hacking ao pressionar a tecla E
        if (Input.GetKeyDown(KeyCode.E))
        {
            IsHackingModeActive = !IsHackingModeActive;
        }

        // Se o modo de hacking estiver ativo, verifica cliques do mouse
        if (IsHackingModeActive)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 mousePosition = Input.mousePosition;

                // --- C�digo original com gerenciamento de c�meras (comentado) ---
                // Camera activeCamera = CameraManager.instance.rCC_GetCurrentCamera()
                //     .GetComponent<CinemachineBrain>().OutputCamera;
                // if (activeCamera != null)
                // {
                //     Ray ray = activeCamera.ScreenPointToRay(mousePosition);
                //     if (Physics.Raycast(ray, out RaycastHit hit))
                //     {
                //         // Aqui voc� pode usar o hit para identificar o objeto e chamar o m�todo de hack
                //         HackableObject hackable = hit.collider.GetComponent<HackableObject>();
                //         if (hackable != null)
                //         {
                //             hackable.rf_ObjectHacked();
                //         }
                //     }
                // }
                // ------------------------------------------------------------------

                // Vers�o simplificada utilizando a c�mera principal (Camera.main)
                Camera mainCamera = Camera.main;
                if (mainCamera != null)
                {
                    Ray ray = mainCamera.ScreenPointToRay(mousePosition);
                    if (Physics.Raycast(ray, out RaycastHit hit))
                    {
                        // Verifica se o objeto atingido possui o componente HackableObject
                        HackableObject hackable = hit.collider.GetComponent<HackableObject>();
                        if (hackable != null)
                        {
                            hackable.rf_ObjectHacked();
                        }
                    }
                }
            }
        }
    }
}

