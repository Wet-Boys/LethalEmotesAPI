using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LethalEmotesApi.Ui.Customize.List;

[DisallowMultipleComponent]
public class EmoteVisibilityToggle : UIBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public EmoteListItem? emoteListItem;
    public Image? visibilityImage;
    public Sprite? enabledSprite;
    public Sprite? disabledSprite;

    private string? _emoteKey;
    private bool IsVisible => !EmoteUiManager.EmotePoolBlacklist.Contains(_emoteKey);

    protected override void Start()
    {
        base.Start();
        UpdateState();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        UpdateState();
    }

    public void SetEmoteKey(string emoteKey)
    {
        _emoteKey = emoteKey;
        UpdateState();
    }

    public void Toggle()
    {
        if (string.IsNullOrEmpty(_emoteKey))
            return;

        if (IsVisible)
        {
            EmoteUiManager.AddToEmoteBlacklist(_emoteKey);
        }
        else
        {
            EmoteUiManager.RemoveFromEmoteBlacklist(_emoteKey);
        }
        
        UpdateState();
    }

    public void UpdateState()
    {
        if (string.IsNullOrEmpty(_emoteKey))
            return;

        if (visibilityImage is null || enabledSprite is null || disabledSprite is null)
            return;

        visibilityImage.sprite = IsVisible ? enabledSprite : disabledSprite;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        emoteListItem!.OnPointerExit(eventData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        emoteListItem!.OnPointerEnter(eventData);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;
        
        Toggle();
    }
}