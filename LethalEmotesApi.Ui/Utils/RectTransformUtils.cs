using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LethalEmotesApi.Ui.Utils;

internal static class RectTransformUtils
{
    private static Camera? _uiCamera;
    private static readonly Vector3[] Corners = new Vector3[4];

    private static Camera GetUiCamera()
    {
        if (_uiCamera is not null && _uiCamera)
            return _uiCamera;

        _uiCamera = null;
        
        var systems = SceneManager.GetActiveScene()
            .GetRootGameObjects()
            .FirstOrDefault(go => go.name == "Systems");
        
        if (systems is null)
        {
            Debug.LogWarning("Failed to find UICamera in active scene, falling back to Camera.current!");
            return Camera.current;
        }
        
        var uiCameraTransform = systems.transform.Find("UI/UICamera");
        if (uiCameraTransform is null)
        {
            Debug.LogWarning("Failed to find UICamera at MainMenu, falling back to Camera.current!");
            return Camera.current;
        }
        
        var uiCamera = uiCameraTransform.GetComponent<Camera>();
        if (uiCamera is null)
        {
            Debug.LogWarning("Failed to find Camera component on UICamera, falling back to Camera.current!");
            return Camera.current;
        }
            
        _uiCamera = uiCamera;

        return _uiCamera;
    }

    public static Rect GetRelativeRect(this RectTransform root, RectTransform worldRectTransform)
    {
        var camera = GetUiCamera();
        
        worldRectTransform.GetWorldCorners(Corners);

        var screenCorners = new Vector3[4];
        for (int i = 0; i < Corners.Length; i++)
            screenCorners[i] = RectTransformUtility.WorldToScreenPoint(camera, Corners[i]);

        var localCorners = new Vector2[4];
        for (int i = 0; i < screenCorners.Length; i++)
            RectTransformUtility.ScreenPointToLocalPointInRectangle(root, screenCorners[i], camera, out localCorners[i]);

        var min = Vector2.zero;
        var max = Vector2.zero;

        foreach (var corner in localCorners)
        {
            min = Vector2.Min(min, corner);
            max = Vector2.Max(max, corner);
        }
        
        var size = max - min;

        return new Rect(min.x, min.y, size.x, size.y);
    }
}