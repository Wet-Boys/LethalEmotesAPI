using System;
using LethalEmotesApi.Ui.Animation;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace LethalEmotesApi.Ui.Wheel;

[DisallowMultipleComponent]
[RequireComponent(typeof(CanvasRenderer))]
[ExecuteAlways]
public class WheelSegmentGraphic : Graphic
{
    private readonly TweenRunner<Vector3Tween> _scaleTweenRunner = new();
    
    public int segmentCount = 8;
    public float segmentRotOffset = 22.5f;
    public float minRadius = 100f;
    public float maxRadius = 300f;

    protected WheelSegmentGraphic()
    {
        _scaleTweenRunner.Init(this);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        raycastTarget = false;
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        var vertColor = (Color32)color;

        float degPer = (float)(Math.PI * 2 / segmentCount);
        float currentRad = degPer + Mathf.Deg2Rad * segmentRotOffset;
        
        int vertIndex = 0;
        float step = (float)Math.PI / 180;
        
        Vector3 minLeft = CosSin(currentRad, minRadius);
        Vector3 minRight = CosSin(currentRad + (degPer), minRadius);
        Vector3 offset = -Vector3.Lerp(minLeft, minRight, 0.5f);
        
        for (float i = 0; i + step < degPer + step / 2; i += step)
        {
            float rad = currentRad + i;
            Vector3 curMin = CosSin(rad, minRadius) + offset;
            Vector3 curMax = CosSin(rad, maxRadius) + offset;
            
            rad = currentRad + i + step;
            Vector3 nextMin = CosSin(rad, minRadius) + offset;
            Vector3 nextMax = CosSin(rad, maxRadius) + offset;
            
            vh.AddVert(curMin, vertColor, new Vector2(0, 0));
            vh.AddVert(curMax, vertColor, new Vector2(0, 1));
            vh.AddVert(nextMin, vertColor, new Vector2(1, 0));
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

    public void TweenScale(Vector3 targetScale, float duration, bool ignoreTimeScale)
    {
        if (transform.localScale == targetScale)
        {
            _scaleTweenRunner.StopTween();
            return;
        }
        
        Vector3Tween tween = new Vector3Tween
        {
            Duration = duration,
            StartValue = transform.localScale,
            TargetValue = targetScale,
            IgnoreTimeScale = ignoreTimeScale
        };
        
        tween.AddOnChangedCallback(TweenScaleChanged);
        
        _scaleTweenRunner.StartTween(tween);
    }

    private void TweenScaleChanged(Vector3 scale)
    {
        transform.localScale = scale;
    }
}