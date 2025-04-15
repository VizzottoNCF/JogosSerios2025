using UnityEngine;

public class DialogTrigger : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private bool _hasBeenActivated = false;
    private Collider2D _coll;

    [Header("Dialog Content")]
    [SerializeField] private string[] _speaker;
    [SerializeField] [TextArea] private string[] _dialog;
    [SerializeField] private Sprite[] _portrait;

    private void Start()
    {
        _coll = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // call dialog if it never triggered before
        if (collision.gameObject.CompareTag("Player") && !_hasBeenActivated)
        {
            _hasBeenActivated = true;

            DialogManager.Instance.rf_StartDialog(_speaker, _dialog, _portrait);
        }
    }
}
