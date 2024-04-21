using System.Collections.Generic;
using System.Linq;
using EmotesAPI;
using LethalEmotesApi.Ui.EmoteHistory;
using UnityEngine;

namespace LethalEmotesAPI.Utils;

public class EmoteHistoryManager : IEmoteHistoryManager
{
    public const float RecentEmoteDistance = 30;
    public const int MaxEmoteHistory = 50;
    
    /// Used for History tracking of emotes, may contain duplicates.
    private readonly Dictionary<string, RecentEmote> _emoteKeyToHistoryOfRecentEmotes = new();
    
    // Keep track of last played emote per player, so we may determine if we need to create a new RecentEmote or not.
    private readonly Dictionary<string, string> _playerLastPlayedEmote = new();

    /// <summary>
    /// Keep a reversed array of RecentEmotes cached for quicker fetches.
    /// </summary>
    private RecentEmote[] _cachedRecentEmoteHistory = [];
    private bool _staleCache;
    
    public void PlayerPerformedEmote(float dist, string emoteKey, string playerName) // gets fired whenever a BoneMapper calls an emote
    {
        var shouldKeepGroup = false;
        if (_emoteKeyToHistoryOfRecentEmotes.TryGetValue(emoteKey, out var recentEmote))
        {
            // Check to make sure all players last played emote still match the emote in this grouping
            shouldKeepGroup = recentEmote.PlayerNames.All(player =>
            {
                if (_playerLastPlayedEmote.TryGetValue(player, out var lastEmoteKey))
                    return recentEmote.EmoteKey == lastEmoteKey;

                return false;
            });
        }
        
        if (shouldKeepGroup)
        {
            recentEmote.AddPlayer(playerName);

            _emoteKeyToHistoryOfRecentEmotes.Remove(emoteKey);
            _emoteKeyToHistoryOfRecentEmotes.Add(emoteKey, recentEmote); // increase player count and re-add item to back of list (just read it backwards)
        }
        else
        {
            _emoteKeyToHistoryOfRecentEmotes.Add(emoteKey, new RecentEmote(dist, emoteKey, playerName));
        }
        
        while (_emoteKeyToHistoryOfRecentEmotes.Count > MaxEmoteHistory)
            _emoteKeyToHistoryOfRecentEmotes.Remove(_emoteKeyToHistoryOfRecentEmotes.First().Key);
        
        _playerLastPlayedEmote[playerName] = emoteKey;
        _staleCache = true;
        
        PlayerEmoted(_emoteKeyToHistoryOfRecentEmotes.Last().Value);
    }
    
    public RecentEmote GetRecentEmote(string emoteKey)// idk if this is needed tbh
    {
        return _emoteKeyToHistoryOfRecentEmotes[emoteKey];
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
            list.Add(new RecentEmote(0, emoteList[i], playerList[i]));
        }
        
        return list.ToArray();
    }

    public RecentEmote[] GetHistory()
    {
        if (_staleCache)
        {
            _cachedRecentEmoteHistory = _emoteKeyToHistoryOfRecentEmotes.Values
                .Reverse()
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