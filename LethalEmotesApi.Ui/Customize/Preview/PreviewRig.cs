using System;
using LethalEmotesApi.Ui.Animation;
using UnityEngine;

namespace LethalEmotesApi.Ui.Customize.Preview;

public class PreviewRig : MonoBehaviour
{
    private readonly TweenRunner<FloatTween> _zoomTweenRunner = new();

    protected PreviewRig()
    {
        _zoomTweenRunner.Init(this);
    }
    
    
    public Camera? cam;
    public void Orbit(float vInput, float hInput)
    {
        transform.Rotate(Vector3.right, vInput);
        transform.Rotate(Vector3.up, -hInput, Space.World);
    }

    public void Zoom(float dir)
    {
        var fov = cam.fieldOfView;
        Debug.Log("Current camera FOV: " + cam.fieldOfView);
        fov -= dir;
        fov = Mathf.Clamp(fov, 2, 154);
        var whatever = new FloatTween()
        {
            Duration = 0.2f,
            StartValue = cam.fieldOfView,
            TargetValue = fov,
            IgnoreTimeScale = true
        };
        whatever.AddOnChangedCallback(CameraFovTween);
        _zoomTweenRunner.StartTween(whatever);
    }

    private void CameraFovTween(float fov)
    {
        cam.fieldOfView = fov;
    }
}