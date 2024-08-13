namespace LethalEmotesApi.Ui.Options;

public class NearestEmoteText : LeUiToggle
{
    protected override bool GetCurrentValue() => EmoteUiManager.NearestEmoteText;

    protected override void SetCurrentValue(bool value) => EmoteUiManager.NearestEmoteText = value;
}