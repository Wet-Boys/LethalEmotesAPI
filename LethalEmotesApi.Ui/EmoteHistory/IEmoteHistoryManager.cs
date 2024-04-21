namespace LethalEmotesApi.Ui.EmoteHistory;

public interface IEmoteHistoryManager
{
    public void PlayerPerformedEmote(float dist, string emoteKey, string playerName);
    
    public RecentEmote GetRecentEmote(string emoteKey);
    
    public RecentEmote[] GetCurrentlyPlayingEmotes();
    
    /// <summary>
    /// Get the History of recently played emotes
    /// <remarks>May contain stale duplicates</remarks>
    /// </summary>
    /// <returns>An array of <see cref="RecentEmote"/>'s ordered from most to least recent</returns>
    public RecentEmote[] GetHistory();
}