using UnityEngine;

namespace LethalEmotesApi.Ui.Customize.Preview;

public class PreviewRig : MonoBehaviour
{
    public Camera? cam;
    public void Orbit(float vInput, float hInput)
    {
        transform.Rotate(Vector3.right, vInput);
        transform.Rotate(Vector3.up, -hInput, Space.World);
    }

    public void Zoom(float dir)
    {
        Debug.Log("Current camera FOV: " + cam.fieldOfView);
        cam.fieldOfView -= dir;
    }
}