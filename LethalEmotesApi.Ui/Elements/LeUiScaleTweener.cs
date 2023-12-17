using System.Collections.Generic;
using LethalEmotesApi.Ui.Animation;
using UnityEngine;

namespace LethalEmotesApi.Ui.Elements;

[DisallowMultipleComponent]
[ExecuteAlways]
public class LeUiScaleTweener : MonoBehaviour
{
    private readonly TweenRunner<Vector3Tween> _scaleTweenRunner = new();
    private Vector3 _internalScale = Vector3.one;
    
    public List<RectTransform> targets = [];

    protected LeUiScaleTweener()
    {
        _scaleTweenRunner.Init(this);
    }

    public void TweenScale(Vector3 targetScale, float duration, bool ignoreTimeScale)
    {
        if (_internalScale == targetScale)
        {
            _scaleTweenRunner.StopTween();
            return;
        }

        var tween = new Vector3Tween
        {
            Duration = duration,
            IgnoreTimeScale = ignoreTimeScale,
            StartValue = _internalScale,
            TargetValue = targetScale
        };
        tween.AddOnChangedCallback(TweenScaleChanged);
            
        _scaleTweenRunner.StartTween(tween);
    }

    private void TweenScaleChanged(Vector3 scale)
    {
        _internalScale = scale;

        foreach (var target in targets)
            target.localScale = scale;
    }
}