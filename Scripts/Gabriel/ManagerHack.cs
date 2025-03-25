using UnityEngine;

public class ManagerHack : MonoBehaviour
{
    public static ManagerHack Instance;
    public bool IsHackingModeActive;

    // Referência ao script de movimento do jogador para desabilitar quando em modo hacker
    public PlayerMovement playerMovement;

    private void Awake()
    {
        // Singleton (se necessário)
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        // Alterna o modo hacker com a tecla E
        if (Input.GetKeyDown(KeyCode.E))
        {
            IsHackingModeActive = !IsHackingModeActive;
            // Ativa ou desativa o controle do jogador
            if (playerMovement != null)
            {
                playerMovement.enabled = !IsHackingModeActive;
            }
        }

        // Se estiver em modo hacker e o jogador clicar, faz o raycast para hackear
        if (IsHackingModeActive && Input.GetMouseButtonDown(0))
        {
            Vector3 mousePosition = Input.mousePosition;
            Camera mainCamera = Camera.main;
            if (mainCamera != null)
            {
                Ray ray = mainCamera.ScreenPointToRay(mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    HackeablePlataform hackable = hit.collider.GetComponent<HackeablePlataform>();
                    if (hackable != null)
                    {
                        hackable.rf_ObjectHacked();
                    }
                }
            }
        }
    }
}
