using System.Linq;
using LethalEmotesApi.Ui.Customize.RebindConflict;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace LethalEmotesApi.Ui.Customize.List;

public class EmoteKeybindButton : EmoteListItemChildInteractable
{
    public TextMeshProUGUI? keybindLabel;
    public TextMeshProUGUI? placeholderLabel;
    public EmoteKeybindIndicator? keybindIndicator;
    public float rebindTimeout = 5f;

    private float _rebindTimeoutTimer;
    private bool _rebinding;
    private bool _cursorInside;
    private InputActionRebindingExtensions.RebindingOperation? _rebindingOperation;

    private static EmoteKeybindButton? _rebindTarget;
    
    private InputActionReference? InputRef => EmoteUiManager.GetEmoteKeybind(currentEmoteKey);
    private bool HasBinds => InputRef is not null && InputRef.action.bindings[0].hasOverrides;
    
    public override void UpdateState()
    {
        if (string.IsNullOrEmpty(currentEmoteKey))
            return;
        
        if (_rebinding)
        {
            SetRebinding();
            return;
        }

        if (keybindLabel is null)
            return;

        var inputRef = InputRef;
        if (inputRef is null)
            return;

        var bind = inputRef.action.bindings[0];
        if (!bind.hasOverrides || string.IsNullOrWhiteSpace(bind.overridePath))
        {
            SetHasNoBinds();
            return;
        }
        
        keybindLabel.text = InputControlPath.ToHumanReadableString(bind.effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
        
        SetHasBinds();
    }

    private void SetHasNoBinds()
    {
        if (keybindLabel is null || placeholderLabel is null || keybindIndicator is null || tooltip is null)
            return;
        
        keybindLabel.gameObject.SetActive(false);
        placeholderLabel.gameObject.SetActive(true);
        keybindIndicator.enabled = false;
        tooltip.SetActive(false);
    }

    private void SetRebinding()
    {
        if (keybindLabel is null || placeholderLabel is null || keybindIndicator is null || tooltip is null)
            return;
        
        placeholderLabel.gameObject.SetActive(false);
        keybindLabel.gameObject.SetActive(false);
        keybindIndicator.enabled = true;
        tooltip.SetActive(false);
    }
    
    private void SetHasBinds()
    {
        if (keybindLabel is null || placeholderLabel is null || keybindIndicator is null || tooltip is null)
            return;
        
        placeholderLabel.gameObject.SetActive(false);
        keybindLabel.gameObject.SetActive(true);
        keybindIndicator.enabled = false;
        tooltip.SetActive(_cursorInside);
    }

    private void Update()
    {
        if (!_rebinding)
            return;

        _rebindTimeoutTimer -= Time.deltaTime;
        if (_rebindTimeoutTimer > 0f)
            return;
        
        FinishRebind();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        _cursorInside = false;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        _cursorInside = false;
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        _cursorInside = true;

        if (tooltip is null || !HasBinds)
            return;
        
        tooltip.SetActive(true);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        _cursorInside = false;

        if (tooltip is null)
            return;
        
        tooltip.SetActive(false);
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);
        
        if (eventData.button == PointerEventData.InputButton.Left)
            StartRebind();

        if (eventData.button == PointerEventData.InputButton.Right)
            ClearBind();
    }

    private void StartRebind()
    {
        var inputRef = InputRef;
        if (inputRef is null)
            return;
        
        CancelExistingRebind();
        _rebindTarget = this;
        _rebindTimeoutTimer = rebindTimeout;

        _rebindingOperation = inputRef.action.PerformInteractiveRebinding()
            .OnMatchWaitForAnother(0.1f)
            .WithControlsHavingToMatchPath("<Keyboard>")
            .WithControlsHavingToMatchPath("<Mouse>")
            .WithCancelingThrough("<Keyboard>/escape")
            .OnComplete(OnRebindComplete)
            .OnCancel(_ => FinishRebind())
            .Start();

        _rebinding = true;
        SetRebinding();
    }

    private void OnRebindComplete(InputActionRebindingExtensions.RebindingOperation operation)
    {
        if (currentEmoteKey is null)
            return;
        
        var bindingPath = operation.action.bindings.First().overridePath;
        if (bindingPath is null)
        {
            FinishRebind();
            return;
        }

        // Array of emote keys that have the same bind path, excluding the current emote key.
        var emoteKeys = EmoteUiManager.GetEmoteKeysForBindPath(bindingPath)
            .ToArray();
        
        if (emoteKeys.Length <= 0) // No conflicting emote keys
        {
            FinishRebind();
            return;
        }

        _rebinding = false;
        
        RebindConflictController.CreateConflictModal(currentEmoteKey, emoteKeys);
    }

    private void FinishRebind()
    {
        if (keybindIndicator is null && keybindIndicator)
            return;
        Debug.Log($"{keybindIndicator}");
        keybindIndicator.enabled = false;
        
        if (_rebindingOperation is not null)
        {
            _rebindingOperation.Dispose();
            _rebindingOperation = null;
        }

        _rebindTarget = null;
        _rebinding = false;
        
        UpdateState();
        
        EmoteUiManager.SaveKeybinds();
    }

    private void ClearBind()
    {
        var inputRef = InputRef;
        if (inputRef is null)
            return;
        
        CancelExistingRebind();
        
        inputRef.action.RemoveAllBindingOverrides();
        
        UpdateState();
        
        EmoteUiManager.SaveKeybinds();
    }

    public static void CancelExistingRebind()
    {
        if (_rebindTarget is null)
            return;
        
        _rebindTarget.FinishRebind();
    }
}