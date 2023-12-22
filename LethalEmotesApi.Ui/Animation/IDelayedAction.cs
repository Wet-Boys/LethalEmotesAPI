namespace LethalEmotesApi.Ui.Animation;

public interface IDelayedAction
{
    void Invoke();
    
    bool IgnoreTimeScale { get; }
    
    float Duration { get; }
}