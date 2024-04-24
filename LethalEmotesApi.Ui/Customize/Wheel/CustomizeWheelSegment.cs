using LethalEmotesApi.Ui.Animation;
using LethalEmotesApi.Ui.Customize.Preview;
using LethalEmotesApi.Ui.Wheel;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LethalEmotesApi.Ui.Customize.Wheel;

[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform))]
public class CustomizeWheelSegment : UIBehaviour
{
    private readonly TweenRunner<Vector3Tween> _previewUnscaleTweenRunner = new();
    private readonly TweenRunner<FloatTween> _previewPosTweenRunner = new();
    
    public WheelSegmentGraphic? targetGraphic;
    public SegmentLabel? targetLabel;
    public RectTransform? segmentRectTransform;
    public PreviewController? previewController;
    public ColorBlock colors;
    [Range(1, 2)] public float scaleMultiplier;

    public bool selected;

    private string? _emoteKey;
    private DrivenRectTransformTracker _tracker;

    protected CustomizeWheelSegment()
    {
        _previewUnscaleTweenRunner.Init(this);
        _previewPosTweenRunner.Init(this);
    }

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

        if (previewController is null || previewController.renderRect is null)
            return;
        
        _tracker.Add(this, previewController.renderRect, DrivenTransformProperties.Rotation);

        var renderRect = previewController.renderRect;
        
        var worldRot = renderRect.eulerAngles;
        renderRect.localEulerAngles = -worldRot;

        UpdateEmotePreview();
        UpdateState(true);
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        _tracker.Clear();
    }

    public void SetEmoteKey(string emoteKey)
    {
        _emoteKey = emoteKey;

        UpdateEmotePreview();
        UpdateState(true);
    }

    public void Select()
    {
        var prevSelected = selected;
        selected = true;

        if (!prevSelected)
            UpdateEmotePreview();
        
        UpdateState();
    }

    public void DeSelect()
    {
        selected = false;

        UpdateEmotePreview();
        UpdateState();
    }
    
    public void ResetState()
    {
        selected = false;

        UpdateEmotePreview();
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

    private void UpdateEmotePreview()
    {
        if (previewController is null || previewController.renderRect is null)
            return;

        var renderRect = previewController.renderRect;
        
        if (_emoteKey is null)
        {
            renderRect.gameObject.SetActive(false);
            return;
        }
        
        if (!EmoteUiManager.GetEmoteVisibility(_emoteKey) || !EmoteUiManager.EmoteDb.EmoteExists(_emoteKey))
        {
            renderRect.gameObject.SetActive(false);
            return;
        }
        
        if (selected)
        {
            renderRect.gameObject.SetActive(true);
            previewController.PlayEmote(_emoteKey);
        }
        else
        {
            previewController.StopEmote();
            renderRect.gameObject.SetActive(false);
        }
    }

    private void UpdateState(bool instant = false)
    {
        if (targetLabel is null || _emoteKey is null)
            return;
        
        targetLabel.SetEmote(_emoteKey);
        
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

        TweenPreview(targetScale, instant);
    }

    private void TweenPreview(float targetScale, bool instant)
    {
        if (previewController is null)
            return;

        var previewTransform = previewController.RectTransform;

        var newPreviewScale = targetScale > 1f ? 1f / scaleMultiplier : 1f;
        var counterScale = new Vector3(newPreviewScale, newPreviewScale, newPreviewScale);

        if (previewTransform.localScale == counterScale)
        {
            _previewUnscaleTweenRunner.StopTween();
            _previewPosTweenRunner.StopTween();
            return;
        }

        var scaleTween = new Vector3Tween
        {
            Duration = instant ? 0f : colors.fadeDuration,
            StartValue = previewTransform.localScale,
            TargetValue = counterScale,
            IgnoreTimeScale = true,
        };
        scaleTween.AddOnChangedCallback(OnUnscalePreview);

        var counterPos = 190f / scaleMultiplier;

        var posTween = new FloatTween
        {
            Duration = instant ? 0f : colors.fadeDuration,
            StartValue = previewTransform.anchoredPosition.y,
            TargetValue = counterPos,
            IgnoreTimeScale = true,
        };
        posTween.AddOnChangedCallback(OnFixPreviewPos);

        _previewUnscaleTweenRunner.StartTween(scaleTween);
    }

    private void OnUnscalePreview(Vector3 scale)
    {
        if (previewController is null)
            return;

        previewController.RectTransform.localScale = scale;
    }
    
    private void OnFixPreviewPos(float yPos)
    {
        if (previewController is null)
            return;

        var pos = previewController.RectTransform.anchoredPosition;
        pos.y = yPos;
        previewController.RectTransform.anchoredPosition = pos;
    }
}