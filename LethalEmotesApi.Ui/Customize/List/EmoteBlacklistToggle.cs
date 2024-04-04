using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LethalEmotesApi.Ui.Customize.List;

[DisallowMultipleComponent]
public class EmoteBlacklistToggle : EmoteListItemChildInteractable, IPointerClickHandler
{
    public Image? toggleImage;
    public EmoteVisibilityToggle? visibilityToggle;

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
        
        if (toggleImage is null)
            return;
        
        toggleImage.enabled = InBlacklist;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;
        
        Toggle();
    }
}