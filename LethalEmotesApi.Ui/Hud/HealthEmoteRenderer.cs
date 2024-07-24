using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace LethalEmotesApi.Ui.Hud;

[DefaultExecutionOrder(-4)]
public class HealthEmoteRenderer : MonoBehaviour
{
    public SkinnedMeshRenderer? emoteSkinnedMeshRenderer;
    public Material? material;
    public RenderTexture? hudRenderTexture;

    public float bakeFuzz = 0.005f;

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
        var min = (emoteSkinnedMeshRenderer!.localBounds.extents.y * 2f) - 0.65f;
        material!.SetFloat("_RemapMin", Mathf.Max(min, 1.7f));

        _cmdBuf!.Clear();
        _cmdBuf.SetViewProjectionMatrices(GetViewMatrix(), _projMat);
        _cmdBuf.DrawRenderer(emoteSkinnedMeshRenderer, material);

        Graphics.SetRenderTarget(hudRenderTexture);
        GL.Clear(false, true, Color.green);
        Graphics.ExecuteCommandBuffer(_cmdBuf);
    }

    private void OnDestroy()
    {
        _cmdBuf?.Dispose();
    }
}