using LethalEmotesApi.Ui.Customize.DragDrop;
using LethalEmotesApi.Ui.Customize.Preview;
using LethalEmotesApi.Ui.Elements.Recycle;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LethalEmotesApi.Ui.Customize.List;

[RequireComponent(typeof(RectTransform))]
public class EmoteListItem : UIBehaviour, IBeginDragHandler, IDragHandler, IPointerEnterHandler, IPointerExitHandler, IRecycleViewItem<string>
{
    public TextMeshProUGUI? label;
    public EmoteBlacklistToggle? blacklistToggle;
    public EmoteVisibilityToggle? visibilityToggle;
    public EmoteKeybindButton? emoteKeybind;
    public Image? selectImage;
    public TextMeshProUGUI? modLabel;
    public EmoteDragDropController? dragDropController;
    public PreviewController? previewController;

    private RectTransform? _rectTransform;

    public int ConstraintIndex { get; set; }
    
    public RectTransform RectTransform => _rectTransform!;
    
    public string? EmoteKey { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        _rectTransform = GetComponent<RectTransform>();
    }

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

    private void UpdateState()
    {
        if (EmoteKey is null)
            return;
        
        if (label is null)
            return;
        
        label.SetText(EmoteUiManager.GetEmoteName(EmoteKey));

        if (modLabel is null)
            return;
        
        modLabel.SetText(EmoteUiManager.GetEmoteModName(EmoteKey));

        if (blacklistToggle is null || visibilityToggle is null || emoteKeybind is null)
            return;

        blacklistToggle.SetEmoteKey(EmoteKey);
        visibilityToggle.SetEmoteKey(EmoteKey);
        emoteKeybind.SetEmoteKey(EmoteKey);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (selectImage is null)
            return;

        selectImage.enabled = true;
        
        if (EmoteKey is null)
            return;
        
        dragDropController!.OnCanGrab();

        if (dragDropController.DragState != EmoteDragDropController.DragDropState.Ready)
            return;
        
        previewController!.PlayEmote(EmoteKey);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (selectImage is null)
            return;

        selectImage.enabled = false;
        
        dragDropController!.OnNotGrab();
    }

    public void BindData(string emoteKey)
    {
        EmoteKey = emoteKey;
        UpdateState();
    }
}