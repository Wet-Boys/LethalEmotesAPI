using LethalEmotesApi.Ui.Customize.DragDrop;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace LethalEmotesApi.Ui.Customize.List;

public class EmoteListItem : UIBehaviour, IBeginDragHandler, IDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    public TextMeshProUGUI? label;
    public EmoteBlacklistToggle? blacklistToggle;
    public EmoteDragDropController? dragDropController;
    
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
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        StartDrag(eventData);
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        StartDrag(eventData);
    }

    private void StartDrag(PointerEventData eventData)
    {
        if (EmoteKey is null)
            EmoteKey = label!.text;
        
        dragDropController!.StartDrag(EmoteKey, eventData);
        
        var go = dragDropController.gameObject;
        eventData.pointerDrag = go;
        ExecuteEvents.Execute(go, eventData, ExecuteEvents.dragHandler);
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

    public void OnPointerEnter(PointerEventData eventData)
    {
        dragDropController!.OnCanGrab();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        dragDropController!.OnNotGrab();
    }
}