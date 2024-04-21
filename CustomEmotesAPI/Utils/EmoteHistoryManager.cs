using System.Collections.Generic;
using System.Linq;
using EmotesAPI;
using LethalEmotesApi.Ui.EmoteHistory;
using UnityEngine;

namespace LethalEmotesAPI.Utils;

public class EmoteHistoryManager : IEmoteHistoryManager
{
    public const float RecentEmoteDistance = 30;
    
    private readonly Dictionary<string, RecentEmote> _emoteKeysToNearbyEmote = new();
    
    public void PlayerPerformedEmote(float dist, string emoteKey, string playerName) //gets fired whenever a BoneMapper calls an emote
    {
        if (_emoteKeysToNearbyEmote.ContainsKey(emoteKey))
        {
            _emoteKeysToNearbyEmote[emoteKey].AddPlayer(playerName);
            RecentEmote recent = _emoteKeysToNearbyEmote[emoteKey];
            _emoteKeysToNearbyEmote.Remove(emoteKey);
            _emoteKeysToNearbyEmote.Add(emoteKey, recent); //increase player count and re-add item to back of list (just read it backwards)
        }
        else
        {
            _emoteKeysToNearbyEmote.Add(emoteKey, new RecentEmote(dist, playerName, emoteKey));
        }
        while (_emoteKeysToNearbyEmote.Count > 50)
        {
            _emoteKeysToNearbyEmote.Remove(_emoteKeysToNearbyEmote.First().Key);
        }
        PlayerEmoted(_emoteKeysToNearbyEmote.Last().Value);
    }
    
    public RecentEmote GetRecentEmote(string emoteKey)//idk if this is needed tbh
    {
        return _emoteKeysToNearbyEmote[emoteKey];
    }
    
    public RecentEmote[] GetCurrentlyPlayingEmotes()
    {
        List<List<string>> playerList = new List<List<string>>();
        List<string> emoteList = new List<string>();
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
                    playerList.Add(new List<string>([item.userName]));
                }
            }
        }
        List<RecentEmote> list = new List<RecentEmote>();
        for (int i = 0; i < emoteList.Count; i++)
        {
            list.Add(new RecentEmote(0, playerList[i], emoteList[i]));
        }
        return list.ToArray();
    }
    public RecentEmote[] GetRecentEmotes()//so my thought is this can be used for the tab you are making in the customize menu.
        //But also, for the emote wheel section, call this once when opening the wheel and then use event below to add new items to the bottom of the list as players emote with the wheel already open.
        //You could almost just sort the emote wheel section oldest to newest to fix that but I don't think we have space for all that...
    {
        return _emoteKeysToNearbyEmote.Values.ToArray();
    }
        
    public delegate void PlayerPerformedEmoteDelegate(RecentEmote recentEmote);
        
    public static event PlayerPerformedEmoteDelegate PlayerPerformedEmoteEvent;
        
    private static void PlayerEmoted(RecentEmote recentEmote)
    {
        PlayerPerformedEmoteEvent?.Invoke(recentEmote);
    }
}