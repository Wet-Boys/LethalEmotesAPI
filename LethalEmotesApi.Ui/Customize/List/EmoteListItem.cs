using TMPro;
using UnityEngine.EventSystems;

namespace LethalEmotesApi.Ui.Customize.List;

public class EmoteListItem : UIBehaviour
{
    public TextMeshProUGUI? label;
    public EmoteBlacklistToggle? blacklistToggle;

    public string? EmoteKey { get; private set; }

    protected override void Start()
    {
        base.Start();
        UpdateState();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        UpdateState();
    }

    public void SetEmoteKey(string emoteKey)
    {
        EmoteKey = emoteKey;
        UpdateState();
    }

    private void UpdateState()
    {
        if (EmoteKey is null)
            return;
        
        if (label is null)
            return;
        
        label.SetText(EmoteUiManager.GetEmoteName(EmoteKey));

        if (blacklistToggle is null)
            return;

        blacklistToggle.SetEmoteKey(EmoteKey);
    }
}