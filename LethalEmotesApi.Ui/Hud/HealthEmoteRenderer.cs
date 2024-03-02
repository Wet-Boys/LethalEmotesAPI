using UnityEngine;
using UnityEngine.Rendering;

namespace LethalEmotesApi.Ui.Hud;

[DefaultExecutionOrder(-4)]
public class HealthEmoteRenderer : MonoBehaviour
{
    public SkinnedMeshRenderer? emoteSkinnedMeshRenderer;
    public Material? material;
    public RenderTexture? targetRenderTexture;

    private CommandBuffer? _cmdBuf;
    private readonly Matrix4x4 _projMat = Matrix4x4.Perspective(60f, 1, 0.3f, 15f);

    // Apparently this is more performant than:
    //     var camTransform = transform;
    //     var pos = camTransform.position;
    //     var lookMat = Matrix4x4.LookAt(pos, pos + camTransform.forward, camTransform.up);
    //     var scaleMat = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(1, 1, -1));
    //     var viewMat = scaleMat * lookMat.inverse;
    private Matrix4x4 GetViewMatrix()
    {
        var camTransform = transform;
        var viewMat = Matrix4x4.Rotate(Quaternion.Inverse(camTransform.rotation)) *
                      Matrix4x4.Translate(-camTransform.position);

        if (SystemInfo.usesReversedZBuffer)
        {
            viewMat.m20 = -viewMat.m20;
            viewMat.m21 = -viewMat.m21;
            viewMat.m22 = -viewMat.m22;
            viewMat.m23 = -viewMat.m23;
        }

        return viewMat;
    }

    private void Start()
    {
        _cmdBuf = new CommandBuffer();
    }

    private void LateUpdate()
    {
        _cmdBuf!.Clear();
        _cmdBuf.SetViewProjectionMatrices(GetViewMatrix(), _projMat);
        _cmdBuf.DrawRenderer(emoteSkinnedMeshRenderer, material);

        Graphics.SetRenderTarget(targetRenderTexture);
        GL.Clear(false, true, Color.green);
        Graphics.ExecuteCommandBuffer(_cmdBuf);
    }

    private void OnDestroy()
    {
        _cmdBuf?.Dispose();
    }
}