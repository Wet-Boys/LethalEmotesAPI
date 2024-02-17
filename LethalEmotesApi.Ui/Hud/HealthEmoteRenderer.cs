using UnityEngine;
using UnityEngine.Rendering;

namespace LethalEmotesApi.Ui.Hud;

[DefaultExecutionOrder(-4)]
public class HealthEmoteRenderer : MonoBehaviour
{
    public SkinnedMeshRenderer? emoteSkinnedMeshRenderer;
    public Material? material;
    public RenderTexture? targetRenderTexture;
    
    private void LateUpdate()
    {
        if (emoteSkinnedMeshRenderer is null || material is null || targetRenderTexture is null)
            return;
        
        var projMat = Matrix4x4.Perspective(60f, 1, 0.3f, 15f);

        var camTransform = transform;
        var pos = camTransform.position;

        var lookMat = Matrix4x4.LookAt(pos, pos + camTransform.forward, camTransform.up);
        var scaleMat = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(1, 1, -1));

        var viewMat = scaleMat * lookMat.inverse;
        
        var buf = new CommandBuffer();
        
        buf.Clear();
        
        buf.SetViewProjectionMatrices(viewMat, projMat);
        
        buf.DrawRenderer(emoteSkinnedMeshRenderer, material);

        var prevRenderTexture = RenderTexture.active;
        
        Graphics.SetRenderTarget(targetRenderTexture);
        
        GL.Clear(false, true, Color.green);
        
        Graphics.ExecuteCommandBuffer(buf);

        Graphics.SetRenderTarget(prevRenderTexture);
    }
}