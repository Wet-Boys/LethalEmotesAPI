using LethalEmotesApi.Ui.Animation;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LethalEmotesApi.Ui.Wheel;

[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform))]
[ExecuteAlways]
public class SegmentLabel : UIBehaviour
{
    private readonly TweenRunner<Vector3Tween> _scaleTweenRunner = new();
    
    public RectTransform? targetLabel;
    public TextMeshProUGUI? targetText;

    private RectTransform? _rectTransform;
    private string? _emoteKey;

    public RectTransform RectTransform
    {
        get
        {
            _rectTransform ??= GetComponent<RectTransform>();
            return _rectTransform;
        }
    }

    protected DrivenRectTransformTracker Tracker;

    protected SegmentLabel()
    {
        _scaleTweenRunner.Init(this);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        
        if(targetLabel is null)
            return;
        
        Tracker.Add(this, targetLabel, DrivenTransformProperties.Rotation);

        var worldRot = RectTransform.eulerAngles;
        targetLabel.localEulerAngles = -worldRot;
        
        UpdateText();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        
        Tracker.Clear();
    }

    public void SetEmote(string? emoteKey)
    {
        _emoteKey = emoteKey;
        UpdateText();
    }

    private void UpdateText()
    {
        if (targetText is null || _emoteKey is null)
            return;
        
        targetText.SetText(EmoteUiManager.GetEmoteName(_emoteKey));
    }

    public void TweenScale(Vector3 targetScale, float duration, bool ignoreTimeScale)
    {
        if (transform.localScale == targetScale)
        {
            _scaleTweenRunner.StopTween();
            return;
        }
        
        Vector3Tween tween = new Vector3Tween
        {
            Duration = duration,
            StartValue = transform.localScale,
            TargetValue = targetScale,
            IgnoreTimeScale = ignoreTimeScale
        };
        
        tween.AddOnChangedCallback(TweenScaleChanged);
        
        _scaleTweenRunner.StartTween(tween);
    }

    private void TweenScaleChanged(Vector3 scale)
    {
        transform.localScale = scale;
    }
}