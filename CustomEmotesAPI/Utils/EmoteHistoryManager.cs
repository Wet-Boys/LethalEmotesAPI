using System;
using System.Collections.Generic;
using System.Linq;
using EmotesAPI;
using LethalEmotesApi.Ui;
using LethalEmotesApi.Ui.EmoteHistory;
using UnityEngine;

namespace LethalEmotesAPI.Utils;

public class EmoteHistoryManager : IEmoteHistoryManager
{
    public const float RecentEmoteDistance = 30;
    public const int MaxEmoteHistory = 50;
    
    /// LUT of most recent emotes by emote key, no duplicates.
    private readonly Dictionary<string, RecentEmote> _emoteKeyToRecentEmotes = new();

    private readonly Dictionary<RecentEmote, long> _emoteHistory = new();
    
    // Keep track of last played emote per player, so we may determine if we need to create a new RecentEmote or not.
    private readonly Dictionary<string, string> _playerLastPlayedEmote = new();

    /// <summary>
    /// Keep a reversed array of RecentEmotes cached for quicker fetches.
    /// </summary>
    private RecentEmote[] _cachedRecentEmoteHistory = [];
    private bool _staleCache;
    
    public void PlayerPerformedEmote(float dist, string emoteKey, string playerName) // gets fired whenever a BoneMapper calls an emote
    {
        // Make sure the emote key that was passed in is valid.
        if (!LethalEmotesUiState.Instance.EmoteDb.GetEmoteVisibility(emoteKey))
            return;
        
        var shouldKeepGroup = false;
        if (_emoteKeyToRecentEmotes.TryGetValue(emoteKey, out var recentEmote))
        {
            // Check to make sure all players last played emote still match the emote in this grouping
            shouldKeepGroup = recentEmote.PlayerNames.All(player =>
            {
                if (_playerLastPlayedEmote.TryGetValue(player, out var lastEmoteKey))
                    return recentEmote.EmoteKey == lastEmoteKey;

                return false;
            });
        }
        
        _emoteKeyToRecentEmotes.Remove(emoteKey);
        if (recentEmote is not null)
            _emoteHistory.Remove(recentEmote);
        
        if (shouldKeepGroup)
        {
            recentEmote.AddPlayer(playerName);
            
            _emoteKeyToRecentEmotes.Add(emoteKey, recentEmote); // increase player count and re-add item to back of list (just read it backwards)
            _emoteHistory[recentEmote] = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        }
        else
        {
            var newRecentEmoteEntry = new RecentEmote(emoteKey, playerName);
            
            _emoteKeyToRecentEmotes.Add(emoteKey, newRecentEmoteEntry);
            _emoteHistory[newRecentEmoteEntry] = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        }
        
        while (_emoteKeyToRecentEmotes.Count > MaxEmoteHistory)
            _emoteKeyToRecentEmotes.Remove(_emoteKeyToRecentEmotes.First().Key);

        var emotesToObliterateFromTime = _emoteHistory
            .OrderByDescending(kvp => kvp.Value)
            .Skip(50)
            .Select(kvp => kvp.Key)
            .ToArray();

        foreach (var emote in emotesToObliterateFromTime)
            _emoteHistory.Remove(emote);
        
        _playerLastPlayedEmote[playerName] = emoteKey;
        _staleCache = true;
        
        PlayerEmoted(_emoteKeyToRecentEmotes.Last().Value);
    }
    
    public RecentEmote GetRecentEmote(string emoteKey)// idk if this is needed tbh
    {
        return _emoteKeyToRecentEmotes[emoteKey];
    }
    
    public RecentEmote[] GetCurrentlyPlayingEmotes()
    {
        List<List<string>> playerList = [];
        List<string> emoteList = [];
        foreach (var item in BoneMapper.allMappers)
        {
            if (!item.local && item.currentClip is not null && Vector3.Distance(item.transform.position, CustomEmotesAPI.localMapper.transform.position) <= RecentEmoteDistance)
            {
                if (emoteList.Contains(item.currentClip.customInternalName))
                {
                    playerList[emoteList.IndexOf(item.currentClip.customInternalName)].Add(item.userName);
                }
                else
                {
                    emoteList.Add(item.currentClip.customInternalName);
                    playerList.Add([item.userName]);
                }
            }
        }
        
        List<RecentEmote> list = [];
        for (int i = 0; i < emoteList.Count; i++)
        {
            list.Add(new RecentEmote(emoteList[i], playerList[i]));
        }
        
        return list.ToArray();
    }

    public RecentEmote[] GetHistory()
    {
        if (_staleCache)
        {
            _cachedRecentEmoteHistory = _emoteHistory
                .OrderByDescending(kvp => kvp.Value)
                .Select(kvp => kvp.Key)
                .ToArray();

            _staleCache = false;
        }

        return _cachedRecentEmoteHistory;
    }

    public delegate void PlayerPerformedEmoteDelegate(RecentEmote recentEmote);
        
    public static event PlayerPerformedEmoteDelegate PlayerPerformedEmoteEvent;
        
    private static void PlayerEmoted(RecentEmote recentEmote)
    {
        PlayerPerformedEmoteEvent?.Invoke(recentEmote);
    }
}