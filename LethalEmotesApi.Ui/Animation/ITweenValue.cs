namespace LethalEmotesApi.Ui.Animation;

public interface ITweenValue
{
    void TweenValue(float percentage);
    
    bool IgnoreTimeScale { get; }
    
    float Duration { get; }

    bool ValidTarget();
}