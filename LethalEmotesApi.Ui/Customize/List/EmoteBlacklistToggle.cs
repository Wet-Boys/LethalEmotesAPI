using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LethalEmotesApi.Ui.Customize.List;

[DisallowMultipleComponent]
public class EmoteBlacklistToggle : UIBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public EmoteListItem? emoteListItem;
    public Image? toggleImage;
    public EmoteVisibilityToggle? visibilityToggle;

    private string? _emoteKey;
    private bool InBlacklist => EmoteUiManager.RandomPoolBlacklist.Contains(_emoteKey);

    protected override void Start()
    {
        base.Awake();
        UpdateState();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        UpdateState();
    }

    public void SetEmoteKey(string emoteKey)
    {
        _emoteKey = emoteKey;
        UpdateState();
    }

    public void Toggle()
    {
        if (string.IsNullOrEmpty(_emoteKey))
            return;
        
        if (InBlacklist)
        {
            EmoteUiManager.RemoveFromRandomPoolBlacklist(_emoteKey);
        }
        else
        {
            EmoteUiManager.AddToRandomPoolBlacklist(_emoteKey);
        }
        
        UpdateState();

        if (visibilityToggle is null)
            return;
        
        visibilityToggle.UpdateState();
    }
    private void UpdateStateBroadcast()
    {
        UpdateState();
    }
    public void UpdateState()
    {
        if (string.IsNullOrEmpty(_emoteKey))
            return;
        
        if (toggleImage is null)
            return;
        toggleImage.enabled = InBlacklist;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        emoteListItem!.OnPointerExit(eventData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        emoteListItem!.OnPointerEnter(eventData);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;
        
        Toggle();
    }
}