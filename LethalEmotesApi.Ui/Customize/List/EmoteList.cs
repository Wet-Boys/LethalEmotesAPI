using System.Collections.Generic;
using System.Linq;
using LethalEmotesApi.Ui.Data;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace LethalEmotesApi.Ui.Customize.List;

[DisallowMultipleComponent]
public class EmoteList : UIBehaviour
{
    public RectTransform? listContentContainer;
    public TMP_InputField? searchInputField;
    public GameObject? entryPrefab;
    
    private CustomizePanel? _customizePanel;
    
    private readonly List<GameObject> _listObjects = [];
    private SearchableEmoteArray? _searchableEmoteArray;

    protected override void Awake()
    {
        base.Awake();

        if (searchInputField is null)
            return;
        
        searchInputField.onValueChanged.AddListener(SearchFieldUpdated);
    }

    protected override void Start()
    {
        base.Start();
        
        _searchableEmoteArray = new SearchableEmoteArray(EmoteUiManager.EmoteKeys.ToArray());

        if (_customizePanel is null)
            _customizePanel = GetComponentInParent<CustomizePanel>();
        
        InitList();
    }

    private void Update()
    {
        if (searchInputField is not null && searchInputField.isFocused)
            EmoteKeybindButton.CancelExistingRebind();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        
        _searchableEmoteArray = new SearchableEmoteArray(EmoteUiManager.EmoteKeys.ToArray());

        if (_customizePanel is null)
            _customizePanel = GetComponentInParent<CustomizePanel>();
        
        InitList();
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

    private void InitList()
    {
        if (listContentContainer is null || entryPrefab is null)
            return;

        if (_listObjects.Count > 0)
            return;
        
        var emoteKeys = _searchableEmoteArray!;

        foreach (var emote in emoteKeys)
        {
            var entryGameObject = Instantiate(entryPrefab, listContentContainer);
            var entry = entryGameObject.GetComponent<EmoteListItem>();
            
            entry.dragDropController = _customizePanel!.dragDropController;
            entry.previewController = _customizePanel!.previewController;
            entry.SetEmoteKey(emote);
            
            _listObjects.Add(entryGameObject);
        }
    }

    private void ClearList()
    {
        foreach (var listObject in _listObjects)
            DestroyImmediate(listObject);
        
        _listObjects.Clear();
    }
    
    private void SearchFieldUpdated(string filter)
    {
        if (_searchableEmoteArray is null)
            return;

        _searchableEmoteArray.Filter = filter;
        
        ClearList();
        InitList();
    }
}