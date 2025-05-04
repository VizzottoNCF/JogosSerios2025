using UnityEngine;
using static Unity.Collections.AllocatorManager;

public class CookieEnemy : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private float _timer = 0f;
    [SerializeField] private float _timeToDie = 0.85f;
    [SerializeField] private Laser _PlayerLaser;
    private MaterialPropertyBlock _MaterialPropertyBlock;
    private Renderer _SpriteRenderer;
    private float _dissolveAmount = 0f;
    private const float COMPLETE_DISSOLVE_AMOUNT = 1.1f;
    public float speed = 2f;
    public float floatHeight = 1f;
    public float floatSpeed = 2f;
    public LayerMask GroundLayer;
    private int direction = 1;
    private float startY;
    private bool _isFacingRight = true;

    private void Start()
    {
        _SpriteRenderer = GetComponentInChildren<Renderer>();
        _MaterialPropertyBlock = new MaterialPropertyBlock();




        startY = transform.position.y;

        // 50/50 chance they go left or right
        if (Random.Range(0, 2) == 0) { Flip(); }

        //complete scuff, may need further fixes
        //TODO: make less scuff
        _PlayerLaser = FindFirstObjectByType<Laser>(); // currently works as it is the sole laser in the scene, as soon as another is made, this is doomed
    }

    private void Update()
    {
        // Movimento horizontal
        transform.position += new Vector3(speed * Time.deltaTime * direction, 0, 0);

        // Movimento vertical (flutuação)
        float newY = startY + Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);

        // Verifica se colidiu com uma parede
        if (Physics2D.Raycast(transform.position, Vector2.right * direction, 0.1f, GroundLayer))
        {
            Flip();
        }

        // death logic
        if (_PlayerLaser.IsTurnedOn)
        {
            Vector2 laserStart = _PlayerLaser.LaserStart;
            Vector2 laserEnd = _PlayerLaser.LaserEnd;

            RaycastHit2D hit = Physics2D.Linecast(laserStart, laserEnd, LayerMask.GetMask("Enemy"));

            //if (hit.collider != null) { print(hit.collider.gameObject.name); }
            if (hit.collider != null && hit.collider.gameObject == this.gameObject)
            {
                _timer += Time.deltaTime;

                // shader effect
                _dissolveAmount = (_timer / _timeToDie) * COMPLETE_DISSOLVE_AMOUNT;
                _SpriteRenderer.GetPropertyBlock(_MaterialPropertyBlock);
                _MaterialPropertyBlock.SetFloat("_DissolveAmount", _dissolveAmount);
                _SpriteRenderer.SetPropertyBlock(_MaterialPropertyBlock);


                if (_timer >= _timeToDie)
                {
                    rf_UpdateCookiesLeft();
                    Destroy(gameObject);
                }
            }
            else
            {
                _timer = 0f;
            }
        }
        else
        {
            _timer = 0f;
        }


    }

    private void Flip()
    {
        direction *= -1;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
    private void rf_Turn(bool turnRight)
    {
        if (turnRight)
        {
            // flips player to the right side
            _isFacingRight = true;
            Vector3 rotator = new Vector3(transform.rotation.x, 0f, transform.rotation.z);
            transform.rotation = Quaternion.Euler(rotator);
        }
        else
        {
            // flips player to the left side
            _isFacingRight = false;
            Vector3 rotator = new Vector3(transform.rotation.x, -180f, transform.rotation.z);
            transform.rotation = Quaternion.Euler(rotator);
        }
    }

    /// <summary>
    ///  Call when cookie is destroyed/deactivated
    /// </summary>
    private void rf_UpdateCookiesLeft()
    {
        // gets finish point script and reduces the ammount of cookies left to be collected
        FinishPoint finishPoint = GameObject.FindWithTag("Finish").GetComponent<FinishPoint>();

        // decreases cookies left by 1
        finishPoint.LevelObjectives.CookiesLeft--;
    }

}
