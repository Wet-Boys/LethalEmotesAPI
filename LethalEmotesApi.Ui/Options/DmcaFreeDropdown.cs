namespace LethalEmotesApi.Ui.Options;

public class DmcaFreeDropdown : LeUiDropdown
{
    protected override int GetCurrentValue() => EmoteUiManager.DmcaFree;

    protected override void SetCurrentValue(int value) => EmoteUiManager.DmcaFree = value;
}