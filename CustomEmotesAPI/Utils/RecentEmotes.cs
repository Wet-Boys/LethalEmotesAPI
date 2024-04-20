using System.Collections.Generic;
using System.Linq;

namespace LethalEmotesAPI.Utils;

public class RecentEmote
{
    private static readonly Dictionary<string, RecentEmote> EmoteKeysToNearbyEmote = new();
    
    public float Distance; //idk if we actually need distance...
    public List<string> PlayerNames;
    public string EmoteKey;
        
    private RecentEmote(float distance, string playerName, string emoteKey)
    {
        Distance = distance;
        PlayerNames = [playerName];
        EmoteKey = emoteKey;
    }
        
    private bool AddPlayer(string playerName)
    {
        if (!PlayerNames.Contains(playerName))
        {
            PlayerNames.Add(playerName);
            return true;
        }
        return false;
    }
        
    internal static void PlayerPerformedEmote(float dist, string emoteKey, string playerName) //gets fired whenever a BoneMapper calls an emote
    {
        if (EmoteKeysToNearbyEmote.ContainsKey(emoteKey))
        {
            EmoteKeysToNearbyEmote[emoteKey].AddPlayer(playerName);
            RecentEmote recent = EmoteKeysToNearbyEmote[emoteKey];
            EmoteKeysToNearbyEmote.Remove(emoteKey);
            EmoteKeysToNearbyEmote.Add(emoteKey, recent); //increase player count and re-add item to back of list (just read it backwards)
        }
        else
        {
            EmoteKeysToNearbyEmote.Add(emoteKey, new RecentEmote(dist, playerName, emoteKey));
        }
        while (EmoteKeysToNearbyEmote.Count > 50)
        {
            EmoteKeysToNearbyEmote.Remove(EmoteKeysToNearbyEmote.First().Key);
        }
        PlayerEmoted(EmoteKeysToNearbyEmote.Last().Value);
    }
        
    public static RecentEmote GetRecentEmote(string emoteKey)//idk if this is needed tbh
    {
        return EmoteKeysToNearbyEmote[emoteKey];
    }
        
    public static RecentEmote[] GetRecentEmotes()//so my thought is this can be used for the tab you are making in the customize menu.
        //But also, for the emote wheel section, call this once when opening the wheel and then use event below to add new items to the bottom of the list as players emote with the wheel already open.
        //You could almost just sort the emote wheel section oldest to newest to fix that but I don't think we have space for all that...
    {
        return EmoteKeysToNearbyEmote.Values.ToArray();
    }
        
    public delegate void PlayerPerformedEmoteDelegate(RecentEmote recentEmote);
        
    public static event PlayerPerformedEmoteDelegate PlayerPerformedEmoteEvent;
        
    private static void PlayerEmoted(RecentEmote recentEmote)
    {
        PlayerPerformedEmoteEvent?.Invoke(recentEmote);
    }
}