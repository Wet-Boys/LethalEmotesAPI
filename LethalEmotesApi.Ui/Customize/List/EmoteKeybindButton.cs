using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace LethalEmotesApi.Ui.Customize.List;

public class EmoteKeybindButton : EmoteListItemChildInteractable, IPointerClickHandler
{
    public TextMeshProUGUI? keybindLabel;
    public GameObject? placeholderLabel;

    private InputActionRebindingExtensions.RebindingOperation? _rebindingOperation;

    private static EmoteKeybindButton? _rebindTarget;
    
    private InputActionReference? InputRef => EmoteUiManager.GetEmoteKeybind(currentEmoteKey);
    
    public override void UpdateState()
    {
        if (string.IsNullOrEmpty(currentEmoteKey))
            return;

        if (keybindLabel is null || placeholderLabel is null)
            return;

        var inputRef = InputRef;
        if (inputRef is null)
            return;

        var bind = inputRef.action.bindings[0];
        if (!bind.hasOverrides || string.IsNullOrWhiteSpace(bind.overridePath))
        {
            HasNoBinds();
            return;
        }
        
        keybindLabel.text = InputControlPath.ToHumanReadableString(bind.effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
        
        HasBinds();
    }

    private void HasNoBinds()
    {
        if (keybindLabel is null || placeholderLabel is null)
            return;
        
        keybindLabel.gameObject.SetActive(false);
        placeholderLabel.SetActive(true);
    }
    
    private void HasBinds()
    {
        if (keybindLabel is null || placeholderLabel is null)
            return;
        
        placeholderLabel.SetActive(false);
        keybindLabel.gameObject.SetActive(true);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        StartRebind();
    }

    private void StartRebind()
    {
        var inputRef = InputRef;
        if (inputRef is null)
            return;
        
        CancelExistingRebind();
        _rebindTarget = this;

        _rebindingOperation = inputRef.action.PerformInteractiveRebinding()
            .OnMatchWaitForAnother(1f)
            .WithControlsHavingToMatchPath("<Keyboard>")
            .WithControlsHavingToMatchPath("<Mouse>")
            .WithCancelingThrough("<Keyboard>/escape")
            .OnComplete(_ => FinishRebind())
            .OnCancel(_ => FinishRebind())
            .Start();
    }

    private void FinishRebind()
    {
        if (_rebindingOperation is not null)
        {
            _rebindingOperation.Dispose();
            _rebindingOperation = null;
        }

        _rebindTarget = null;
        
        UpdateState();
        
        EmoteUiManager.SaveKeybinds();
    }

    private static void CancelExistingRebind()
    {
        if (_rebindTarget is null)
            return;
        
        _rebindTarget.FinishRebind();
    }
}