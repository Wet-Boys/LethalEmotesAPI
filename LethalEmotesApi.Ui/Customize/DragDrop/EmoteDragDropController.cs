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

    private string? _emoteKey;
    private int _segmentIndex = -1;
    private RectTransform? _rectTransform;

    public DragDropState DragState { get; private set; } = DragDropState.Ready;

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

    protected override void OnDisable()
    {
        base.OnDisable();
        
        OnNotGrab();
    }

    public void OnCanGrab()
    {
        if (cursorGrabTex is null)
            return;

        if (DragState != DragDropState.Ready)
            return;
        
        Cursor.SetCursor(cursorGrabTex, Vector2.zero, CursorMode.Auto);
    }

    public void OnGrabbing()
    {
        if (cursorGrabbingTex is null)
            return;

        if (DragState != DragDropState.Dragging)
            return;
        
        Cursor.SetCursor(cursorGrabbingTex, Vector2.zero, CursorMode.Auto);
    }

    public void OnNotGrab()
    {
        if (cursorNormalTex is null)
            return;

        if (DragState != DragDropState.Ready)
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
        if (DragState != DragDropState.Ready)
            return;
        
        if (customizeWheelController is null)
            return;

        DragState = DragDropState.Dragging;
        _segmentIndex = -1;
        _emoteKey = emoteKey;
        
        dragDropItem!.SetEmoteKey(emoteKey);

        UpdateDragPos(eventData);
        dragDropRectTransform!.gameObject.SetActive(true);
        OnGrabbing();
    }

    public void StartWheelGrab(int fromIndex, string emoteKey, PointerEventData eventData)
    {
        if (DragState != DragDropState.Ready)
            return;
        
        if (customizeWheelController is null)
            return;

        DragState = DragDropState.Dragging;
        _segmentIndex = fromIndex;
        _emoteKey = emoteKey;
        
        dragDropItem!.SetEmoteKey(emoteKey);
        
        UpdateDragPos(eventData);
        dragDropRectTransform!.gameObject.SetActive(true);
        OnGrabbing();
    }

    private void UpdateDragPos(PointerEventData eventData)
    {
        if (DragState != DragDropState.Dragging)
            return;

        var customizeWheel = GetCustomizeWheel();
        
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_rectTransform, eventData.position,
            eventData.enterEventCamera, out var mousePos);
        
        dragDropRectTransform!.localPosition = new Vector3(mousePos.x, mousePos.y, 0);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(customizeWheel!.RectTransform, eventData.position,
            eventData.enterEventCamera, out var wheelMousePos);
        
        customizeWheel.DetectSegmentFromMouse(wheelMousePos);
    }

    public void StopDrag()
    {
        if (DragState != DragDropState.Dragging)
            return;
        
        var customizeWheel = GetCustomizeWheel();
        if (_emoteKey is null || customizeWheel is null)
            return;

        DragState = DragDropState.Dropping;
        dragDropRectTransform!.gameObject.SetActive(false);

        if (_segmentIndex < 0)
        {
            customizeWheel.DropEmote(_emoteKey);
        }
        else
        {
            customizeWheel.SwapSegmentEmotes(_segmentIndex);
        }

        DragState = DragDropState.Ready;
        _segmentIndex = -1;

        OnNotGrab();
    }

    public void CancelDrag()
    {
        OnNotGrab();
        
        if (DragState != DragDropState.Dragging)
            return;
        
        var customizeWheel = GetCustomizeWheel();
        if (_emoteKey is null || customizeWheel is null)
            return;
        
        customizeWheel.ResetState();
        dragDropRectTransform!.gameObject.SetActive(false);
        DragState = DragDropState.Ready;
        _segmentIndex = -1;
    }

    private CustomizeWheel? GetCustomizeWheel()
    {
        if (customizeWheelController is null)
            return null;

        return customizeWheelController.customizeWheel;
    }

    public enum DragDropState
    {
        Ready,
        Dragging,
        Dropping
    }
}