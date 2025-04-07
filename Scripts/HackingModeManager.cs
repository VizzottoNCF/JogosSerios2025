using System;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class HackingModeManager : MonoBehaviour
{
    public static HackingModeManager Instance;
    public bool IsHackingModeActive;
    [SerializeField] private GameObject triggerPrefab;
    [SerializeField] private ParticleSystem _particleSystem;

    private void Update()
    {
        // Alterna o modo de hacking ao pressionar a tecla E
        if (Input.GetKeyDown(KeyCode.E))
        {
            rf_SwitchStates();
        }

        // Se o modo de hacking estiver ativo, verifica cliques do mouse
        if (IsHackingModeActive)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 mousePosition = Input.mousePosition;

                // --- Código original com gerenciamento de câmeras (comentado) ---
                // Camera activeCamera = CameraManager.instance.rCC_GetCurrentCamera()
                //     .GetComponent<CinemachineBrain>().OutputCamera;
                // if (activeCamera != null)
                // {
                //     Ray ray = activeCamera.ScreenPointToRay(mousePosition);
                //     if (Physics.Raycast(ray, out RaycastHit hit))
                //     {
                //         // Aqui você pode usar o hit para identificar o objeto e chamar o método de hack
                //         HackableObject hackable = hit.collider.GetComponent<HackableObject>();
                //         if (hackable != null)
                //         {
                //             hackable.rf_ObjectHacked();
                //         }
                //     }
                // }
                // ------------------------------------------------------------------

                // Versão simplificada utilizando a câmera principal (Camera.main)



                Vector3 worldPos = rv3_GetWorldPosition();
                if (worldPos != Vector3.zero)
                {
                    GameObject triggerObj = Instantiate(triggerPrefab, worldPos, Quaternion.identity);
                    Destroy(triggerObj, 0.1f); // Destroy after a short time
                }
            }

        }
    }

    // switch between active or inactive 
    private void rf_SwitchStates()
    {

        // activate 
        if (!IsHackingModeActive)
        {
            // enable shader graph
            FullScreenHackController.instance.rf_ToggleHackModeOn();
            IsHackingModeActive = true;
        }

        //deactivate
        else if (IsHackingModeActive)
        {
            // disable shader graph
            FullScreenHackController.instance.rf_ToggleHackModeOff();
            IsHackingModeActive = false;
        }
    }

    private Vector3 rv3_GetWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            return hit.point;
        }

        return Vector3.zero; // Return zero if no hit
    }

    public void rf_SpawnRipple(Transform Position)
    {
        ParticleSystem instantiatedParticleSystem = Instantiate(_particleSystem, Position.position, Position.rotation);

        ParticleSystem.MainModule mainModule = instantiatedParticleSystem.main;

        _particleSystem.transform.position = Position.position;

        _particleSystem.Play();
    }
}

