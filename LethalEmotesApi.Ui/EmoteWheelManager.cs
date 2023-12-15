using LethalEmotesApi.Ui.Data;

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

    internal static void EmoteSelected(string emote)
    {
        OnEmoteSelected?.Invoke(emote);
    }

    internal static EmoteWheelsController? WheelControllerInstance;

    public static void OpenEmoteWheel()
    {
        
    }

    public static void CloseEmoteWheel()
    {
        
    }
}