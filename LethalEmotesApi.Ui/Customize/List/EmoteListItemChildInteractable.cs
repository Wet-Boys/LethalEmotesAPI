using UnityEngine.EventSystems;

namespace LethalEmotesApi.Ui.Customize.List;

public abstract class EmoteListItemChildInteractable : UIBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public EmoteListItem? emoteListItem;
    
    protected string? currentEmoteKey;

    public void SetEmoteKey(string emoteKey)
    {
        currentEmoteKey = emoteKey;
        UpdateState();
    }

    private void UpdateStateBroadcast()
    {
        UpdateState();
    }

    public virtual void UpdateState() { }

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

    public void OnPointerEnter(PointerEventData eventData)
    {
        emoteListItem!.OnPointerExit(eventData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        emoteListItem!.OnPointerEnter(eventData);
    }
}