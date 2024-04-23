using System;
using LethalEmotesApi.Ui.Animation;
using UnityEngine;

namespace LethalEmotesApi.Ui.Customize.Preview;

public class PreviewRig : MonoBehaviour
{
    private readonly TweenRunner<FloatTween> _zoomTweenRunner = new();

    public Vector3 initialEulerAngles;
    public PreviewEmoteRenderer? emoteRenderer;

    protected PreviewRig()
    {
        _zoomTweenRunner.Init(this);
    }
    
    public void ResetControls()
    {
        transform.localEulerAngles = initialEulerAngles;
    }

    public void Orbit(float vInput, float hInput)
    {
        transform.Rotate(Vector3.right, vInput);
        transform.Rotate(Vector3.up, hInput, Space.World);
        var x = ClampAngle(transform.localEulerAngles.x, -60, 60);
        transform.localEulerAngles = new Vector3(x, transform.localEulerAngles.y, transform.localEulerAngles.z);
    }

    // https://forum.unity.com/threads/limiting-rotation-with-mathf-clamp.171294/ - thread where I borrowed the code
    private static float ClampAngle(float angle, float min, float max)
    {
        if (min < 0 && max > 0 && (angle > max || angle < min))
        {
            angle -= 360;
            if (angle > max || angle < min)
            {
                if (Mathf.Abs(Mathf.DeltaAngle(angle, min)) < Mathf.Abs(Mathf.DeltaAngle(angle, max)))
                    return min;
                
                return max;
            }
        }
        else if(min > 0 && (angle > max || angle < min))
        {
            angle += 360;
            if (angle > max || angle < min)
            {
                if (Mathf.Abs(Mathf.DeltaAngle(angle, min)) < Mathf.Abs(Mathf.DeltaAngle(angle, max)))
                    return min;
                
                return max;
            }
        }
 
        if (angle < min)
            return min;
        
        if (angle > max)
            return max;
        
        return angle;
    }
    
    
    public void Zoom(float dir)
    {
        if (emoteRenderer is null)
            return;
        
        var fov = emoteRenderer.Fov;
        //Debug.Log("Current camera FOV: " + cam.fieldOfView);
        fov -= dir;
        fov = Mathf.Clamp(fov, 2, 154);
        var whatever = new FloatTween()
        {
            Duration = 0.2f,
            StartValue = emoteRenderer.Fov,
            TargetValue = fov,
            IgnoreTimeScale = true
        };
        whatever.AddOnChangedCallback(CameraFovTween);
        _zoomTweenRunner.StartTween(whatever);
    }

    private void CameraFovTween(float fov)
    {
        if (emoteRenderer is null)
            return;
        
        emoteRenderer.Fov = fov;
    }
}