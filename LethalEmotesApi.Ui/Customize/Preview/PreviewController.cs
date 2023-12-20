using UnityEngine;
using UnityEngine.EventSystems;

namespace LethalEmotesApi.Ui.Customize.Preview;

public class PreviewController : UIBehaviour
{
    public GameObject? previewPrefab;

    private GameObject? _previewObjectInstance;
    private Animator? _previewAnimator;

    protected override void Start()
    {
        base.Start();
        
        ResetPreviewInstance();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        
        ResetPreviewInstance();
    }

    protected override void OnDisable()
    {
        base.OnDisable();

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
        
        EmoteUiManager.PlayAnimationOn(_previewAnimator, emoteKey);
    }

    private void ResetPreviewInstance()
    {
        if (previewPrefab is null)
            return;

        DestroyPreviewInstance();

        _previewObjectInstance = Instantiate(previewPrefab, transform.position + new Vector3(0, -10000f, 0),
            Quaternion.Euler(0, 0, 0));
        _previewObjectInstance.SetActive(true);

        _previewAnimator = _previewObjectInstance.GetComponentInChildren<Animator>();
    }

    private void DestroyPreviewInstance()
    {
        if (_previewObjectInstance is null)
            return;
        
        _previewObjectInstance.SetActive(false);
        
        DestroyImmediate(_previewObjectInstance);
        _previewObjectInstance = null;
        _previewAnimator = null;
    }
}