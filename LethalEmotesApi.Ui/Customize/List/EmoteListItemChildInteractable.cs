using UnityEngine;
using UnityEngine.EventSystems;

namespace LethalEmotesApi.Ui.Customize.List;

public abstract class EmoteListItemChildInteractable : UIBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public EmoteListItem? emoteListItem;
    public GameObject? tooltip;
    
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
        
        if (tooltip is null)
            return;
        
        tooltip.SetActive(false);
        
        UpdateState();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        
        if (tooltip is null)
            return;
        
        tooltip.SetActive(false);
    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        emoteListItem!.OnPointerExit(eventData);
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        emoteListItem!.OnPointerEnter(eventData);
    }
}