using System.Collections.Generic;

namespace LethalEmotesApi.Ui.EmoteHistory;

public class RecentEmote
{
    public float Distance; //idk if we actually need distance...
    public readonly List<string> PlayerNames;
    public string EmoteKey;
    
    public RecentEmote(float distance, string playerName, string emoteKey)
    {
        Distance = distance;
        PlayerNames = [playerName];
        EmoteKey = emoteKey;
    }
    
    public RecentEmote(float distance, List<string> playerName, string emoteKey)
    {
        Distance = distance;
        PlayerNames = playerName;
        EmoteKey = emoteKey;
    }
    
    public bool AddPlayer(string playerName)
    {
        if (PlayerNames.Contains(playerName))
            return false;
            
        PlayerNames.Add(playerName);
        return true;
    }
}