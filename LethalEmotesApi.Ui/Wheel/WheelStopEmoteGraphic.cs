using System;
using UnityEngine;
using UnityEngine.UI;

namespace LethalEmotesApi.Ui.Wheel;

[DisallowMultipleComponent]
[RequireComponent(typeof(CanvasRenderer))]
[ExecuteAlways]
public class WheelStopEmoteGraphic : Graphic
{
    public float radius = 95f;

    protected override void OnEnable()
    {
        base.OnEnable();
        raycastTarget = false;
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();
        
        var vertColor = (Color32)color;

        float circleRads = (float)Math.PI * 2f;
        int vertIndex = 0;
        float step = (float)Math.PI / 180;

        for (float i = 0; i + step < circleRads; i += step)
        {

            float rad = i;
            Vector3 curMax = CosSin(rad, radius);
            
            rad = i + step;
            Vector3 nextMax = CosSin(rad, radius);
            
            vh.AddVert(Vector3.zero, vertColor, new Vector2(0, 0));
            vh.AddVert(curMax, vertColor, new Vector2(0, 1));
            vh.AddVert(Vector3.zero, vertColor, new Vector2(1, 0));
            vh.AddVert(nextMax, vertColor, new Vector2(1, 1));
            
            vh.AddTriangle(vertIndex + 2, vertIndex + 1, vertIndex);
            vh.AddTriangle(vertIndex + 3, vertIndex + 1, vertIndex + 2);

            vertIndex += 4;
        }
    }
    
    private static Vector3 CosSin(float rad, float dist)
    {
        float x = (float)(Math.Cos(rad) * dist);
        float y = (float)(Math.Sin(rad) * dist);

        return new Vector3(x, y, 0);
    }
}