using LethalEmotesApi.Ui.Customize;
using UnityEngine;

namespace LethalEmotesApi.Ui.Options;

public class UseGlobalSettings : LeUiToggle
{
    protected override bool GetCurrentValue() => EmoteUiManager.UseGlobalSettings;

    protected override void SetCurrentValue(bool value)
    {
        EmoteUiManager.UseGlobalSettings = !value;
        EmoteUiManager.UseGlobalSettings = value;
        EmoteUiManager.RefreshBothLists();
        EmoteUiManager.emoteUiInstance!.ReloadData();
        GetComponentInParent<CustomizePanel>().gameObject.BroadcastMessage("UpdateStateBroadcast");
    }
}