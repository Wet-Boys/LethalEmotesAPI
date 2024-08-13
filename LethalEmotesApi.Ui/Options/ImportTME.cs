namespace LethalEmotesApi.Ui.Options;

public class ImportTME : LeUiToggle
{
    protected override bool GetCurrentValue() => EmoteUiManager.ImportTME;

    protected override void SetCurrentValue(bool value) => EmoteUiManager.ImportTME = value;
}