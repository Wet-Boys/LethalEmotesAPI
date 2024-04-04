using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LethalEmotesApi.Ui.Customize.List;

[DisallowMultipleComponent]
public class EmoteBlacklistToggle : EmoteListItemChildInteractable, IPointerClickHandler
{
    public Image? toggleImage;
    public EmoteVisibilityToggle? visibilityToggle;
    public TextMeshProUGUI? tooltipLabel;

    private bool InBlacklist => EmoteUiManager.RandomPoolBlacklist.Contains(currentEmoteKey);

    public void Toggle()
    {
        if (string.IsNullOrEmpty(currentEmoteKey))
            return;
        
        if (InBlacklist)
        {
            EmoteUiManager.RemoveFromRandomPoolBlacklist(currentEmoteKey);
        }
        else
        {
            EmoteUiManager.AddToRandomPoolBlacklist(currentEmoteKey);
        }
        
        UpdateState();

        if (visibilityToggle is null)
            return;
        
        visibilityToggle.UpdateState();
    }
    
    public override void UpdateState()
    {
        if (string.IsNullOrEmpty(currentEmoteKey))
            return;
        
        if (toggleImage is null || tooltipLabel is null)
            return;
        
        toggleImage.enabled = InBlacklist;

        tooltipLabel.text = $"{(InBlacklist ? "Add to" : "Remove from")} random pool";
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        
        if (tooltip is null)
            return;
        
        tooltip.SetActive(true);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        
        if (tooltip is null)
            return;
        
        tooltip.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;
        
        Toggle();
    }
}