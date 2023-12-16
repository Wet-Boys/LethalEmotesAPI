using System;
using UnityEngine.Events;

namespace LethalEmotesApi.Ui;

public class EmoteInteractionHandler
{
    private readonly Func<bool> _callback;
    private readonly Action<bool> _lockCallback;

    public EmoteInteractionHandler(Func<bool> callback, Action<bool> lockCallback)
    {
        _callback = callback;
        _lockCallback = lockCallback;
    }
    
    public bool InOtherMenu()
    {
        return _callback.Invoke();
    }

    public void SetMenuLocked(bool locked)
    {
        _lockCallback.Invoke(locked);
    }
}