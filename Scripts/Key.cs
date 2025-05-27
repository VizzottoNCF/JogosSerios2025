using UnityEngine;

public class Key : MonoBehaviour
{
    [SerializeField] private Door[] _door;


    public void rf_CompleteKey() {  for (int i = 0; i < _door.Length; i++) { _door[i].rf_GetAKey(); } AudioManager.Instance.rf_PlaySFX("Key"); }
}
