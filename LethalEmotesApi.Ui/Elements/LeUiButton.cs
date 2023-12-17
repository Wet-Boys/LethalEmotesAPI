using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LethalEmotesApi.Ui.Elements;

[DisallowMultipleComponent]
public class LeUiButton : UIBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    public Graphic? targetGraphic;
    public LeUiScaleTweener? scaleTweener;
    public ColorBlock colors;
    [Range(0, 2)] public float scaleSelected;
    [Range(0, 2)] public float scalePressed;

    public UnityEvent onClick = new();

    private bool _selected;
    private bool _pressed;
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        _selected = true;
        UpdateState();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _selected = false;
        UpdateState();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _pressed = _selected;
        UpdateState();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _pressed = false;
        UpdateState();
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        onClick.Invoke();
    }

    private Color GetColor()
    {
        if (_pressed)
            return colors.pressedColor;

        if (_selected)
            return colors.selectedColor;

        return colors.normalColor;
    }

    private float GetScale()
    {
        if (_pressed)
            return scalePressed;

        if (_selected)
            return scaleSelected;

        return 1f;
    }
    
    private void UpdateState(bool instant = false)
    {
        var color = GetColor();
        targetGraphic!.CrossFadeColor(color, instant ? 0f : colors.fadeDuration, true, true);

        var scale = GetScale();
        var scaleVec = new Vector3(scale, scale, scale);
        scaleTweener!.TweenScale(scaleVec, instant ? 0f : colors.fadeDuration, true);
    }
}