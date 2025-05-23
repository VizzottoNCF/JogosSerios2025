using DG.Tweening;
using UnityEngine;

public class HackableObject : MonoBehaviour
{

    [Header("References")]
    [SerializeField] private bool IsActive;
    [SerializeField] private GameObject _3D_Mesh;
    [SerializeField] private re_ObjectType ObjectType;
    [SerializeField] private Vector3 _ActiveCoord;
    [SerializeField] private Vector3 _InactiveCoord;
    [SerializeField] private float _tweenDuration = 1f;
    [SerializeField] private Laser _PlayerLaser;
    //[SerializeField] private float _TimeToHack = 0.5f;
    //private float _hackTimer = 0f;
    //private float _hackCooldown = 1.0f;
    //private bool _onCooldown = false;
    //private float _cooldownTimer = 0f;


    [Header("Situational")]
    private Tween _currentTween;
    private Tween _multiTween;
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
        //complete scuff, may need further fixes
        //TODO: make less scuff
        _PlayerLaser = FindFirstObjectByType<Laser>(); // currently works as it is the sole laser in the scene, as soon as another is made, this is doomed

        _rb = GetComponent<Rigidbody2D>();
        _coll = GetComponent<Collider2D>();

        // assort _3D_Mesh position
        if (IsActive) { _3D_Mesh.transform.position = _ActiveCoord; }
        else { _3D_Mesh.transform.position = _InactiveCoord; }

        // platform setup
        if (ObjectType == re_ObjectType.PLATFORM)
        {
            // trigger is enabled when platform is inactive
            _coll.isTrigger = !IsActive;
        }
    }

    //ivate void Update()
    //
        #region failed laser code
        //// cooldown logic so hacks cant be spammed
        //if (_onCooldown)
        //{
        //    _hackTimer = 0f;
        //    _cooldownTimer += Time.deltaTime;
        //    if (_cooldownTimer >= _hackCooldown) { _cooldownTimer = 0f; _onCooldown = false; }
        //}


        //// hacking logic
        //if (_PlayerLaser.IsTurnedOn)
        //{
        //    Vector2 laserStart = _PlayerLaser.LaserStart;
        //    Vector2 laserEnd = _PlayerLaser.LaserEnd;

        //    RaycastHit2D hit = Physics2D.Linecast(laserStart, laserEnd, LayerMask.GetMask("Ground"));

        //    //if (hit.collider != null) { print(hit.collider.gameObject.name); }
        //    if (hit && hit.collider != null && hit.collider.gameObject == this.gameObject)
        //    {
        //        _hackTimer += Time.deltaTime;

        //        if (_hackTimer >= _TimeToHack && !_onCooldown)
        //        {
        //            hit = default;
        //            _onCooldown = true;
        //            _hackTimer = 0f;
        //            rf_ObjectHacked();
        //        }
        //    }
        //    // reset timer
        //    else
        //    {
        //        _hackTimer = 0f;
        //        _cooldownTimer = 0f;
        //    }
        //}
        //else
        //{
        //    _hackTimer = 0f;
        //}
        #endregion
    //




    
    private void OnMouseDown()
    {
        if (HackingModeManager.Instance.IsHackingModeActive && Vector2.Distance(HackingModeManager.Instance.gameObject.transform.position, this.transform.position) <= _PlayerLaser._LaserRange)
        {
            rf_ObjectHacked();
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

    /// <summary>
    /// Moves 3D Meshes smoothly using Tweening.
    /// </summary>
    /// <param name="targetPosition"> The position Vector3 you want to move to</param>
    private void rf_StartTween(Vector3 targetPosition)
    {
        if (_currentTween != null && _currentTween.IsActive()) { _currentTween.Kill(); }

        _currentTween = _3D_Mesh.transform.DOMove(targetPosition, _tweenDuration).SetEase(Ease.InOutQuad);
    }

    private void rf_multiTween(Vector3 targetPosition)
    {
        if (_currentTween != null && _currentTween.IsActive()) { _currentTween.Kill(); }
        if (_multiTween != null && _multiTween.IsActive()) { _multiTween.Kill(); }

        _currentTween = _3D_Mesh.transform.DOMove(targetPosition, _tweenDuration).SetEase(Ease.InOutQuad);
        _multiTween = transform.DOMove(targetPosition, _tweenDuration).SetEase(Ease.InOutQuad);
    }

    #region Platform Hack
    private void rf_PlatformHack()
    {
        // ACTIVATE PLATFORM
        if (!IsActive)
        {
            IsActive = true;
            _coll.isTrigger = false;
            rf_StartTween(_ActiveCoord);
            //debug
            gameObject.GetComponent<SpriteRenderer>().color = Color.white;
        }

        // DEACTIVATE PLATFORM
        else
        {
            IsActive = false;
            _coll.isTrigger = true;
            rf_StartTween(_InactiveCoord);
            //debug
            gameObject.GetComponent<SpriteRenderer>().color = Color.red;
        }

    }
    #endregion

    #region Wall Hack
    private void rf_WallHack()
    {
        // ACTIVATE PLATFORM
        if (!IsActive)
        {
            IsActive = true;
            rf_multiTween(_ActiveCoord);
        }

        // DEACTIVATE PLATFORM
        else
        {
            IsActive = false;
            rf_multiTween(_InactiveCoord);
        }
    }
    #endregion

    #region Door Hack
    private void rf_DoorHack()
    {

    }
    #endregion
}
