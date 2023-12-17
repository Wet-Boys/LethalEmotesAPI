using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LethalEmotesApi.Ui.Wheel;

[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform))]
[ExecuteAlways]
public class WheelStopEmote : UIBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public WheelStopEmoteGraphic? backgroundGraphic;
    public Graphic? foregroundGraphic;
    public ColorBlock colors;
    [Range(1, 2)] public float scaleMultiplier;
    
    public bool selected;

    protected override void OnEnable()
    {
        base.OnEnable();
        UpdateState(true);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        selected = true;
        UpdateState();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        selected = false;
        UpdateState();
    }

    private Color GetBackgroundColor()
    {
        if (selected)
            return colors.selectedColor;

        return colors.normalColor;
    }
    
    private Color GetForegroundColor()
    {
        if (selected)
            return colors.highlightedColor;

        return colors.disabledColor;
    }
    
    private float GetScale()
    {
        if (selected)
            return scaleMultiplier;

        return 1f;
    }
    
    private void UpdateState(bool instant = false)
    {
        var backgroundColor = GetBackgroundColor();
        var foregroundColor = GetForegroundColor();
        StartColorTween(foregroundColor * colors.colorMultiplier, backgroundColor * colors.colorMultiplier, instant);
    }

    private void StartColorTween(Color foregroundColor, Color backgroundColor, bool instant)
    {
        backgroundGraphic!.CrossFadeColor(backgroundColor, instant ? 0.0f : colors.fadeDuration, true, true);
        foregroundGraphic!.CrossFadeColor(foregroundColor, instant ? 0.0f : colors.fadeDuration, true, true);
    }
}