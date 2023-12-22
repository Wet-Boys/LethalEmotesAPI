using UnityEngine;

namespace LethalEmotesApi.Ui.Animation;

public struct AnimCurveTween<T> : ITweenValue
    where T : struct, ITweenValue
{
    public T WrappedTweenValue { get; set; }
    public AnimationCurve Curve { get; set; }
    public bool IgnoreTimeScale => WrappedTweenValue.IgnoreTimeScale;
    public float Duration => WrappedTweenValue.Duration;
    
    public void TweenValue(float percentage)
    {
        var curvedPercentage = Curve.Evaluate(percentage);
        
        WrappedTweenValue.TweenValue(curvedPercentage);
    }
    
    public bool ValidTarget() => WrappedTweenValue.ValidTarget();
}