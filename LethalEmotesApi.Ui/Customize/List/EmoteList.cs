using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace LethalEmotesApi.Ui.Customize.List;

[DisallowMultipleComponent]
public class EmoteList : UIBehaviour
{
    public RectTransform? listContentContainer;
    public GameObject? entryPrefab;

    private readonly List<GameObject> _listObjects = [];

    protected override void Start()
    {
        base.Start();
        InitList();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        InitList();
    }

    private void InitList()
    {
        if (listContentContainer is null || entryPrefab is null)
            return;

        if (_listObjects.Count > 0)
            return;
        
        var emoteKeys = EmoteUiManager.EmoteKeys;

        foreach (var emote in emoteKeys)
        {
            var entryGameObject = Instantiate(entryPrefab, listContentContainer);
            var entry = entryGameObject.GetComponent<EmoteListItem>();
            
            entry.SetEmoteKey(emote);
            
            _listObjects.Add(entryGameObject);
        }
    }
}