using System.Collections.Generic;
using LethalEmotesApi.Ui.Customize.DragDrop;
using LethalEmotesApi.Ui.Customize.Preview;
using LethalEmotesApi.Ui.Data;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LethalEmotesApi.Ui.Customize.Wheel;

[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform))]
public class CustomizeWheel : UIBehaviour, IBeginDragHandler, IDragHandler, IPointerEnterHandler, IPointerMoveHandler
{
    public PreviewController? previewController;
    public EmoteDragDropController? dragDropController;
    public ColorBlock colors;
    [Range(1, 2)] public float scaleMultiplier;
    public Material? segmentMaterial;
    public WheelDefaultButton? defaultButton;
    public float minDist = 100f;
    public List<CustomizeWheelSegment> wheelSegments = [];
    
    public EmoteChangedCallback OnEmoteChanged = new();
    public EmotesSwappedCallback OnEmotesSwapped = new();

    private string[] _emoteArray = [];
    private int _currentSegmentIndex = -1;
    
    private RectTransform? _rectTransform;

    public RectTransform RectTransform
    {
        get
        {
            _rectTransform ??= GetComponent<RectTransform>();
            return _rectTransform;
        }
    }

    protected override void Awake()
    {
        base.Awake();

        if (dragDropController is null)
            dragDropController = GetComponentInParent<EmoteDragDropController>();

        if (previewController is null)
            previewController = GetComponentInParent<CustomizePanel>().previewController;

        foreach (var segment in wheelSegments)
        {
            segment.colors = colors;
            segment.scaleMultiplier = scaleMultiplier;

            if (segmentMaterial is not null)
                segment.targetGraphic!.material = segmentMaterial;
            
            segment.ResetState();
        }
    }
    
    protected override void Start()
    {
        base.Start();
        
        ResetState();
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        ResetState();
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        StartDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        StartDrag(eventData);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        OnPointerMove(eventData);
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        if (dragDropController is null)
            return;
        
        RectTransformUtility.ScreenPointToLocalPointInRectangle(RectTransform, eventData.position,
            eventData.enterEventCamera, out var mousePos);

        var prevSegmentIndex = _currentSegmentIndex;
        DetectSegmentFromMouse(mousePos);
        
        if (_currentSegmentIndex < 0)
        {
            dragDropController.OnNotGrab();
            return;
        }
        
        dragDropController.OnCanGrab();

        if (dragDropController.DragState != EmoteDragDropController.DragDropState.Ready)
            return;
        if (previewController is null)
            return;
        if (prevSegmentIndex == _currentSegmentIndex)
            return;
        
        previewController.PlayEmote(_emoteArray[_currentSegmentIndex]);
    }

    private void StartDrag(PointerEventData eventData)
    {
        if (dragDropController is null)
            return;

        if (_currentSegmentIndex < 0)
        {
            dragDropController.OnNotGrab();
            return;
        }
        
        dragDropController.StartWheelGrab(_currentSegmentIndex, _emoteArray[_currentSegmentIndex], eventData);
        
        var go = dragDropController.gameObject;
        eventData.pointerDrag = go;
        ExecuteEvents.Execute(go, eventData, ExecuteEvents.dragHandler);
    }

    public void LoadEmoteData(EmoteWheelData emoteData)
    {
        if (defaultButton is null)
            return;
        
        defaultButton.SetDefault(emoteData.IsDefaultWheel());
        _emoteArray = emoteData.Emotes;
        
        ResetState();
    }

    public void DetectSegmentFromMouse(Vector2 mousePos)
    {
        var dist = Vector2.Distance(Vector2.zero, mousePos);
        if (dist < minDist)
        {
            DeSelectAll();
            return;
        }
        
        var rect = RectTransform.rect;
        if (mousePos.x > rect.xMax || mousePos.x < rect.xMin || mousePos.y > rect.yMax || mousePos.y < rect.yMin)
        {
            DeSelectAll();
            return;
        }
        
        var segmentIndex = GetClosestSegmentIndex(mousePos);
        if (segmentIndex != _currentSegmentIndex && _currentSegmentIndex >= 0)
            wheelSegments[_currentSegmentIndex].DeSelect();

        _currentSegmentIndex = segmentIndex;
        SelectSegment();
    }

    public void DropEmote(string emoteKey)
    {
        if (_currentSegmentIndex < 0)
            return;
        
        wheelSegments[_currentSegmentIndex].DeSelect();
        OnEmoteChanged.Invoke(_currentSegmentIndex, emoteKey);
    }

    public void SwapSegmentEmotes(int fromIndex)
    {
        if (fromIndex < 0)
            return;
        
        if (_currentSegmentIndex < 0)
        {
            OnEmoteChanged.Invoke(fromIndex, "none");
            return;
        }

        (_emoteArray[_currentSegmentIndex], _emoteArray[fromIndex]) = (_emoteArray[fromIndex], _emoteArray[_currentSegmentIndex]);
        OnEmotesSwapped.Invoke(fromIndex, _currentSegmentIndex);
    }

    private void SelectSegment()
    {
        if (_currentSegmentIndex < 0)
            return;
        
        wheelSegments[_currentSegmentIndex].Select();
    }
    
    public void DeSelectAll()
    {
        _currentSegmentIndex = -1;
        foreach (var segment in wheelSegments)
            segment.DeSelect();
    }
    
    public void ResetState()
    {
        _currentSegmentIndex = -1;
        foreach (var segment in wheelSegments)
            segment.ResetState();
        
        UpdateLabels();
    }
    
    private int GetClosestSegmentIndex(Vector2 mousePos)
    {
        int segmentIndex = -1;
        float shortestDist = float.MaxValue;
        
        for (var i = 0; i < wheelSegments.Count; i++)
        {
            var emoteWheelSegment = wheelSegments[i];
            Vector2 pos = emoteWheelSegment.segmentRectTransform!.position - RectTransform.position;

            float distToMouse = Vector2.Distance(pos, mousePos);
            if (!(distToMouse < shortestDist))
                continue;

            shortestDist = distToMouse;
            segmentIndex = i;
        }
        
        return segmentIndex;
    }

    private void UpdateLabels()
    {
        if (_emoteArray.Length != wheelSegments.Count)
            return;
        
        for (var i = 0; i < wheelSegments.Count; i++)
            wheelSegments[i].SetLabel(_emoteArray[i]);
    }

    public class EmoteChangedCallback : UnityEvent<int, string>;
    public class EmotesSwappedCallback : UnityEvent<int, int>;
}