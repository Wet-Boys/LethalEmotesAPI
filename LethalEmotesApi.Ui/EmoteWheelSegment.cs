using System;
using LethalEmotesApi.Ui.Wheel;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LethalEmotesApi.Ui;

[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform))]
[ExecuteAlways]
public class EmoteWheelSegment : UIBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public WheelSegmentGraphic? targetGraphic;
    public SegmentLabel? targetLabel;
    public RectTransform? segmentRectTransform;
    public ColorBlock colors;
    [Range(1, 2)] public float scaleMultiplier;
    
    public bool selected;
    public bool focused;

    private bool IsSelected()
    {
        return selected && focused;
    }

    protected override void Awake()
    {
        base.Awake();

        if (targetGraphic is null)
            targetGraphic = GetComponentInChildren<WheelSegmentGraphic>();

        if (targetLabel is null)
            targetLabel = GetComponentInChildren<SegmentLabel>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        UpdateState(false, true);
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

    public void DeSelect()
    {
        selected = false;
        UpdateState(false);
    }

    public void ResetState()
    {
        selected = false;
        UpdateState(false, true);
    }

    private Color GetColor()
    {
        if (IsSelected())
            return colors.selectedColor;

        return colors.normalColor;
    }

    private float GetScale()
    {
        if (IsSelected())
            return scaleMultiplier;

        return 1f;
    }

    private void UpdateState(bool requireFocus = true, bool instant = false)
    {
        if (!focused && requireFocus)
            return;

        var color = GetColor();
        StartColorTween(color * colors.colorMultiplier, instant);
        
        StartScaleTween(GetScale(), instant);
    }

    private void StartColorTween(Color targetColor, bool instant)
    {
        if (targetGraphic is null)
            return;
        
        targetGraphic.CrossFadeColor(targetColor, instant ? 0.0f : colors.fadeDuration, true, true);
    }

    private void StartScaleTween(float targetScale, bool instant)
    {
        if (targetGraphic is null || targetLabel is null)
            return;
        
        targetGraphic.TweenScale(new Vector3(targetScale, targetScale, targetScale), instant ? 0f : colors.fadeDuration, true);
        targetLabel.TweenScale(new Vector3(targetScale, targetScale, targetScale), instant ? 0f : colors.fadeDuration, true);
    }
}