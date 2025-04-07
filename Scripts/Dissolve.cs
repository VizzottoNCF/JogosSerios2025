using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Dissolve : MonoBehaviour
{
    // dissolve shader graph script

    [SerializeField] private float _dissolveTime = 0.75f;
    [SerializeField] private bool _debug = false;

    private SpriteRenderer[] _spriteRenderer;
    private Material[] _materials;

    private int _dissolveAmmount = Shader.PropertyToID("_DissolveAmmount");
    private int _verticalDissolveAmmount = Shader.PropertyToID("_VerticalDissolveAmmount");

    private void Start()
    {
        _spriteRenderer = GetComponentsInChildren<SpriteRenderer>();

        _materials = new Material[_spriteRenderer.Length];
        for (int i = 0; i < _spriteRenderer.Length; i++)
        {
            _materials[i] = _spriteRenderer[i].material;
        }
    }
    private void Update()
    {
        if (_debug)
        {
            if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                StartCoroutine(rIE_Vanish(true, false));
            }
            if (Keyboard.current.fKey.wasPressedThisFrame)
            {
                StartCoroutine(rIE_Appear(true, false));
            }
        }
    }

    private IEnumerator rIE_Vanish(bool useDissolve, bool useVertical)
    {
        float elapsedTime = 0f;
        while (elapsedTime < _dissolveTime)
        {
            elapsedTime += Time.deltaTime;

            float lerpedDissolve = Mathf.Lerp(0, 1.1f, (elapsedTime / _dissolveTime));
            float lerpedVerticalDissolve = Mathf.Lerp(0, 1.1f, (elapsedTime/_dissolveTime));

            for (int i = 0; i < _materials.Length; i++)
            {
                if (useDissolve)
                {
                    _materials[i].SetFloat(_dissolveAmmount, lerpedDissolve);
                }
                if (useVertical)
                {
                    _materials[i].SetFloat(_verticalDissolveAmmount, lerpedVerticalDissolve);
                }
            }

            yield return null;
        }
    }


    private IEnumerator rIE_Appear(bool useDissolve, bool useVertical)
    {
        float elapsedTime = 0f;
        while (elapsedTime < _dissolveTime)
        {
            elapsedTime += Time.deltaTime;

            float lerpedDissolve = Mathf.Lerp(1.1f, 0, (elapsedTime / _dissolveTime));
            float lerpedVerticalDissolve = Mathf.Lerp(1.1f, 0, (elapsedTime / _dissolveTime));

            for (int i = 0; i < _materials.Length; i++)
            {
                if (useDissolve)
                {
                    _materials[i].SetFloat(_dissolveAmmount, lerpedDissolve);
                }
                if (useVertical)
                {
                    _materials[i].SetFloat(_verticalDissolveAmmount, lerpedVerticalDissolve);
                }
            }

            yield return null;
        }
    }
}
