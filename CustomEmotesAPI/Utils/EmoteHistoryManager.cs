using System;
using System.Collections.Generic;
using System.Linq;
using EmotesAPI;
using LethalEmotesApi.Ui.EmoteHistory;
using UnityEngine;

namespace LethalEmotesAPI.Utils;

public class EmoteHistoryManager : IEmoteHistoryManager
{
    private const float EmoteDistance = 30;
    private const int MaxEmoteHistory = 50;
    
    /// LUT of most recent emotes by emote key, no duplicates.
    private readonly Dictionary<string, RecentEmote> _emoteKeyToRecentEmotes = new();
    
    // Store emotes with a timestamp.
    private readonly Dictionary<RecentEmote, long> _emoteHistory = new();
    
    // Keep track of last played emote per player, so we may determine if we need to create a new RecentEmote or not.
    private readonly Dictionary<string, string> _playerLastPlayedEmote = new();

    private readonly Dictionary<string, List<BoneMapper>> _activeEmotes = new();
    
    /// <summary>
    /// Keep a reversed array of RecentEmotes cached for quicker fetches.
    /// </summary>
    private RecentEmote[] _cachedRecentEmoteHistory = [];
    private bool _staleCache;
    
    public void PlayerPerformedEmote(string emoteKey, BoneMapper mapper) // gets fired whenever a BoneMapper calls an emote
    {
        // Make sure the emote key that was passed in is valid.
        if (!LethalEmotesUiState.Instance.EmoteDb.GetEmoteVisibility(emoteKey))
            return;
        
        AddActiveEmote(emoteKey, mapper);
        
        if (!InRange(mapper))
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
            recentEmote.AddPlayer(mapper.userName);
            
            _emoteKeyToRecentEmotes.Add(emoteKey, recentEmote); // increase player count and re-add item to back of list (just read it backwards)
            _emoteHistory[recentEmote] = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        }
        else
        {
            var newRecentEmoteEntry = new RecentEmote(emoteKey, mapper.userName);
            
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
        
        _playerLastPlayedEmote[mapper.userName] = emoteKey;
        _staleCache = true;
        
        PlayerEmoted(_emoteKeyToRecentEmotes.Last().Value);
    }

    public void PlayerStoppedEmote(string playerName)
    {
        if (!_playerLastPlayedEmote.TryGetValue(playerName, out var emoteKey))
            return;

        if (!_activeEmotes.TryGetValue(emoteKey, out var mappers))
            return;

        var mapperIndex = mappers.FindIndex(mapper => mapper.userName == playerName);
        if (mapperIndex < 0)
            return;

        RemoveActiveEmote(emoteKey, mappers[mapperIndex]);
    }

    private void AddActiveEmote(string emoteKey, BoneMapper mapper)
    {
        if (!_activeEmotes.TryGetValue(emoteKey, out var mappers))
            mappers = [];
        
        mappers.Add(mapper);
        _activeEmotes[emoteKey] = mappers;

        if (_playerLastPlayedEmote.TryGetValue(mapper.userName, out var lastEmoteKey))
            RemoveActiveEmote(lastEmoteKey, mapper);
    }

    private void RemoveActiveEmote(string emoteKey, BoneMapper mapper)
    {
        if (!_activeEmotes.TryGetValue(emoteKey, out var mappers))
            return;

        mappers.Remove(mapper);
        _activeEmotes[emoteKey] = mappers;
    }

    public RecentEmote GetRecentEmote(string emoteKey)// idk if this is needed tbh
    {
        return _emoteKeyToRecentEmotes[emoteKey];
    }
    
    public RecentEmote[] GetCurrentlyPlayingEmotes()
    {
        var emotes = new List<RecentEmote>();
        foreach (var (emoteKey, mappers) in _activeEmotes)
        {
            var validMappers = mappers.Where(InRange)
                .Select(mapper => mapper.userName)
                .ToArray();

            if (validMappers.Length <= 0)
                continue;
            
            var emote = new RecentEmote(emoteKey, validMappers);
            emotes.Add(emote);
        }
        
        return emotes.ToArray();
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
    
    private static bool InRange(BoneMapper mapper)
    {
        var localMapper = CustomEmotesAPI.localMapper;
        var localMapperPos = localMapper.transform.position;
        var mapperPos = mapper.transform.position;

        return Vector3.Distance(localMapperPos, mapperPos) < EmoteDistance;
    }
}