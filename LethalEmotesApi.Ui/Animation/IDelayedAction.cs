namespace LethalEmotesApi.Ui.Animation;

internal interface IDelayedAction
{
    void Invoke();
    
    bool IgnoreTimeScale { get; }
    
    float Duration { get; }
}