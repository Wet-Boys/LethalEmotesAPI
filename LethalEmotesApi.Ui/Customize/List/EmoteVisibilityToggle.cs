using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LethalEmotesApi.Ui.Customize.List;

[DisallowMultipleComponent]
public class EmoteVisibilityToggle : EmoteListItemChildInteractable, IPointerClickHandler
{
    public Image? visibilityImage;
    public Sprite? enabledSprite;
    public Sprite? disabledSprite;
    public EmoteBlacklistToggle? blacklistToggle;
    public TextMeshProUGUI? tooltipLabel;
    
    private bool IsVisible => !EmoteUiManager.EmotePoolBlacklist.Contains(currentEmoteKey);

    public void Toggle()
    {
        if (string.IsNullOrEmpty(currentEmoteKey))
            return;

        if (IsVisible)
        {
            EmoteUiManager.AddToEmoteBlacklist(currentEmoteKey);
        }
        else
        {
            EmoteUiManager.RemoveFromEmoteBlacklist(currentEmoteKey);
        }
        
        UpdateState();

        if (blacklistToggle is null)
            return;
        
        blacklistToggle.UpdateState();
    }
    
    public override void UpdateState()
    {
        if (string.IsNullOrEmpty(currentEmoteKey))
            return;

        if (visibilityImage is null || enabledSprite is null || disabledSprite is null || tooltipLabel is null)
            return;
        
        visibilityImage.sprite = IsVisible ? enabledSprite : disabledSprite;

        tooltipLabel.text = $"{(IsVisible ? "Disable" : "Enable")} seeing this emote in-game";
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