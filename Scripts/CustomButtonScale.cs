using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class CustomButtonScale : CustomButtonBase
{
    [Header("References")]
    [SerializeField] private float toScale = 1.1f;
    [SerializeField] private float duration = 0.15f;
    private const float ORIGINAL_SCALE = 1.0f;

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);

        // tweens to 1.1x scale on hover
        transform.DOScale(toScale, duration).SetEase(Ease.InOutSine);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);

        // tweens back to original scale leaves hover
        transform.DOScale(ORIGINAL_SCALE, duration).SetEase(Ease.InOutSine);
    }
}
