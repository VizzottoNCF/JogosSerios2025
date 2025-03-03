using UnityEngine;

public class spin : MonoBehaviour
{
    private Quaternion quat;
    private void Start()
    {
        quat = transform.rotation;
    }
    private void Update()
    {
        quat *= Quaternion.Euler(0, 1, 1);

        transform.rotation = quat;
    }
}
