namespace LethalEmotesApi.Ui.EmoteHistory;

public interface IEmoteHistoryManager
{
    public void PlayerPerformedEmote(float dist, string emoteKey, string playerName);
    
    public RecentEmote GetRecentEmote(string emoteKey);
    
    public RecentEmote[] GetCurrentlyPlayingEmotes();

    public RecentEmote[] GetRecentEmotes();
}