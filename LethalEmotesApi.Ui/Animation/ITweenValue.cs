namespace LethalEmotesApi.Ui.Animation;

internal interface ITweenValue
{
    void TweenValue(float percentage);
    
    bool IgnoreTimeScale { get; }
    
    float Duration { get; }

    bool ValidTarget();
}