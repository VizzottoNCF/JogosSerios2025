using UnityEngine;

public class HackeablePlataform : MonoBehaviour
{
    public enum re_ObjectType
    {
        PLATFORM,
        WALL,
        DOOR,
    }

    [SerializeField] private re_ObjectType ObjectType;
    private Collider2D _coll;

    private void Start()
    {
        _coll = GetComponent<Collider2D>();

        if (ObjectType == re_ObjectType.PLATFORM)
        {
            // Inicialmente, desabilita o controle da plataforma (para não interferir com a física normal)
            var platformControl = GetComponent<PlataformController>();
            if (platformControl != null)
            {
                platformControl.enabled = false;
            }
            // Debug: pinta a plataforma de vermelho para indicar que está hackeável
            GetComponent<SpriteRenderer>().color = Color.red;
        }
    }

    public void rf_ObjectHacked()
    {
        switch (ObjectType)
        {
            case re_ObjectType.PLATFORM:
                rf_PlatformHack();
                break;
            case re_ObjectType.WALL:
                rf_WallHack();
                break;
            case re_ObjectType.DOOR:
                rf_DoorHack();
                break;
        }
    }

    #region Platform Hack
    private void rf_PlatformHack()
    {
        // Ao ser hackeada, habilita o controle da plataforma
        var platformControl = GetComponent<PlataformController>();
        if (platformControl != null)
        {
            platformControl.enabled = true;
        }
        // Pode ajustar o collider se necessário
        if (_coll != null)
        {
            _coll.isTrigger = false;
        }
        // Debug: muda a cor para branco para indicar que foi hackeada
        GetComponent<SpriteRenderer>().color = Color.white;
    }
    #endregion

    #region Wall Hack
    private void rf_WallHack()
    {
        // Implementar se necessário
    }
    #endregion

    #region Door Hack
    private void rf_DoorHack()
    {
        // Implementar se necessário
    }
    #endregion
}

