using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private int _currentKeyAmount;
    [SerializeField] private int _keyTotal = 0;

    private void Update()
    {
        if (_currentKeyAmount >= _keyTotal) { Destroy(gameObject); }
    }

    public void rf_GetAKey() { _currentKeyAmount++; }
}
