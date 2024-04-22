using TMPro;
using UnityEngine;

namespace LethalEmotesApi.Ui.Customize.List;

public class EmoteHistoryListItem : EmoteListItem
{
    public TextMeshProUGUI? mostRecentPlayerLabel;

    private string[] _playerNames = [];
    
    public void SetPlayers(string[] playerNames)
    {
        _playerNames = playerNames;
        
        Canvas.ForceUpdateCanvases();
        
        UpdateState();
    }

    private void UpdateState()
    {
        if (mostRecentPlayerLabel is null)
            return;
        
        if (_playerNames.Length <= 0)
        {
            mostRecentPlayerLabel.text = "";
            return;
        }
        
        var labelRect = mostRecentPlayerLabel.rectTransform.rect;
        var remainingWidth = labelRect.width;
        var labelText = "";
        var excessText = "";
        var fitAllNames = false;

        for (int i = 0; i < _playerNames.Length; i++)
        {
            var playerText = _playerNames[i];
            var nameWidth = TextWidthApproximation(playerText);
            
            if (i + 1 < _playerNames.Length)
            {
                excessText = $" +{_playerNames.Length - (i + 1)}";
                playerText += excessText;
                nameWidth += TextWidthApproximation(excessText);
                fitAllNames = false;
            }
            else
            {
                fitAllNames = true;
            }

            if (nameWidth >= remainingWidth)
            {
                fitAllNames = false;
                break;
            }

            playerText = playerText[..^excessText.Length];

            labelText += $"{playerText}, ";
            remainingWidth -= nameWidth;
        }
        
        if (fitAllNames)
            labelText = labelText[..^2];

        mostRecentPlayerLabel.text = labelText;
    }
    
    // Below method is thanks to: https://forum.unity.com/threads/calculate-width-of-a-text-before-without-assigning-it-to-a-tmp-object.758867/#post-5057900
    private float TextWidthApproximation(string text)
    {
        if (mostRecentPlayerLabel is null)
            return 0;
        
        var fontSize = mostRecentPlayerLabel.fontSize;
        var fontAsset = mostRecentPlayerLabel.font;
        var style = mostRecentPlayerLabel.fontStyle;
        
        // Compute scale of the target point size relative to the sampling point size of the font asset.
        var pointSizeScale = fontSize / (fontAsset.faceInfo.pointSize * fontAsset.faceInfo.scale);
        var emScale = fontSize * 0.01f;

        var styleSpacingAdjustment = (style & FontStyles.Bold) == FontStyles.Bold ? fontAsset.boldSpacing : 0;
        var normalSpacingAdjustment = fontAsset.normalSpacingOffset;

        float width = 0;

        foreach (var unicode in text)
        {
            // Make sure the given unicode exists in the font asset.
            if (fontAsset.characterLookupTable.TryGetValue(unicode, out var character))
                width += character.glyph.metrics.horizontalAdvance * pointSizeScale + (styleSpacingAdjustment + normalSpacingAdjustment) * emScale;
        }

        return width;
    }
}