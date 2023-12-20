using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace LethalEmotesApi.Ui.Spotlight;

[DisallowMultipleComponent]
[ExecuteAlways]
public class SpotlightController : MonoBehaviour
{
    public SpotlightMask? spotlightMask;
    public Canvas? canvas;
    private readonly List<SpotlightBase> _spotlights = [];

    private void OnValidate()
    {
        if (canvas is null)
            canvas = GetComponentInParent<Canvas>();
        
        UpdateMask();
    }

    private void Awake()
    {
        if (canvas is null)
            canvas = GetComponentInParent<Canvas>();
    }

    private void Start()
    {
        if (canvas is null)
            canvas = GetComponentInParent<Canvas>();
        
        UpdateMask();
    }

    public void RegisterSpotlight(SpotlightBase spotlight)
    {
        if (_spotlights.Contains(spotlight))
            return;
        
        _spotlights.Add(spotlight);
        UpdateMask();
    }

    public void UnregisterSpotlight(SpotlightBase spotlight)
    {
        _spotlights.Remove(spotlight);
        UpdateMask();
    }

    public void UpdateMask()
    {
        if (spotlightMask is null)
            return;
        
        if (canvas is null)
            return;

        var scaleFactor = canvas.scaleFactor;
        
        spotlightMask.ClearAll();

        foreach (var spotlight in _spotlights)
            spotlightMask.AddSpotlightShape(spotlight.GetMaskShape(scaleFactor));
    }
}