using UnityEngine.Events;

namespace LethalEmotesApi.Ui.Animation;

public struct DelayedAction : IDelayedAction
{
    public UnityAction Action { get; set; }
    public float Duration { get; set; }
    public bool IgnoreTimeScale { get; set; }
    
    public void Invoke()
    {
        Action.Invoke();
    }
}