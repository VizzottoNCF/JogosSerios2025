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
    [SerializeField] private GameObject triggerPrefab;
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



    #region Click Logic

    //public void OnPointerDown(PointerEventData eventData)
    //{
    //    print("Event PointerDown triggered");
    //    if (!IsHackingModeActive) { return; }

    //    print("Is on Hack Mode");
    //    // Check if clicked object has the component
    //    GameObject clickedObject = eventData.pointerCurrentRaycast.gameObject;

    //    if (clickedObject == null) { return; }
        
    //    print(clickedObject.name);

    //    HackableObject hackable = clickedObject.GetComponent<HackableObject>();

    //    if (hackable != null)
    //    {
    //        Debug.Log("Hackable object clicked!");
    //        hackable.rf_ObjectHacked();
    //    }
    //    else
    //    {
    //        Debug.Log("Clicked object is not hackable.");
    //    }
    //}
    #endregion
}

