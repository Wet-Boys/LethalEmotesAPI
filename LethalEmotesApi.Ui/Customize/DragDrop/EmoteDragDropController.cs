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
    public Texture2D? cursorGrabTex;
    public Texture2D? cursorGrabbingTex;
    public Texture2D? cursorNormalTex;
    
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

    public void OnCanGrab()
    {
        if (cursorGrabTex is null)
            return;

        if (_dragDropState != DragDropState.Ready)
            return;
        
        Cursor.SetCursor(cursorGrabTex, Vector2.zero, CursorMode.Auto);
    }

    public void OnGrabbing()
    {
        if (cursorGrabbingTex is null)
            return;

        if (_dragDropState != DragDropState.Dragging)
            return;
        
        Cursor.SetCursor(cursorGrabbingTex, Vector2.zero, CursorMode.Auto);
    }

    public void OnNotGrab()
    {
        if (cursorNormalTex is null)
            return;

        if (_dragDropState != DragDropState.Ready)
            return;
        
        Cursor.SetCursor(cursorNormalTex, Vector2.zero, CursorMode.Auto);
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
        
        OnGrabbing();
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
        OnNotGrab();
    }

    public void CancelDrag()
    {
        OnNotGrab();
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