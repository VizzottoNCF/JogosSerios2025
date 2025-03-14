using UnityEngine;

public class HackableObject : MonoBehaviour
{
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
            _coll.isTrigger = true;
            //debug
            gameObject.GetComponent<SpriteRenderer>().color = Color.red;
        }
    }


    public void rf_ObjectHacked()
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
        _coll.isTrigger = false;
        //debug
        gameObject.GetComponent<SpriteRenderer>().color = Color.white;
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
