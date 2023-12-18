using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LethalEmotesApi.Ui.Customize.List;

[DisallowMultipleComponent]
public class EmoteBlacklistToggle : UIBehaviour
{
    public Image? toggleImage;
    public Sprite? enabledSprite;
    public Sprite? disabledSprite;

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
        
        var sprite = GetSprite();
        if (sprite is null)
            return;

        toggleImage.sprite = sprite;
    }

    private Sprite? GetSprite()
    {
        return InBlacklist ? disabledSprite : enabledSprite;
    }
}