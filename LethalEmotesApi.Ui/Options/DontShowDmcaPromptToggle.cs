namespace LethalEmotesApi.Ui.Options;

public class DontShowDmcaPromptToggle : LeUiToggle
{
    protected override bool GetCurrentValue() => EmoteUiManager.DontShowDmcaPrompt;

    protected override void SetCurrentValue(bool value) => EmoteUiManager.DontShowDmcaPrompt = value;
}