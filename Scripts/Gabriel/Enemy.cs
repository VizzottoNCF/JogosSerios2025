using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float speed = 1.5f;
    [SerializeField] private float _maxRangeFromSpawner = 4f;
    private Rigidbody2D rb; 
    private float direction = -1;
    private Vector3 _startPosition;
    [SerializeField] private LayerMask GroundLayer;
    private float _flipTimer = 0f;
    private float _timer = 0f;
    private float _timeToDie = 0.75f;
    private float _dissolveAmount = 0f;
    [SerializeField] private Laser _PlayerLaser;
    [SerializeField] private GameObject _healthCanvas;
    [SerializeField] private Slider _hackSlider;


    void Start()
    {
        _startPosition = GetComponent<Transform>().position;
        rb = GetComponent<Rigidbody2D>();

        // 50/50 chance they go left or right
        if (Random.Range(0, 2) == 0) { Flip(); }

        //complete scuff, may need further fixes
        //TODO: make less scuff
        _PlayerLaser = FindFirstObjectByType<Laser>(); // currently works as it is the sole laser in the scene, as soon as another is made, this is doomed
    }

    private void Update()
    {
        _flipTimer += Time.deltaTime;
        _timer += Time.deltaTime;


        // death logic
        if (_PlayerLaser.IsTurnedOn && HackingModeManager.Instance.IsHackingModeActive)
        {
            _healthCanvas.SetActive(true);

            Vector2 laserStart = _PlayerLaser.LaserStart;
            Vector2 laserEnd = _PlayerLaser.LaserEnd;

            RaycastHit2D hit = Physics2D.Linecast(laserStart, laserEnd, LayerMask.GetMask("Enemy"));

            if (hit.collider != null && hit.collider.gameObject == this.gameObject)
            {
                _timer += Time.deltaTime;

                // wheel effect
                _dissolveAmount = _timer / _timeToDie;
                _hackSlider.value = _dissolveAmount;
                if (_timer >= _timeToDie)
                {
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
            _healthCanvas.SetActive(false);
            _timer = 0f;
        }



    }

    void FixedUpdate()
    {
        // Movimento horizontal
        transform.position += new Vector3(speed * Time.deltaTime * direction, 0, 0);
        rb.linearVelocity = new Vector2(direction * speed, 0);



        // flips player if hit a wall or too far from spawner entity
        if (Physics2D.Raycast(transform.position, Vector2.right * direction, 0.1f, GroundLayer) || Vector2.Distance(transform.position, _startPosition) >= _maxRangeFromSpawner)
        {
            Flip();
        }
    }

    private void Flip()
    {

        // operates on a half a second cooldown to prevent being stuck on place
        if (_flipTimer > 0.5f)
        {
            _flipTimer = 0f;
            direction *= -1;
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }

    }
}



