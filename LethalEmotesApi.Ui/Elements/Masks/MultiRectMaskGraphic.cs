using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LethalEmotesApi.Ui.Elements.Masks;

[DisallowMultipleComponent]
[RequireComponent(typeof(CanvasRenderer), typeof(Mask))]
[ExecuteAlways]
public class MultiRectMaskGraphic : Graphic
{
    public RectTransform? maskRoot;

    private void OnValidate()
    {
        UpdateState();
    }

    public void UpdateState()
    {
        SetVerticesDirty();
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        foreach (var mask in GetRectMasks())
        {
            
            var vertColor = mask.Color;
            var rect = mask.Rect;
            
            var bl = new Vector2(rect.xMin, rect.yMin);
            var tl = new Vector2(rect.xMin, rect.yMax);
            var tr = new Vector2(rect.xMax, rect.yMax);
            var br = new Vector2(rect.xMax, rect.yMin);
            
            vh.AddUIVertexQuad([
                new UIVertex { position = bl, color = vertColor },
                new UIVertex { position = tl, color = vertColor },
                new UIVertex { position = tr, color = vertColor },
                new UIVertex { position = br, color = vertColor },
            ]);
        }
    }

    private RectMask[] GetRectMasks()
    {
        if (maskRoot is null)
            return [];
        
        return GetRectMasksRecursive(maskRoot).ToArray();
        
        List<RectMask> GetRectMasksRecursive(RectTransform root)
        {
            var rectMasks = new List<RectMask>();
            
            for (int i = 0; i < root.childCount; i++)
            {
                var child = root.GetChild(i);

                var childRectTransform = child.gameObject.GetComponent<RectTransform>();
                if (childRectTransform is null)
                    continue;
            
                rectMasks.AddRange(GetRectMasksRecursive(childRectTransform));

                var childMask = child.gameObject.GetComponent<MultiRectMaskChild>();
                if (childMask is null)
                    continue;
                
                childMask.SetController(this);

                if (!childMask.isActiveAndEnabled)
                    continue;

                var rectMask = new RectMask(childMask.GetRectRelativeToMaskRoot(), childMask.color);
                rectMasks.Add(rectMask);
            }

            return rectMasks;
        }
    }
    
    private readonly struct RectMask(Rect rect, Color color)
    {
        public readonly Rect Rect = rect;
        public readonly Color Color = color;
    }
}