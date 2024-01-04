namespace LethalEmotesApi.Ui.Options;

public class ThirdPersonDropdown : LeUiDropdown
{
    protected override int GetCurrentValue() => EmoteUiManager.ThirdPerson;

    protected override void SetCurrentValue(int value) => EmoteUiManager.ThirdPerson = value;
}