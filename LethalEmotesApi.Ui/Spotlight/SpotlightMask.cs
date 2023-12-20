using System;
using System.Collections.Generic;
using LethalEmotesApi.Ui.Spotlight.MaskShape;
using UnityEngine;
using UnityEngine.UI;

namespace LethalEmotesApi.Ui.Spotlight;

[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform), typeof(CanvasRenderer))]
[ExecuteAlways]
public class SpotlightMask : MaskableGraphic
{
    private readonly List<ISpotlightMaskShape> _spotlights = [];

    public void AddSpotlightShape(ISpotlightMaskShape maskShape)
    {
        _spotlights.Add(maskShape);
        
        SetVerticesDirty();
    }

    public void ClearAll()
    {
        _spotlights.Clear();
        
        SetVerticesDirty();
    }
    
    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        int vertexOffset = 0;
    
        foreach (var mask in _spotlights)
            mask.PopulateMesh(vh, ref vertexOffset);
    }
}