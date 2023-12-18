using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LethalEmotesApi.Ui.Customize.List;

[DisallowMultipleComponent]
public class EmoteBlacklistToggle : UIBehaviour
{
    public Image? toggleImage;

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
    }
    
    private void UpdateState()
    {
        if (string.IsNullOrEmpty(_emoteKey))
            return;
        
        if (toggleImage is null)
            return;

        toggleImage.enabled = InBlacklist;
    }
    
}