using TMPro;
using UnityEngine.EventSystems;

namespace LethalEmotesApi.Ui.Customize.DragDrop;

public class DragDropItem : UIBehaviour
{
    public TextMeshProUGUI? label;
    
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
        
        try
        {
            label.SetText(EmoteUiManager.GetEmoteName(EmoteKey));
        }
        catch
        {
            label.SetText(EmoteKey);
        }
    }
}