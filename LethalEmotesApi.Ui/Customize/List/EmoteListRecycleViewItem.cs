using LethalEmotesApi.Ui.Elements.Recycle;
using UnityEngine;

namespace LethalEmotesApi.Ui.Customize.List;

[RequireComponent(typeof(RectTransform))]
public class EmoteListRecycleViewItem : MonoBehaviour, IRecycleViewItem<string>
{
    public EmoteListItem? emoteListItem;
    
    public int ConstraintIndex { get; set; }

    private RectTransform? _rectTransform;

    public RectTransform RectTransform => _rectTransform!;

    private void Awake()
    {
        if (_rectTransform is null)
            _rectTransform = GetComponent<RectTransform>();
    }

    public void BindData(string emoteKey)
    {
        if (emoteListItem is null)
            return;
        
        emoteListItem.SetEmoteKey(emoteKey);
    }
}