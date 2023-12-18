namespace LethalEmotesApi.Ui.Options;

public class RootMotionTypeDropdown : LeUiDropdown
{
    protected override int GetCurrentValue() => EmoteUiManager.RootMotionType;

    protected override void SetCurrentValue(int value) => EmoteUiManager.RootMotionType = value;
}