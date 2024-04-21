using System.Collections.Generic;
using LethalEmotesApi.Ui.Elements.Recycle;
using LethalEmotesApi.Ui.EmoteHistory;
using TMPro;
using UnityEngine;

namespace LethalEmotesApi.Ui.Customize.List;

[DisallowMultipleComponent]
public class EmoteHistoryList : RecycleListView<EmoteHistoryListRecycleViewItem, RecentEmote>
{
    public TextMeshProUGUI? emptyHistoryLabel;
    
    private CustomizePanel? _customizePanel;
    
    protected override IList<RecentEmote> ListData => EmoteUiManager.EmoteHistoryManager.GetHistory();

    protected override void OnEnable()
    {
        base.OnEnable();
        
        if (_customizePanel is null)
            _customizePanel = GetComponentInParent<CustomizePanel>();
        
        if (scrollRect is not null)
            scrollRect.normalizedPosition = Vector2.one;

        if (emptyHistoryLabel is null)
            return;
        
        emptyHistoryLabel.gameObject.SetActive(ListData.Count <= 0);
        
        UpdateState();
    }

    protected override void OnInstantiateListItem(EmoteHistoryListRecycleViewItem viewItem)
    {
        var instance = viewItem.emoteListItem;
        if (instance is null)
            return;
        
        instance.dragDropController = _customizePanel!.dragDropController;
        instance.previewController = _customizePanel!.previewController;
    }
}