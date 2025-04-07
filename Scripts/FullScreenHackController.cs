using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;

public class FullScreenHackController : MonoBehaviour
{
    public static FullScreenHackController instance;

    [Header("Time Stats")]
    [SerializeField] private float _hackDisplayTime = 1.5f;
    [SerializeField] private float _hackFadeOutTime = 0.5f;

    [Header("References")]
    [SerializeField] private ScriptableRendererFeature _fullScreenHack;
    [SerializeField] private Material _material;

    private int _voronoiIntensity = Shader.PropertyToID("_VoronoiIntensity");
    private int _vignetteIntensity = Shader.PropertyToID("_VignetteIntensity");

    private const float VORONOI_INTENSITY_START_AMOUNT = 1f;
    private const float VIGNETTE_INTENSITY_START_AMOUNT = 0.85f;

    private Coroutine _currentRoutine;

    private void Awake()
    {
        // create singleton instance
        if (instance == null) { instance = this; }

        _fullScreenHack.SetActive(false);
    }

    // Call this to activate hacker mode effect
    public void rf_ToggleHackModeOn()
    {
        if (_currentRoutine != null) StopCoroutine(_currentRoutine);
        _currentRoutine = StartCoroutine(rIE_ActivateHackMode());
    }

    // Call this to deactivate hacker mode effect
    public void rf_ToggleHackModeOff()
    {
        if (_currentRoutine != null) StopCoroutine(_currentRoutine);
        _currentRoutine = StartCoroutine(rIE_FadeOutHackEffect());
    }

    // eases in hack mode effect
    private IEnumerator rIE_ActivateHackMode()
    {
        _fullScreenHack.SetActive(true);
        float elapsedTime = 0f;
        _material.SetFloat(_voronoiIntensity, 0);
        _material.SetFloat(_vignetteIntensity, 0);

        while (elapsedTime < _hackFadeOutTime)
        {
            elapsedTime += Time.deltaTime;

            float lerpedVoronoi = Mathf.Lerp(0, VORONOI_INTENSITY_START_AMOUNT, (elapsedTime / _hackFadeOutTime));
            float lerpedVignette = Mathf.Lerp(0f, VIGNETTE_INTENSITY_START_AMOUNT, (elapsedTime / _hackFadeOutTime));


            _material.SetFloat(_voronoiIntensity, lerpedVoronoi);
            _material.SetFloat(_vignetteIntensity, lerpedVignette);

            yield return null;
        }

        yield return null;
    }

    // eases out hack mode effect
    private IEnumerator rIE_FadeOutHackEffect()
    {
        float elapsedTime = 0f;


        while (elapsedTime < _hackFadeOutTime)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / _hackFadeOutTime;

            float lerpedVoronoi = Mathf.Lerp(VORONOI_INTENSITY_START_AMOUNT, 0f, (elapsedTime/_hackFadeOutTime));
            float lerpedVignette = Mathf.Lerp(VIGNETTE_INTENSITY_START_AMOUNT, 0f, (elapsedTime / _hackFadeOutTime));

            _material.SetFloat(_voronoiIntensity, lerpedVoronoi);
            _material.SetFloat(_vignetteIntensity, lerpedVignette);

            yield return null;
        }

        _material.SetFloat(_voronoiIntensity, 0f);
        _material.SetFloat(_vignetteIntensity, 0f);
        _fullScreenHack.SetActive(false);
    }
}
