namespace LethalEmotesApi.Ui.Options;

public class NearestEmoteTextTutorial : LeUiToggle
{
    protected override bool GetCurrentValue() => EmoteUiManager.NearestEmoteTutorial;

    protected override void SetCurrentValue(bool value) => EmoteUiManager.NearestEmoteTutorial = value;
}