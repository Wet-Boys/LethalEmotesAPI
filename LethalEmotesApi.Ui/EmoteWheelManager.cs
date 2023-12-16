using LethalEmotesApi.Ui.Data;
using UnityEngine;

namespace LethalEmotesApi.Ui;

public static class EmoteWheelManager
{
    internal const string EmoteNone = "none";
    
    public delegate void OnEmoteSelectedDelegate(string emote);
    public static event OnEmoteSelectedDelegate? OnEmoteSelected;

    public delegate EmoteWheelSetData GetEmoteWheelSetDataDelegate();
    public static GetEmoteWheelSetDataDelegate? GetEmoteWheelSetData;

    public delegate void SetEmoteWheelSetDataDelegate(EmoteWheelSetData wheelSetData);
    public static SetEmoteWheelSetDataDelegate? SetEmoteWheelSetData;

    public static EmoteInteractionHandler? InteractionHandler;

    internal static void EmoteSelected(string emote)
    {
        OnEmoteSelected?.Invoke(emote);
    }

    internal static EmoteWheelsController? WheelControllerInstance;

    private static bool _emotesOpen;

    public static void OpenEmoteWheel()
    {
        if (WheelControllerInstance is null || InteractionHandler is null)
            return;

        if (InteractionHandler.InOtherMenu() || _emotesOpen)
            return;
        
        WheelControllerInstance.Show();
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        
        InteractionHandler.SetMenuLocked(true);
        _emotesOpen = true;
    }

    public static void CloseEmoteWheel()
    {
        if (WheelControllerInstance is null || InteractionHandler is null)
            return;
        
        if (InteractionHandler.InOtherMenu() && !_emotesOpen)
            return;
        
        WheelControllerInstance.Hide();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        InteractionHandler.SetMenuLocked(false);
        _emotesOpen = false;
    }

    public static void WheelLeft()
    {
        if (WheelControllerInstance is null || !_emotesOpen)
            return;
        
        WheelControllerInstance.NextWheel();
    }

    public static void WheelRight()
    {
        if (WheelControllerInstance is null || !_emotesOpen)
            return;
        
        WheelControllerInstance.PrevWheel();
    }
}