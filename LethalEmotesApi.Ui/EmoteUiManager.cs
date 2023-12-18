using System.Collections.Generic;
using LethalEmotesApi.Ui.Data;
using UnityEngine;

namespace LethalEmotesApi.Ui;

public static class EmoteUiManager
{
    private static IEmoteUiStateController? _stateController;
    internal static EmoteUiPanel? EmoteUiInstance;
    
    public static void RegisterStateController(IEmoteUiStateController stateController)
    {
        _stateController = stateController;
    }

    internal static void PlayEmote(string emoteKey)
    {
        _stateController?.PlayEmote(emoteKey);
    }

    internal static void LockMouseInput()
    {
        _stateController?.LockMouseInput();
    }

    internal static void UnlockMouseInput()
    {
        _stateController?.UnlockMouseInput();
    }

    internal static void LockPlayerInput()
    {
        _stateController?.LockPlayerInput();
    }

    internal static void UnlockPlayerInput()
    {
        _stateController?.UnlockPlayerInput();
    }

    internal static void PlayAnimationOn(Animator animator, string emoteKey)
    {
        _stateController?.PlayAnimationOn(animator, emoteKey);
    }

    internal static IReadOnlyCollection<string> GetAllEmoteNames()
    {
        return _stateController!.GetAllEmoteNames();
    }

    internal static EmoteWheelSetData LoadEmoteWheelSetData()
    {
        return _stateController!.LoadEmoteWheelSetData();
    }

    internal static void SaveEmoteWheelSetData(EmoteWheelSetData dataToSave)
    {
        _stateController?.SaveEmoteWheelSetData(dataToSave);
    }

    public static bool IsEmoteWheelsOpen() => EmoteUiInstance is
        { IsOpen: true, CurrentView: EmoteUiPanel.UiView.EmoteWheels };

    public static bool IsCustomizePanelOpen() => EmoteUiInstance is
        { IsOpen: true, CurrentView: EmoteUiPanel.UiView.Customize };

    public static bool CanOpenEmoteWheels()
    {
        if (_stateController is null)
            return false;
        
        return _stateController.CanOpenEmoteUi() && !IsCustomizePanelOpen();
    }

    public static void OnLeftWheel()
    {
        if (EmoteUiInstance is null || EmoteUiInstance.emoteWheelsController is null)
            return;
        
        EmoteUiInstance.emoteWheelsController.NextWheel();
    }

    public static void OnRightWheel()
    {
        if (EmoteUiInstance is null || EmoteUiInstance.emoteWheelsController is null)
            return;
        
        EmoteUiInstance.emoteWheelsController.PrevWheel();
    }

    public static void OpenEmoteWheels()
    {
        if (EmoteUiInstance is null)
            return;

        if (!CanOpenEmoteWheels())
            return;
        
        EmoteUiInstance.Show();
    }

    public static void CloseEmoteWheels()
    {
        if (EmoteUiInstance is null)
            return;

        if (!IsEmoteWheelsOpen())
            return;
        
        EmoteUiInstance.Hide();
    }

    public static void CloseCustomizationPanel()
    {
        if (EmoteUiInstance is null)
            return;

        if (!IsCustomizePanelOpen())
            return;
        
        EmoteUiInstance.Hide();
    }

    public static void CloseUiGracefully()
    {
        CloseCustomizationPanel();

        if (EmoteUiInstance is null)
            return;

        if (!IsEmoteWheelsOpen())
            return;
        
        EmoteUiInstance.CloseGracefully();
    }
}