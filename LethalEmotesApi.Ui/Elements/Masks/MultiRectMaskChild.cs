using UnityEngine;
using UnityEngine.EventSystems;

namespace LethalEmotesApi.Ui.Elements.Masks;

[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform))]
[ExecuteAlways]
public class MultiRectMaskChild : UIBehaviour
{
    public Color color;

    private RectTransform? _rectTransform;
    
    public RectTransform RectTransform
    {
        get
        {
            if (_rectTransform is null)
                _rectTransform = GetComponent<RectTransform>();

            return _rectTransform;
        }
    }

    private MultiRectMaskGraphic? _controller;

    public void SetController(MultiRectMaskGraphic controller) => _controller = controller;

    public Rect GetRectRelativeToMaskRoot()
    {
        var rectTransform = RectTransform;
        
        if (_controller is null)
            return rectTransform.rect;

        var rect = rectTransform.rect;
        rect.position += (Vector2)rectTransform.localPosition;

        if (_controller.maskRoot is null)
            return rect;

        var parent = transform.parent;
        while (parent is not null && parent != _controller.maskRoot?.transform)
        {
            rect.position += (Vector2)parent.localPosition;
            parent = parent.parent;
        }

        return rect;
    }

    private void OnValidate()
    {
        UpdateState();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        
        UpdateState();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        
        UpdateState();
    }

    protected override void OnRectTransformDimensionsChange()
    {
        base.OnRectTransformDimensionsChange();
        
        UpdateState();
    }

    protected override void OnTransformParentChanged()
    {
        base.OnTransformParentChanged();
        
        UpdateState();
    }

    private void UpdateState()
    {
        if (_controller is null)
            return;
        
        _controller.UpdateState();
    }
}