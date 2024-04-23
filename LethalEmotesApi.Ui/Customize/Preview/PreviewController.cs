using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Experimental.Rendering;
using UnityEngine.UI;

namespace LethalEmotesApi.Ui.Customize.Preview;

[DisallowMultipleComponent]
public class PreviewController : UIBehaviour, IDragHandler, IScrollHandler
{
    private static int _instanceCount;
    
    public GameObject? previewPrefab;
    public float rotSpeed = 25.0f;
    public int renderResolution = 360;
    public RectTransform? renderRect;
    public GraphicsFormat renderColorFormat;
    public GraphicsFormat renderDepthFormat;
    public RawImage? previewGraphic;

    private GameObject? _previewObjectInstance;
    private Animator? _previewAnimator;
    private PreviewRig? _previewRig;
    private PreviewEmoteRenderer? _previewEmoteRenderer;
    private RenderTexture? _renderTexture;

    public PreviewEmoteRenderer PreviewEmoteRenderer => _previewEmoteRenderer!;

    private float Ratio => renderRect is not null ? renderRect.rect.width / renderRect.rect.height : 1f;

    protected override void Awake()
    {
        base.Awake();
        
        _renderTexture = new RenderTexture(renderResolution, (int)Mathf.Floor(renderResolution * Ratio), 1, RenderTextureFormat.DefaultHDR);
    }

    protected override void Start()
    {
        base.Start();
        
        ResetPreviewInstance();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        
        _instanceCount++;
        
        ResetPreviewInstance();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        
        _instanceCount--;
        if (_instanceCount < 0)
            _instanceCount = 0;

        DestroyPreviewInstance();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        DestroyPreviewInstance();
    }

    public void PlayEmote(string emoteKey)
    {
        if (_previewAnimator is null)
            return;
        try
        {
            EmoteUiManager.PlayAnimationOn(_previewAnimator, emoteKey);
        }
        catch
        {
            Debug.Log("Preview can not play emote because it doesn't exist");
        }
    }

    public void ResetPreviewControls()
    {
        if (_previewRig is null || _previewEmoteRenderer is null)
            return;

        _previewRig.ResetControls();
        _previewEmoteRenderer.Fov = 25f;
    }

    private void ResetPreviewInstance()
    {
        if (previewPrefab is null || previewGraphic is null || _renderTexture is null)
            return;

        DestroyPreviewInstance();

        _renderTexture.Create();

        const float perDim = 5;

        var x = -10000f * (int)(_instanceCount % perDim);
        var y = -10000f * (int)((_instanceCount - Mathf.Floor(_instanceCount / perDim) * perDim) % perDim);
        var z = -10000f * (int)((_instanceCount - Mathf.Floor(_instanceCount / (perDim * perDim)) * (perDim * perDim)) % (perDim * perDim));

        _previewObjectInstance = Instantiate(previewPrefab, transform.position + new Vector3(x, y, z),
            Quaternion.Euler(0, 0, 0));
        _previewObjectInstance.SetActive(true);

        _previewAnimator = _previewObjectInstance.GetComponentInChildren<Animator>();
        _previewRig = _previewObjectInstance.GetComponentInChildren<PreviewRig>();
        _previewEmoteRenderer = _previewRig.emoteRenderer!;
        _previewEmoteRenderer.AspectRatio = Ratio;
        _previewEmoteRenderer.targetRenderTexture = _renderTexture;

        previewGraphic.texture = _renderTexture;
    }

    private void DestroyPreviewInstance()
    {
        if (_previewObjectInstance is null)
            return;
        
        _previewObjectInstance.SetActive(false);
        
        DestroyImmediate(_previewObjectInstance);
        _previewObjectInstance = null;
        _previewAnimator = null;
        _previewRig = null;
        _previewEmoteRenderer = null;

        if (_renderTexture is null)
            return;
        
        if (_renderTexture.IsCreated())
            _renderTexture.Release();
    }

    public void OnDrag(PointerEventData data)
    {
        float vInput = data.delta.y * Time.deltaTime * rotSpeed;
        float hInput = data.delta.x * Time.deltaTime * rotSpeed;
        
        _previewRig?.Orbit(vInput, hInput);
    }
    
    public void OnScroll(PointerEventData eventData)
    {
        float dir = eventData.scrollDelta.y * 4;
        _previewRig?.Zoom(dir);
    }
}