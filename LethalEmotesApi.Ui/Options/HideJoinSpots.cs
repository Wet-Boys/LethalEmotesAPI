using System;

namespace LethalEmotesApi.Ui.Options;

public class HideJoinSpots : LeUiToggle
{
    protected override bool GetCurrentValue() => EmoteUiManager.HideJoinSpots;

    protected override void SetCurrentValue(bool value) => EmoteUiManager.HideJoinSpots = value;
}