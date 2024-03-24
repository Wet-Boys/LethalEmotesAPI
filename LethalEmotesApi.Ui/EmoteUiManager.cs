using System;
using System.Collections.Generic;
using LethalEmotesApi.Ui.Data;
using LethalEmotesApi.Ui.Db;
using UnityEngine;

namespace LethalEmotesApi.Ui;

public static class EmoteUiManager
{
    internal static IEmoteUiStateController? _stateController;
    internal static EmoteUiPanel? EmoteUiInstance;
    
    public static void RegisterStateController(IEmoteUiStateController stateController)
    {
        _stateController = stateController;
    }

    public static IEmoteUiStateController? GetStateController() => _stateController;

    internal static void PlayEmote(string emoteKey)
    {
        try
        {
            _stateController?.PlayEmote(emoteKey);
        }
        catch
        {
            Debug.Log("Emote selected might not exist");
        }
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

    internal static IEmoteDb EmoteDb => _stateController!.EmoteDb;

    internal static IReadOnlyCollection<string> EmoteKeys => _stateController!.EmoteDb.EmoteKeys;

    internal static string GetEmoteName(string emoteKey) => _stateController!.EmoteDb.GetEmoteName(emoteKey);

    internal static string GetEmoteModName(string emoteKey) => _stateController!.EmoteDb.GetModName(emoteKey);
    
    internal static IReadOnlyCollection<string> RandomPoolBlacklist => _stateController!.RandomPoolBlacklist;
    
    internal static IReadOnlyCollection<string> EmotePoolBlacklist => _stateController!.EmotePoolBlacklist;

    internal static void AddToRandomPoolBlacklist(string emoteKey) => _stateController?.AddToRandomPoolBlacklist(emoteKey);

    internal static void RemoveFromRandomPoolBlacklist(string emoteKey) => _stateController?.RemoveFromRandomPoolBlacklist(emoteKey);
    
    internal static void AddToEmoteBlacklist(string emoteKey) => _stateController?.AddToEmoteBlacklist(emoteKey);

    internal static void RemoveFromEmoteBlacklist(string emoteKey) => _stateController?.RemoveFromEmoteBlacklist(emoteKey);

    internal static void RefreshBothLists() => _stateController?.RefreshBothLists();


    internal static EmoteWheelSetData LoadEmoteWheelSetData()
    {
        return _stateController!.LoadEmoteWheelSetData();
    }

    internal static void SaveEmoteWheelSetData(EmoteWheelSetData dataToSave)
    {
        _stateController!.SaveEmoteWheelSetData(dataToSave);

        if (EmoteUiInstance is null)
            return;
        
        EmoteUiInstance.ReloadData();
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

        if (!IsEmoteWheelsOpen())
            return;
        
        EmoteUiInstance.emoteWheelsController.PrevWheel();
    }

    public static void OnRightWheel()
    {
        if (EmoteUiInstance is null || EmoteUiInstance.emoteWheelsController is null)
            return;
        
        if (!IsEmoteWheelsOpen())
            return;
        
        EmoteUiInstance.emoteWheelsController.NextWheel();
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

    public static float EmoteVolume
    {
        get => _stateController!.EmoteVolume;
        set => _stateController!.EmoteVolume = value;
    }

    public static bool HideJoinSpots
    {
        get => _stateController!.HideJoinSpots;
        set => _stateController!.HideJoinSpots = value;
    }

    public static int RootMotionType
    {
        get => _stateController!.RootMotionType;
        set => _stateController!.RootMotionType = value;
    }

    public static bool EmotesAlertEnemies
    {
        get => _stateController!.EmotesAlertEnemies;
        set => _stateController!.EmotesAlertEnemies = value;
    }

    public static int DmcaFree
    {
        get => _stateController!.DmcaFree;
        set => _stateController!.DmcaFree = value;
    }

    public static int ThirdPerson
    {
        get => _stateController!.ThirdPerson;
        set => _stateController!.ThirdPerson = value;
    }
    public static bool UseGlobalSettings
    {
        get => _stateController!.UseGlobalSettings;
        set => _stateController!.UseGlobalSettings = value;
    }
}