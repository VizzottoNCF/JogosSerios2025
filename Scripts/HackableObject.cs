using UnityEngine;

public class HackableObject : MonoBehaviour
{

    [Header("References")]
    [SerializeField] private GameObject _3D_Mesh;
    [SerializeField] private re_ObjectType ObjectType;
    private Rigidbody2D _rb;
    private Collider2D _coll;
    private enum re_ObjectType
    {
        PLATFORM,
        WALL,
        DOOR,
    }

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _coll = GetComponent<Collider2D>();

        if (ObjectType == re_ObjectType.PLATFORM)
        {
            //_3D_Mesh?.GetComponent<Material>().
            _coll.isTrigger = true;
            //debug
            gameObject.GetComponent<SpriteRenderer>().color = Color.red;
        }
    }

    private void OnMouseDown()
    {
        if (HackingModeManager.Instance.IsHackingModeActive)
        {
            rf_ObjectHacked();
            print(gameObject.name);
        }
    }

    private void rf_ObjectHacked()
    {
        // calls function based on object type 
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
        // local bool is true when platform is a trigger (not able to be collided with)
        bool isPlatformInactive = _coll.isTrigger;

        if (isPlatformInactive)
        {
            _coll.isTrigger = false;
            //debug
            gameObject.GetComponent<SpriteRenderer>().color = Color.white;
        }
        else
        {
            _coll.isTrigger = true;
            //debug
            gameObject.GetComponent<SpriteRenderer>().color = Color.red;
        }

    }
    #endregion
    #region Wall Hack
    private void rf_WallHack()
    {

    }
    #endregion
    #region Door Hack
    private void rf_DoorHack()
    {

    }
    #endregion
}
