namespace LethalEmotesApi.Ui.Options;

public class ImportBetterEmotes : LeUiToggle
{
    protected override bool GetCurrentValue() => EmoteUiManager.ImportBetterEmotes;

    protected override void SetCurrentValue(bool value) => EmoteUiManager.ImportBetterEmotes = value;
}