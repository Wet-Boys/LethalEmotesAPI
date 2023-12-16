using System;
using UnityEngine.Events;

namespace LethalEmotesApi.Ui;

public class EmoteInteractionHandler
{
    private readonly Func<bool> _callback;
    private readonly Action<bool> _lockCallback;
    private bool _inEmoteUi;

    public EmoteInteractionHandler(Func<bool> callback, Action<bool> lockCallback)
    {
        _callback = callback;
        _lockCallback = lockCallback;
    }
    
    public bool InOtherMenu()
    {
        return _callback.Invoke();
    }

    public void SetInEmoteUi(bool inEmoteUi)
    {
        _lockCallback.Invoke(inEmoteUi);
        _inEmoteUi = inEmoteUi;
    }

    public bool InEmoteUi()
    {
        return _inEmoteUi;
    }
}