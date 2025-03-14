using UnityEngine;

public class spin : MonoBehaviour
{
    [Header("Spin Type:")]
    [SerializeField] public bool FloatSpin;
    [SerializeField] public bool GyroSpin;
    [SerializeField][Range(-1,1)] public int GyroSpinDirection;
    private Quaternion _quat;
    private Vector3 _initialPos;
    private float _timer = 0f;
    private void Start()
    {
        _quat = transform.rotation;
        _initialPos = transform.position;
        if (GyroSpinDirection == 0) { GyroSpinDirection = 1; } // defaults to spin left
    }
    private void Update()
    {
        if (GyroSpin)
        {
            _quat *= Quaternion.Euler(0, GyroSpinDirection, GyroSpinDirection);
            //_quat *= Quaternion.Euler(0, 1, 1);
        }

        if (FloatSpin)
        {
            _timer += Time.deltaTime;

            float offsetY = Mathf.Sin(_timer * Mathf.PI * 2) * 0.5f;
            transform.position = new Vector3(_initialPos.x, _initialPos.y + offsetY, _initialPos.z);
        }

        // makes current quaternion into rotation
        transform.rotation = _quat;
    }
}
