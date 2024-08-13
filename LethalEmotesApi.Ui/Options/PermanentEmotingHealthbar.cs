namespace LethalEmotesApi.Ui.Options;

public class PermanentEmotingHealthbar : LeUiToggle
{
    protected override bool GetCurrentValue() => EmoteUiManager.PermanentEmotingHealthbar;

    protected override void SetCurrentValue(bool value) => EmoteUiManager.PermanentEmotingHealthbar = value;
}