using System.Collections.Generic;
using System.Linq;
using LethalEmotesApi.Ui.Data;
using LethalEmotesApi.Ui.Elements.Recycle;
using TMPro;
using UnityEngine;

namespace LethalEmotesApi.Ui.Customize.List;

[DisallowMultipleComponent]
public class EmoteList : RecycleListView<EmoteListItem, string>
{
    public TMP_InputField? searchInputField;
    
    private CustomizePanel? _customizePanel;
    private SearchableEmoteArray? _searchableEmoteArray;

    private bool _firstUpdate;

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

    private void Update()
    {
        if (!_firstUpdate)
        {
            UpdateState();
            _firstUpdate = true;
        }
        
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
        
        searchInputField.text = "";

        if (_searchableEmoteArray is null)
            return;

        _searchableEmoteArray.Filter = "";
    }
    
    protected override void OnInstantiateListItem(EmoteListItem instance)
    {
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