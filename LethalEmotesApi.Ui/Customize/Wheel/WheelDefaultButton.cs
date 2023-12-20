using LethalEmotesApi.Ui.Elements;
using LethalEmotesApi.Ui.Wheel;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LethalEmotesApi.Ui.Customize.Wheel;

[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform))]
[ExecuteAlways]
public class WheelDefaultButton : UIBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public WheelStopEmoteGraphic? targetGraphic;
    public LeUiScaleTweener? scaleTweener;
    public TextMeshProUGUI? targetLabel;
    public ColorBlock colors;
    [Range(1, 2)] public float scaleMultiplier = 1;
    
    public UnityEvent onClick = new();

    private bool _isDefault;
    
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

    public void OnPointerClick(PointerEventData eventData)
    {
        onClick.Invoke();
    }

    public void SetDefault(bool isDefault, bool instant = false)
    {
        _isDefault = isDefault;
        
        UpdateState(instant);
    }
    
    private Color GetColor()
    {
        if (selected)
            return _isDefault ? colors.selectedColor : colors.disabledColor;

        return _isDefault ? colors.highlightedColor : colors.normalColor;
    }
    
    private float GetScale()
    {
        if (selected)
            return scaleMultiplier;

        return 1f;
    }
    
    private void UpdateLabel()
    {
        if (targetLabel is null)
            return;

        targetLabel.text = _isDefault ? "Unset as Default Wheel" : "Set as Default Wheel";
    }
    
    private void UpdateState(bool instant = false)
    {
        UpdateLabel();
        
        var color = GetColor();
        StartColorTween(color * colors.colorMultiplier, instant);

        var scale = GetScale();
        StartScaleTween(scale, instant);
    }

    private void StartColorTween(Color color, bool instant)
    {
        targetGraphic!.CrossFadeColor(color, instant ? 0.0f : colors.fadeDuration, true, true);
    }

    private void StartScaleTween(float scale, bool instant)
    {
        if (scaleTweener is null)
            return;
        
        var scaleVec = new Vector3(scale, scale, scale);
        scaleTweener.TweenScale(scaleVec, instant ? 0.0f : colors.fadeDuration, true);
    }
}