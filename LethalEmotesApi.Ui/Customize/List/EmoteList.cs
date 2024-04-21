using System.Collections.Generic;
using System.Linq;
using LethalEmotesApi.Ui.Data;
using LethalEmotesApi.Ui.Elements.Recycle;
using TMPro;
using UnityEngine;

namespace LethalEmotesApi.Ui.Customize.List;

[DisallowMultipleComponent]
public class EmoteList : RecycleListView<EmoteListRecycleViewItem, string>
{
    public TMP_InputField? searchInputField;
    
    private CustomizePanel? _customizePanel;
    private SearchableEmoteArray? _searchableEmoteArray;

    protected override IList<string> ListData => _searchableEmoteArray!.ToArray();

    protected override void Awake()
    {
        base.Awake();

        if (searchInputField is null)
            return;
        
        searchInputField.onValueChanged.AddListener(SearchFieldUpdated);
    }

    protected override void Start()
    {
        _searchableEmoteArray = new SearchableEmoteArray(EmoteUiManager.EmoteKeys.ToArray());
    
        if (_customizePanel is null)
            _customizePanel = GetComponentInParent<CustomizePanel>();
        
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
        
        if (searchInputField is not null && searchInputField.isFocused)
            EmoteKeybindButton.CancelExistingRebind();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        
        _searchableEmoteArray = new SearchableEmoteArray(EmoteUiManager.EmoteKeys.ToArray());

        if (_customizePanel is null)
            _customizePanel = GetComponentInParent<CustomizePanel>();

        if (scrollRect is not null)
            scrollRect.normalizedPosition = Vector2.one;
        
        UpdateState();
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        if (searchInputField is null)
            return;
        
        searchInputField.SetTextWithoutNotify("");

        if (_searchableEmoteArray is null)
            return;

        _searchableEmoteArray.Filter = "";
    }
    
    protected override void OnInstantiateListItem(EmoteListRecycleViewItem viewItem)
    {
        var instance = viewItem.emoteListItem;
        if (instance is null)
            return;
        
        instance.dragDropController = _customizePanel!.dragDropController;
        instance.previewController = _customizePanel!.previewController;
    }
    
    private void SearchFieldUpdated(string filter)
    {
        if (_searchableEmoteArray is null)
            return;

        _searchableEmoteArray.Filter = filter;
        
        UpdateState();
    }
}