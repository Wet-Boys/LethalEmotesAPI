using System.Linq;
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

        if (visibilityImage is null || enabledSprite is null || disabledSprite is null)
            return;
        
        visibilityImage.sprite = IsVisible ? enabledSprite : disabledSprite;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;
        
        Toggle();
    }
}