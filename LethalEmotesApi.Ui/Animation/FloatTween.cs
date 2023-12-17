using System;
using UnityEngine;
using UnityEngine.Events;

namespace LethalEmotesApi.Ui.Animation;

public struct FloatTween : ITweenValue
{
    public float StartValue { get; set; }
    public float TargetValue { get; set; }
    public bool IgnoreTimeScale { get; set; }
    public float Duration { get; set; }

    private FloatTweenCallback? _target;
    
    public void TweenValue(float percentage)
    {
        if (!ValidTarget())
            return;

        var newValue = Mathf.Lerp(StartValue, TargetValue, percentage);
        _target!.Invoke(newValue);
    }
    
    public bool ValidTarget()
    {
        return _target is not null;
    }

    public void AddOnChangedCallback(UnityAction<float> callback)
    {
        _target ??= new FloatTweenCallback();
        _target.AddListener(callback);
    }

    private class FloatTweenCallback : UnityEvent<float>;
}