using LethalEmotesApi.Ui.Customize.Wheel;
using UnityEngine;
using UnityEngine.EventSystems;

namespace LethalEmotesApi.Ui.Customize.DragDrop;

[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform))]
public class EmoteDragDropController : UIBehaviour, IPointerMoveHandler, IPointerClickHandler, IDragHandler, IEndDragHandler
{
    public RectTransform? dragDropRectTransform;
    public DragDropItem? dragDropItem;
    public CustomizeWheelController? customizeWheelController;
    
    private DragDropState _dragDropState = DragDropState.Ready;
    private string? _emoteKey;
    private RectTransform? _rectTransform;

    protected override void Awake()
    {
        base.Awake();

        if (_rectTransform is null)
            _rectTransform = GetComponent<RectTransform>();
    }

    protected override void Start()
    {
        base.Start();

        if (_rectTransform is null)
            _rectTransform = GetComponent<RectTransform>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        
        if (_rectTransform is null)
            _rectTransform = GetComponent<RectTransform>();
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        UpdateDragPos(eventData);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        StopDrag();
    }

    public void OnDrag(PointerEventData eventData)
    {
        UpdateDragPos(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        StopDrag();
    }
    
    public void StartDrag(string emoteKey, PointerEventData eventData)
    {
        if (_dragDropState != DragDropState.Ready)
            return;
        
        if (customizeWheelController is null)
            return;

        _dragDropState = DragDropState.Dragging;
        _emoteKey = emoteKey;
        
        dragDropItem!.SetEmoteKey(emoteKey);

        UpdateDragPos(eventData);
        dragDropRectTransform!.gameObject.SetActive(true);
    }

    private void UpdateDragPos(PointerEventData eventData)
    {
        if (_dragDropState != DragDropState.Dragging)
            return;

        var customizeWheel = GetCustomizeWheel();
        
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_rectTransform, eventData.position,
            eventData.enterEventCamera, out var mousePos);
        
        dragDropRectTransform!.localPosition = new Vector3(mousePos.x, mousePos.y, 0);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(customizeWheel!.RectTransform, eventData.position,
            eventData.enterEventCamera, out var wheelMousePos);
        
        customizeWheel.OnDropPointMove(wheelMousePos);
    }

    public void StopDrag()
    {
        if (_dragDropState != DragDropState.Dragging)
            return;
        
        var customizeWheel = GetCustomizeWheel();
        if (_emoteKey is null || customizeWheel is null)
            return;

        _dragDropState = DragDropState.Dropping;
        dragDropRectTransform!.gameObject.SetActive(false);
        
        customizeWheel.DropEmote(_emoteKey);

        _dragDropState = DragDropState.Ready;
    }

    public void CancelDrag()
    {
        if (_dragDropState != DragDropState.Dragging)
            return;
        
        var customizeWheel = GetCustomizeWheel();
        if (_emoteKey is null || customizeWheel is null)
            return;
        
        customizeWheel.ResetState();
        dragDropRectTransform!.gameObject.SetActive(false);
        _dragDropState = DragDropState.Ready;
    }

    private CustomizeWheel? GetCustomizeWheel()
    {
        if (customizeWheelController is null)
            return null;

        return customizeWheelController.customizeWheel;
    }
    
    private enum DragDropState
    {
        Ready,
        Dragging,
        Dropping
    }
}