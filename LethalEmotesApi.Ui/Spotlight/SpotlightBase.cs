using System;
using LethalEmotesApi.Ui.Spotlight.MaskShape;
using UnityEngine;
using UnityEngine.EventSystems;

namespace LethalEmotesApi.Ui.Spotlight;

[RequireComponent(typeof(RectTransform))]
[ExecuteAlways]
public abstract class SpotlightBase : UIBehaviour
{
    public Vector2 ScreenPos => transform.localPosition;
    public Color maskColor = Color.white;

    private Vector3 _localPos;
    private SpotlightController? _spotlightController;

    protected override void Awake()
    {
        base.Awake();

        _localPos = transform.localPosition;

        if (_spotlightController is null)
            _spotlightController = GetComponentInParent<SpotlightController>();
        
        _spotlightController.RegisterSpotlight(this);
    }

    protected void Update()
    {
        if (!transform.hasChanged)
            return;

        var localPos = transform.localPosition;
        if (_localPos == localPos)
            return;

        _localPos = localPos;
        OnPositionChanged();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        
        if (_spotlightController is null)
            _spotlightController = GetComponentInParent<SpotlightController>();
        
        _spotlightController.RegisterSpotlight(this);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        
        if (_spotlightController is null)
            return;
        
        _spotlightController!.UnregisterSpotlight(this);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        if (_spotlightController is null)
            return;
        
        _spotlightController!.UnregisterSpotlight(this);
    }

    private void OnPositionChanged()
    {
        _spotlightController!.UpdateMask();
    }

    public abstract ISpotlightMaskShape GetMaskShape(float scaleFactor);
}