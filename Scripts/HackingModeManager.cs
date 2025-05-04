using System;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class HackingModeManager : MonoBehaviour//, IPointerDownHandler
{
    public static HackingModeManager Instance;
    public bool IsHackingModeActive;
    [SerializeField] private ParticleSystem _particleSystem;

    private void Awake()
    {
        // singleton instance
        if (Instance == null) { Instance = this; }
    }

    private void Update()
    {
        // Alterna o modo de hacking ao pressionar a tecla E
        if (Input.GetKeyDown(KeyCode.E))
        {
            rf_SwitchStates();
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
            GameController.Instance.CanPlayerMove = false;
        }

        //deactivate
        else if (IsHackingModeActive)
        {
            // disable shader graph
            FullScreenHackController.instance.rf_ToggleHackModeOff();
            IsHackingModeActive = false;
            GameController.Instance.CanPlayerMove = true;
        }
    }

    public void rf_SpawnRipple(Transform Position)
    {
        ParticleSystem instantiatedParticleSystem = Instantiate(_particleSystem, Position.position, Position.rotation);

        ParticleSystem.MainModule mainModule = instantiatedParticleSystem.main;

        _particleSystem.transform.position = Position.position;

        _particleSystem.Play();
    }
}

