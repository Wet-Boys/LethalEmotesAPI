namespace LethalEmotesApi.Ui.Options;

public class EmotesAlertEnemies : LeUiToggle
{
    protected override bool GetCurrentValue() => EmoteUiManager.EmotesAlertEnemies;

    protected override void SetCurrentValue(bool value) => EmoteUiManager.EmotesAlertEnemies = value;
}