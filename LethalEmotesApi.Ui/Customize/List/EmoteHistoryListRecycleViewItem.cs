using LethalEmotesApi.Ui.Elements.Recycle;
using LethalEmotesApi.Ui.EmoteHistory;
using UnityEngine;

namespace LethalEmotesApi.Ui.Customize.List;

public class EmoteHistoryListRecycleViewItem : MonoBehaviour, IRecycleViewItem<RecentEmote>
{
    public EmoteHistoryListItem? emoteListItem;
    
    public int ConstraintIndex { get; set; }

    private RectTransform? _rectTransform;
    public RectTransform RectTransform => _rectTransform!;

    private void Awake()
    {
        if (_rectTransform is null)
            _rectTransform = GetComponent<RectTransform>();
    }

    public void BindData(RecentEmote data)
    {
        if (emoteListItem is null)
            return;
        
        emoteListItem.SetEmoteKey(data.EmoteKey);
        emoteListItem.SetPlayers(data.PlayerNames);
    }
}