using UnityEngine;
using UnityEngine.Rendering;

namespace LethalEmotesApi.Ui.Customize.Preview;

[DisallowMultipleComponent]
[DefaultExecutionOrder(-4)]
public class PreviewEmoteRenderer : MonoBehaviour
{
    public MeshRenderer? backgroundRenderer;
    public SkinnedMeshRenderer? emoteSkinnedMeshRenderer;
    public RenderTexture? targetRenderTexture;
    
    [SerializeField]
    private float fov = 25f;

    [SerializeField]
    private float aspectRatio = 1f;
    
    [SerializeField]
    private float near = 0.3f;

    [SerializeField]
    private float far = 15f;

    public float Fov
    {
        get => fov;
        set
        {
            fov = value;
            UpdateProjMat();
        }
    }
    
    public float AspectRatio
    {
        get => aspectRatio;
        set
        {
            aspectRatio = value;
            UpdateProjMat();
        }
    }
    
    public float Near
    {
        get => near;
        set
        {
            near = value;
            UpdateProjMat();
        }
    }
    
    public float Far
    {
        get => far;
        set
        {
            far = value;
            UpdateProjMat();
        }
    }

    private Quaternion _lastRot;
    private Vector3 _lastPos;

    private Matrix4x4? _projMat;
    private Matrix4x4? _viewMat;

    public Matrix4x4 ProjMat
    {
        get
        {
            _projMat ??= Matrix4x4.Perspective(fov, aspectRatio, near, far);
            return _projMat.Value;
        }
    }

    public Matrix4x4 ViewMat
    {
        get
        {
            var camTransform = transform;
            
            if (_viewMat.HasValue && camTransform.rotation == _lastRot && camTransform.position == _lastPos)
                return _viewMat.Value;
            
            _lastRot = camTransform.rotation;
            _lastPos = camTransform.position;
            
            var viewMat = Matrix4x4.Rotate(Quaternion.Inverse(_lastRot)) *
                       Matrix4x4.Translate(-_lastPos);
            
            if (SystemInfo.usesReversedZBuffer)
            {
                viewMat.m20 = -viewMat.m20;
                viewMat.m21 = -viewMat.m21;
                viewMat.m22 = -viewMat.m22;
                viewMat.m23 = -viewMat.m23;
            }

            _viewMat = viewMat;
            
            return _viewMat.Value;
        }
    }

    private CommandBuffer? _cmdBuffer;

    private void Awake()
    {
        _cmdBuffer = new CommandBuffer();
    }

    private void OnValidate()
    {
        UpdateProjMat();
    }

    private void LateUpdate()
    {
        _cmdBuffer!.Clear();
        _cmdBuffer.SetViewProjectionMatrices(ViewMat, ProjMat);
        
        DrawWithMaterials(backgroundRenderer!);
        DrawWithMaterials(emoteSkinnedMeshRenderer!);
        
        Graphics.SetRenderTarget(targetRenderTexture);
        GL.Clear(true, true, Color.clear);
        Graphics.ExecuteCommandBuffer(_cmdBuffer);
    }

    private void DrawWithMaterials(Renderer renderer)
    {
        for (var i = 0; i < renderer.sharedMaterials.Length; i++)
        {
            var material = renderer.sharedMaterials[i];
            
            _cmdBuffer!.DrawRenderer(renderer, material, i, -1);
        }
    }

    private void OnDestroy()
    {
        _cmdBuffer?.Dispose();
    }
    
    private void UpdateProjMat()
    {
        _projMat = Matrix4x4.Perspective(fov, aspectRatio, near, far);
    }
}