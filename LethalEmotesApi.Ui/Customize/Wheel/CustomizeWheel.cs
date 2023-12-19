using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LethalEmotesApi.Ui.Customize.Wheel;

[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform))]
public class CustomizeWheel : UIBehaviour
{
    public ColorBlock colors;
    [Range(1, 2)] public float scaleMultiplier;
    public Material? segmentMaterial;
    public float minDist = 100f;
    public List<CustomizeWheelSegment> wheelSegments = [];
    public EmoteChangedCallback OnEmoteChanged = new();

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

    public void LoadEmoteData(string[] emoteArray)
    {
        _emoteArray = emoteArray;
        
        ResetState();
    }

    public void OnDropPointMove(Vector2 mousePos)
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
        
        OnEmoteChanged.Invoke(_currentSegmentIndex, emoteKey);
        wheelSegments[_currentSegmentIndex].DeSelect();
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
}