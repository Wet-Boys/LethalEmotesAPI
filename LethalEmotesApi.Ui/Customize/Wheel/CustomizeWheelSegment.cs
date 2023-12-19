using LethalEmotesApi.Ui.Wheel;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LethalEmotesApi.Ui.Customize.Wheel;

[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform))]
public class CustomizeWheelSegment : UIBehaviour
{
    public WheelSegmentGraphic? targetGraphic;
    public SegmentLabel? targetLabel;
    public RectTransform? segmentRectTransform;
    public ColorBlock colors;
    [Range(1, 2)] public float scaleMultiplier;

    public bool selected;

    protected override void Awake()
    {
        base.Awake();
        
        if (targetGraphic is null)
            targetGraphic = GetComponentInChildren<WheelSegmentGraphic>();

        if (targetLabel is null)
            targetLabel = GetComponentInChildren<SegmentLabel>();
    }

    protected override void Start()
    {
        base.Start();
        
        UpdateState(true);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        
        UpdateState(true);
    }

    public void SetLabel(string text)
    {
        if (targetLabel is null)
            return;
        
        targetLabel.SetEmote(text);
    }

    public void Select()
    {
        selected = true;
        UpdateState();
    }

    public void DeSelect()
    {
        selected = false;
        UpdateState();
    }
    
    public void ResetState()
    {
        selected = false;
        UpdateState(true);
    }
    
    private Color GetColor()
    {
        if (selected)
            return colors.selectedColor;

        return colors.normalColor;
    }

    private float GetScale()
    {
        if (selected)
            return scaleMultiplier;

        return 1f;
    }

    private void UpdateState(bool instant = false)
    {
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