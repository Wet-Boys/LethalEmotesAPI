using System.Linq;
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
        {
            mostRecentPlayerLabel.text = "";
            return;
        }

        mostRecentPlayerLabel.text = playerNames.Last();

        if (playerNames.Length <= 1)
        {
            excessPlayersLabel.text = "";
            return;
        }

        excessPlayersLabel.text = $"+{playerNames.Length - 1}";
    }
}