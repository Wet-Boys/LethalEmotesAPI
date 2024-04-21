using System.Collections.Generic;

namespace LethalEmotesApi.Ui.EmoteHistory;

public class RecentEmote
{
    public float Distance; // idk if we actually need distance...
    public readonly string EmoteKey;
    public readonly List<string> PlayerNames;
    
    public RecentEmote(float distance, string emoteKey, string playerName)
    {
        Distance = distance;
        EmoteKey = emoteKey;
        PlayerNames = [playerName];
    }
    
    public RecentEmote(float distance, string emoteKey, List<string> playerNames)
    {
        Distance = distance;
        EmoteKey = emoteKey;
        PlayerNames = playerNames;
    }
    
    public bool AddPlayer(string playerName)
    {
        if (PlayerNames.Contains(playerName))
            return false;
            
        PlayerNames.Add(playerName);
        return true;
    }
}