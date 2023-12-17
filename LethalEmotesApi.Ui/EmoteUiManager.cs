using LethalEmotesApi.Ui.Data;

namespace LethalEmotesApi.Ui;

public static class EmoteUiManager
{
    private static IEmoteUiStateController? _stateController;
    internal static EmoteUiPanel? EmoteUiInstance;

    public static void RegisterStateController(IEmoteUiStateController stateController)
    {
        _stateController = stateController;
    }
}