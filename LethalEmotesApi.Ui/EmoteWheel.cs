using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LethalEmotesApi.Ui;

public class EmoteWheel : MonoBehaviour, IPointerMoveHandler
{
    public ColorBlock colors;
    [Range(1, 2)] public float scaleMultiplier;
    
    public int segmentCount = 8;
    public float segmentRotOffset = 22.5f;
    public float minRadius = 100f;
    public float maxRadius = 300f;
    
    public List<EmoteWheelSegment> wheelSegments = [];
    public string[] emoteArray;

    private int _currentSegmentIndex = -1;
    private RectTransform _rectTransform;
    private readonly EmoteSelectedCallback _emoteSelectedCallback = new();

    private bool _focused;
    public bool Focused
    {
        get => _focused;
        set
        {
            _focused = value;
            foreach (var segment in wheelSegments)
                segment.focused = _focused;

            if (!_focused)
                _emoteSelectedCallback.RemoveAllListeners();
        }
    }

    protected EmoteWheel()
    {
        emoteArray = new string[segmentCount];
    }

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        
        foreach (var segment in wheelSegments)
        {
            segment.colors = colors;
            segment.scaleMultiplier = scaleMultiplier;

            segment.targetGraphic!.segmentCount = segmentCount;
            segment.targetGraphic!.segmentRotOffset = segmentRotOffset;
            segment.targetGraphic!.minRadius = minRadius;
            segment.targetGraphic!.maxRadius = maxRadius;
            
            segment.ResetState();
        }
    }

    private void OnEnable()
    {
        ResetAll();
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        if (!_focused)
            return;
        
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_rectTransform, eventData.position,
            eventData.enterEventCamera, out var mousePos);

        var rect = _rectTransform.rect;

        var dist = Vector2.Distance(Vector2.zero, mousePos);
        if (dist < minRadius)
        {
            DeSelectAll();
            _emoteSelectedCallback.Invoke(EmoteWheelManager.EmoteNone);
            return;
        }

        if (mousePos.x > rect.xMax || mousePos.x < rect.xMin || mousePos.y > rect.yMax || mousePos.y < rect.yMin)
        {
            DeSelectAll();
            _emoteSelectedCallback.Invoke("");
            return;
        }
        
        var segmentIndex = GetClosestSegmentIndex(mousePos);
        if (segmentIndex == _currentSegmentIndex)
            return;
        
        if (_currentSegmentIndex > -1)
            wheelSegments[_currentSegmentIndex].OnPointerExit(eventData);
            
        _currentSegmentIndex = segmentIndex;
        wheelSegments[segmentIndex].OnPointerEnter(eventData);
        
        _emoteSelectedCallback.Invoke(emoteArray[_currentSegmentIndex]);
    }

    public void DeSelectAll()
    {
        _currentSegmentIndex = -1;
        foreach (var segment in wheelSegments)
            segment.DeSelect();
    }

    public void ResetAll()
    {
        _currentSegmentIndex = -1;
        foreach (var segment in wheelSegments)
            segment.ResetState();
    }

    public void LoadEmotes(string[] emotes)
    {
        emoteArray = emotes;
    }

    private int GetClosestSegmentIndex(Vector2 mousePos)
    {
        int segmentIndex = -1;
        float shortestDist = float.MaxValue;
        
        for (var i = 0; i < wheelSegments.Count; i++)
        {
            var emoteWheelSegment = wheelSegments[i];
            Vector2 pos = emoteWheelSegment.segmentRectTransform!.position;

            float distToMouse = Vector2.Distance(pos, mousePos);
            if (!(distToMouse < shortestDist))
                continue;

            shortestDist = distToMouse;
            segmentIndex = i;
        }
        
        return segmentIndex;
    }

    public void AddOnEmoteSelectedCallback(UnityAction<string> callback)
    {
        _emoteSelectedCallback.AddListener(callback);
    }
    

    private class EmoteSelectedCallback : UnityEvent<string>;
}