using UnityEngine;

public class spin : MonoBehaviour
{
    [Header("Spin Type:")]
    [SerializeField] private bool FloatSpin;
    [SerializeField] private bool GyroSpin;
    [SerializeField] private bool LateralSpin;
    [SerializeField][Range(-1,1)] public int SpinDirection;
    private Quaternion _quat;
    private Vector3 _initialPos;
    private float _timer = 0f;
    private void Start()
    {
        _quat = transform.rotation;
        _initialPos = transform.position;
        if (SpinDirection == 0) { SpinDirection = 1; } // defaults to spin left
    }
    private void Update()
    {
        if (GyroSpin)
        {
            _quat *= Quaternion.Euler(0, SpinDirection, SpinDirection);
        }

        if (FloatSpin)
        {
            _timer += Time.deltaTime;

            float offsetY = Mathf.Sin(_timer * Mathf.PI * 2) * 0.5f;
            transform.position = new Vector3(_initialPos.x, _initialPos.y + offsetY, _initialPos.z);
        }

        if (LateralSpin)
        {
            _quat *= Quaternion.Euler(0, 0, SpinDirection);
        }

        // makes current quaternion into rotation
        transform.rotation = _quat;
    }
}
