using System.Collections;
using UnityEngine;

namespace LethalEmotesApi.Ui.Animation;

internal class DelayedActionRunner<T> where T : struct, IDelayedAction
{
    protected MonoBehaviour? CoroutineContainer;
    protected IEnumerator? Coroutine;

    private static IEnumerator Start(T delayedAction)
    {
        var elapsedTime = 0f;
        while (elapsedTime < delayedAction.Duration)
        {
            elapsedTime += delayedAction.IgnoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime;

            yield return null;
        }
        
        delayedAction.Invoke();
    }

    public void Init(MonoBehaviour coroutineContainer)
    {
        CoroutineContainer = coroutineContainer;
    }

    public void StartAction(T delayedAction)
    {
        if (CoroutineContainer is null)
            return;
        
        if (Coroutine is not null)
        {
            CoroutineContainer.StopCoroutine(Coroutine);
            Coroutine = null;
        }
        
        if (!CoroutineContainer.gameObject.activeInHierarchy)
        {
            delayedAction.Invoke();
            return;
        }

        Coroutine = Start(delayedAction);
        CoroutineContainer.StartCoroutine(Coroutine);
    }
    
    public void StopAction()
    {
        if (Coroutine is null || CoroutineContainer is null)
            return;
        
        CoroutineContainer.StopCoroutine(Coroutine);
        Coroutine = null;
    }
}