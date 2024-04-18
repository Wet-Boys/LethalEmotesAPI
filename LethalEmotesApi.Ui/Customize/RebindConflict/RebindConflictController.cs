using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LethalEmotesApi.Ui.Customize.RebindConflict;

public class RebindConflictController : MonoBehaviour
{
    private static RebindConflictController? _instance;

    public GameObject? conflictItemPrefab;
    public Transform? conflictContainer;
    public CanvasGroup? canvasGroup;

    private InputActionReference? _target;
    private readonly List<InputActionReference> _conflictingRefs = [];
    private readonly List<GameObject> _conflictItems = [];

    private void Init(InputActionReference target, string[] emoteKeys)
    {
        if (conflictItemPrefab is null || conflictContainer is null)
            return;

        _target = target;

        foreach (var emoteKey in emoteKeys)
        {
            var emoteName = EmoteUiManager.GetEmoteName(emoteKey);
            var inputRef = EmoteUiManager.GetEmoteKeybind(emoteKey);

            if (inputRef is null)
                continue;
            
            var instance = Instantiate(conflictItemPrefab, conflictContainer);
            _conflictItems.Add(instance);
            
            var conflictItem = instance.GetComponent<RebindConflictItem>();
            
            conflictItem.SetLabelText(emoteName);
            
            _conflictingRefs.Add(inputRef);
        }
        
        Show();
    }

    public void Confirm()
    {
        if (_target is null)
            return;
        
        foreach (var inputRef in _conflictingRefs)
            inputRef.action.RemoveAllBindingOverrides();
        
        EmoteUiManager.SaveKeybinds();
        
        EmoteUiManager.EmoteUiInstance!.customizePanel!.gameObject.BroadcastMessage("UpdateStateBroadcast");
        
        Hide();
    }

    public void Cancel()
    {
        if (_target is null)
            return;
        
        _target.action.RemoveAllBindingOverrides();
        
        EmoteUiManager.LoadKeybinds();
        EmoteUiManager.SaveKeybinds();
        
        EmoteUiManager.EmoteUiInstance!.customizePanel!.gameObject.BroadcastMessage("UpdateStateBroadcast");
        
        Hide();
    }

    private void Awake()
    {
        _instance = this;
        
        Hide();
    }

    private void OnDisable()
    {
        Hide();
    }

    private void OnDestroy()
    {
        _instance = null;
    }

    private void Hide()
    {
        _target = null;
        _conflictingRefs.Clear();
        
        if (canvasGroup is null)
            return;
        
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        foreach (var instance in _conflictItems)
            Destroy(instance);
        
        _conflictItems.Clear();
    }

    private void Show()
    {
        if (canvasGroup is null)
            return;
        
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    public static void CreateConflictModal(string targetEmoteKey, string[] emoteKeys)
    {
        if (_instance is null || _instance._target is not null)
            return;

        var target = EmoteUiManager.GetEmoteKeybind(targetEmoteKey);
        if (target is null)
            return;
        
        _instance.Init(target, emoteKeys);
    }

    public static void CancelExisting()
    {
        if (_instance is null)
            return;
        
        _instance.Cancel();
    }
}