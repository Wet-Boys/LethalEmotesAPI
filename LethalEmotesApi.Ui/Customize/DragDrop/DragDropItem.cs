using LethalEmotesApi.Ui.Customize.List;
using UnityEngine.EventSystems;

namespace LethalEmotesApi.Ui.Customize.DragDrop;

public class DragDropItem : UIBehaviour
{
    public EmoteListItem? listItem;
    
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
        if (EmoteKey is null || listItem is null)
            return;
        
        listItem.SetEmoteKey(EmoteKey);
    }
}