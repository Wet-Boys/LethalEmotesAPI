using TMPro;

namespace LethalEmotesApi.Ui.Customize.List;

public class EmoteHistoryListItem : EmoteListItem
{
    public TextMeshProUGUI? mostRecentPlayerLabel;
    public TextMeshProUGUI? excessPlayersLabel;

    public void SetPlayers(string[] playerNames)
    {
        if (mostRecentPlayerLabel is null || excessPlayersLabel is null)
            return;

        if (playerNames.Length <= 0)
            return;

        mostRecentPlayerLabel.text = playerNames[0];

        if (playerNames.Length <= 1)
            return;

        excessPlayersLabel.text = $"+{playerNames.Length - 1}";
    }
}