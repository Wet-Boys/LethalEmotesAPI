using System.Collections.Generic;
using LethalEmotesApi.Ui.Animation;
using LethalEmotesApi.Ui.Data;
using LethalEmotesApi.Ui.Wheel;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LethalEmotesApi.Ui;

[RequireComponent(typeof(CanvasGroup))]
public class EmoteWheel : MonoBehaviour, IPointerMoveHandler
{
    private readonly TweenRunner<AnimCurveTween<Vector3Tween>> _posTweenRunner = new();
    private readonly TweenRunner<AnimCurveTween<FloatTween>> _alphaTweenRunner = new();
    private readonly DelayedActionRunner<DelayedAction> _delayedActionRunner = new();

    public CanvasGroup? canvasGroup;
    public WheelStopEmote? wheelStopEmote;
    
    public ColorBlock colors;
    [Range(1, 2)] public float scaleMultiplier;
    
    public int segmentCount = 8;
    public float segmentRotOffset = 22.5f;
    public float minRadius = 100f;
    public float maxRadius = 300f;

    public bool test = false;
    
    public List<EmoteWheelSegment> wheelSegments = [];
    public string[] emoteArray;

    private int _currentSegmentIndex = -1;
    private RectTransform _rectTransform;
    private readonly EmoteSelectedCallback _emoteSelectedCallback = new();

    private bool _focused;
    public bool Focused
    {
        get => _focused;
        set
        {
            _focused = value;
            foreach (var segment in wheelSegments)
                segment.focused = _focused;

            if (!_focused)
                _emoteSelectedCallback.RemoveAllListeners();
        }
    }

    protected EmoteWheel()
    {
        emoteArray = new string[segmentCount];
        
        _posTweenRunner.Init(this);
        _alphaTweenRunner.Init(this);
        _delayedActionRunner.Init(this);
    }

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();

        if (canvasGroup is null)
            canvasGroup = GetComponent<CanvasGroup>();
        
        foreach (var segment in wheelSegments)
        {
            segment.colors = colors;
            segment.scaleMultiplier = scaleMultiplier;

            segment.targetGraphic!.segmentCount = segmentCount;
            segment.targetGraphic!.segmentRotOffset = segmentRotOffset;
            segment.targetGraphic!.minRadius = minRadius;
            segment.targetGraphic!.maxRadius = maxRadius;
            
            segment.ResetState();
        }
        
        if (test)
            LoadEmotes(EmoteWheelData.Default().Emotes);
    }

    private void OnEnable()
    {
        ResetState();
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        if (!_focused)
            return;
        
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_rectTransform, eventData.position,
            eventData.enterEventCamera, out var mousePos);

        var rect = _rectTransform.rect;

        var dist = Vector2.Distance(Vector2.zero, mousePos);
        if (dist < minRadius)
        {
            DeSelectAll();
            _emoteSelectedCallback.Invoke(EmoteWheelManager.EmoteNone);
            wheelStopEmote!.OnPointerEnter(eventData);
            return;
        }

        if (mousePos.x > rect.xMax || mousePos.x < rect.xMin || mousePos.y > rect.yMax || mousePos.y < rect.yMin)
        {
            DeSelectAll();
            _emoteSelectedCallback.Invoke("");
            return;
        }
        
        var segmentIndex = GetClosestSegmentIndex(mousePos);
        if (segmentIndex == _currentSegmentIndex)
            return;
        
        wheelStopEmote!.OnPointerExit(eventData);
        
        if (_currentSegmentIndex > -1)
            wheelSegments[_currentSegmentIndex].OnPointerExit(eventData);
            
        _currentSegmentIndex = segmentIndex;
        wheelSegments[segmentIndex].OnPointerEnter(eventData);
        
        _emoteSelectedCallback.Invoke(emoteArray[_currentSegmentIndex]);
    }

    public void DeSelectAll()
    {
        _currentSegmentIndex = -1;
        foreach (var segment in wheelSegments)
            segment.DeSelect();
    }

    public void ResetState()
    {
        _currentSegmentIndex = -1;
        foreach (var segment in wheelSegments)
            segment.ResetState();
        
        _posTweenRunner.StopTween();
        _alphaTweenRunner.StopTween();
        _delayedActionRunner.StopAction();

        canvasGroup!.alpha = 1.0f;
    }

    public void LoadEmotes(string[] emotes)
    {
        emoteArray = emotes;
        for (int i = 0; i < emoteArray.Length; i++)
        {
            var label = wheelSegments[i].targetLabel;
            if (label is null)
                continue;

            label.SetEmote(emoteArray[i]);
        }
    }

    private int GetClosestSegmentIndex(Vector2 mousePos)
    {
        int segmentIndex = -1;
        float shortestDist = float.MaxValue;
        
        for (var i = 0; i < wheelSegments.Count; i++)
        {
            var emoteWheelSegment = wheelSegments[i];
            Vector2 pos = emoteWheelSegment.segmentRectTransform!.position - _rectTransform.position;

            float distToMouse = Vector2.Distance(pos, mousePos);
            if (!(distToMouse < shortestDist))
                continue;

            shortestDist = distToMouse;
            segmentIndex = i;
        }
        
        return segmentIndex;
    }

    public void AddOnEmoteSelectedCallback(UnityAction<string> callback)
    {
        _emoteSelectedCallback.AddListener(callback);
    }

    public void TweenPos(Vector3 targetPos, AnimationCurve curve, float duration, bool ignoreTimeScale)
    {
        if (transform.localPosition == targetPos)
        {
            _posTweenRunner.StopTween();
            return;
        }

        var tween = new Vector3Tween
        {
            Duration = duration,
            StartValue = transform.localPosition,
            TargetValue = targetPos,
            IgnoreTimeScale = ignoreTimeScale
        };
        tween.AddOnChangedCallback(TweenPosChanged);
        
        var animTween = new AnimCurveTween<Vector3Tween>
        {
            WrappedTweenValue = tween,
            Curve = curve
        };
        
        _posTweenRunner.StartTween(animTween);
    }

    private void TweenPosChanged(Vector3 pos)
    {
        transform.localPosition = pos;
    }

    public void TweenAlpha(float targetAlpha, AnimationCurve curve, float duration, bool ignoreTimeScale)
    {
        if (canvasGroup is null)
            return;
        
        // ReSharper disable once CompareOfFloatsByEqualityOperator
        if (canvasGroup.alpha == targetAlpha)
        {
            _alphaTweenRunner.StopTween();
            return;
        }

        var tween = new FloatTween
        {
            Duration = duration,
            StartValue = canvasGroup.alpha,
            TargetValue = targetAlpha,
            IgnoreTimeScale = ignoreTimeScale
        };
        tween.AddOnChangedCallback(TweenAlphaChanged);

        var animTween = new AnimCurveTween<FloatTween>
        {
            WrappedTweenValue = tween,
            Curve = curve
        };
        
        _alphaTweenRunner.StartTween(animTween);
    }

    private void TweenAlphaChanged(float alpha)
    {
        canvasGroup!.alpha = alpha;
    }

    public void DisableAfterDuration(float duration, bool ignoreTimeScale)
    {
        if (!gameObject.activeInHierarchy)
        {
            _delayedActionRunner.StopAction();
            return;
        }

        var action = new DelayedAction
        {
            Duration = duration,
            IgnoreTimeScale = ignoreTimeScale,
            Action = DelayedDisable
        };
        
        _delayedActionRunner.StartAction(action);
    }

    private void DelayedDisable()
    {
        gameObject.SetActive(false);
    }

    private class EmoteSelectedCallback : UnityEvent<string>;
}