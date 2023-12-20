using System;
using UnityEngine;
using UnityEngine.UI;

namespace LethalEmotesApi.Ui.Spotlight.MaskShape;

public struct CircleSpotlightShape : ISpotlightMaskShape
{
    public Vector2 ScreenPos { get; set; } = Vector2.zero;
    public float Radius { get; set; } = 0f;
    public float ScaleFactor { get; set; } = 1f;
    public Color MaskColor { get; set; } = Color.white;
    
    public CircleSpotlightShape() { }
    
    public void PopulateMesh(in VertexHelper vh, ref int vertexOffset)
    {
        var vertColor = (Color32)MaskColor;

        const float circleRads = (float)Math.PI * 2f;
        const float step = (float)Math.PI / 180;
        
        Vector3 center = ScreenPos;

        for (float i = 0; i + step < circleRads; i += step)
        {
            float rad = i;
            Vector3 curMax = CosSin(rad, Radius) / ScaleFactor;
            
            rad = i + step;
            Vector3 nextMax = CosSin(rad, Radius) / ScaleFactor;
            
            vh.AddVert(center, vertColor, new Vector2(0, 0));
            vh.AddVert(curMax + center, vertColor, new Vector2(0, 1));
            vh.AddVert(center, vertColor, new Vector2(1, 0));
            vh.AddVert(nextMax + center, vertColor, new Vector2(1, 1));
            
            vh.AddTriangle(vertexOffset + 2, vertexOffset + 1, vertexOffset);
            vh.AddTriangle(vertexOffset + 3, vertexOffset + 1, vertexOffset + 2);
            
            vertexOffset += 4;
        }
    }
    
    private static Vector3 CosSin(float rad, float dist)
    {
        float x = Mathf.Cos(rad) * dist;
        float y = Mathf.Sin(rad) * dist;

        return new Vector3(x, y, 0);
    }
}