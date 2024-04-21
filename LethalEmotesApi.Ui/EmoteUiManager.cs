using System.Collections.Generic;
using System.Threading;
using LethalEmotesApi.Ui.Data;
using LethalEmotesApi.Ui.Db;
using LethalEmotesApi.Ui.EmoteHistory;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LethalEmotesApi.Ui;

public static class EmoteUiManager
{
    private static IEmoteUiStateController? _stateController;
    internal static EmoteUiPanel? emoteUiInstance;

    private static bool _hasShownDmcaPrompt;

    private static EmoteWheelSetDisplayData? _emoteDisplayData;
    
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

    internal static void EnableKeybinds()
    {
        _stateController?.EnableKeybinds();
    }
    
    internal static void DisableKeybinds()
    {
        _stateController?.DisableKeybinds();
    }

    internal static void PlayAnimationOn(Animator animator, string emoteKey)
    {
        _stateController?.PlayAnimationOn(animator, emoteKey);
    }

    internal static IEmoteDb EmoteDb => _stateController!.EmoteDb;

    internal static IEmoteHistoryManager EmoteHistoryManager => _stateController!.EmoteHistoryManager;

    internal static IReadOnlyCollection<string> EmoteKeys => _stateController!.EmoteDb.EmoteKeys;

    internal static string GetEmoteName(string emoteKey)
    {
        var emoteDb = _stateController!.EmoteDb;

        if (emoteDb.EmoteExists(emoteKey))
            return emoteDb.GetEmoteName(emoteKey);

        if (_emoteDisplayData is null)
            return emoteKey;

        return _emoteDisplayData.EmoteKeyEmoteNameLut.GetValueOrDefault(emoteKey, emoteKey);
    }

    internal static string GetEmoteModName(string emoteKey)
    {
        var emoteDb = _stateController!.EmoteDb;
        
        if (emoteDb.EmoteExists(emoteKey))
            return emoteDb.GetModName(emoteKey);
        
        if (_emoteDisplayData is null)
            return "Unknown";

        return _emoteDisplayData.EmoteKeyModNameLut.GetValueOrDefault(emoteKey, "Unknown");
    }

    internal static bool GetEmoteVisibility(string emoteKey) => _stateController!.EmoteDb.GetEmoteVisibility(emoteKey);

    internal static IReadOnlyCollection<string> RandomPoolBlacklist => _stateController!.RandomPoolBlacklist;
    
    internal static IReadOnlyCollection<string> EmotePoolBlacklist => _stateController!.EmotePoolBlacklist;

    internal static void AddToRandomPoolBlacklist(string emoteKey) => _stateController?.AddToRandomPoolBlacklist(emoteKey);

    internal static void RemoveFromRandomPoolBlacklist(string emoteKey) => _stateController?.RemoveFromRandomPoolBlacklist(emoteKey);
    
    internal static void AddToEmoteBlacklist(string emoteKey) => _stateController?.AddToEmoteBlacklist(emoteKey);

    internal static void RemoveFromEmoteBlacklist(string emoteKey) => _stateController?.RemoveFromEmoteBlacklist(emoteKey);

    internal static void RefreshBothLists() => _stateController?.RefreshBothLists();
    
    internal static InputActionReference? GetEmoteKeybind(string? emoteKey)
    {
        if (emoteKey is null)
            return null;

        return _stateController?.GetEmoteKeybind(emoteKey);
    }

    internal static string[] GetEmoteKeysForBindPath(string bindPath) =>
        _stateController!.GetEmoteKeysForBindPath(bindPath);

    internal static EmoteWheelSetData LoadEmoteWheelSetData()
    {
        _emoteDisplayData = _stateController!.LoadEmoteWheelSetDisplayData();
        var wheelSetData = _stateController.LoadEmoteWheelSetData();

        _stateController.SaveEmoteWheelSetDisplayData(_emoteDisplayData.LoadFromWheelSetData(wheelSetData));

        return wheelSetData;
    }

    internal static void SaveEmoteWheelSetData(EmoteWheelSetData dataToSave)
    {
        _stateController!.SaveEmoteWheelSetData(dataToSave);
        
        _emoteDisplayData = _emoteDisplayData!.LoadFromWheelSetData(dataToSave);
        _stateController.SaveEmoteWheelSetDisplayData(_emoteDisplayData);

        if (emoteUiInstance is null)
            return;
        
        emoteUiInstance.ReloadData();
    }
    
    internal static void SaveKeybinds() => _stateController?.SaveKeybinds();
    
    internal static void LoadKeybinds() => _stateController?.LoadKeybinds();

    public static bool IsEmoteWheelsOpen() => emoteUiInstance is
        { IsOpen: true, CurrentView: EmoteUiPanel.UiView.EmoteWheels };

    public static bool IsCustomizePanelOpen() => emoteUiInstance is
        { IsOpen: true, CurrentView: EmoteUiPanel.UiView.Customize };

    public static bool IsEmoteUiOpen() => emoteUiInstance is { IsOpen: true };

    public static bool CanOpenEmoteWheels()
    {
        if (_stateController is null)
            return false;
        
        return _stateController.CanOpenEmoteUi() && !IsCustomizePanelOpen();
    }

    public static void OnLeftWheel()
    {
        if (emoteUiInstance is null || emoteUiInstance.emoteWheelsController is null)
            return;

        if (!IsEmoteWheelsOpen())
            return;
        
        emoteUiInstance.emoteWheelsController.PrevWheel();
    }

    public static void OnRightWheel()
    {
        if (emoteUiInstance is null || emoteUiInstance.emoteWheelsController is null)
            return;
        
        if (!IsEmoteWheelsOpen())
            return;
        
        emoteUiInstance.emoteWheelsController.NextWheel();
    }

    public static void OpenEmoteWheels()
    {
        if (emoteUiInstance is null)
            return;

        if (!CanOpenEmoteWheels())
            return;
        
        emoteUiInstance.Show();
        _stateController?.DisableKeybinds();
    }

    public static void CloseEmoteWheels()
    {
        if (emoteUiInstance is null)
            return;

        if (!IsEmoteWheelsOpen())
            return;
        
        emoteUiInstance.Hide();
        _stateController?.EnableKeybinds();
    }

    public static void CloseCustomizationPanel()
    {
        if (emoteUiInstance is null)
            return;

        if (!IsCustomizePanelOpen())
            return;
        
        emoteUiInstance.Hide();
        _stateController?.EnableKeybinds();
    }

    public static void CloseUiGracefully()
    {
        // CloseCustomizationPanel();

        if (emoteUiInstance is null)
            return;

        // if (!IsEmoteWheelsOpen())
        //     return;
        
        emoteUiInstance.CloseGracefully();
        _stateController?.EnableKeybinds();
    }

    public static void ShowDmcaPrompt()
    {
        if (_stateController is null || _hasShownDmcaPrompt)
            return;
        
        ThreadPool.QueueUserWorkItem(_ =>
        {
            while (emoteUiInstance is null)
            {
                Thread.Sleep(250);
            }
            
            _hasShownDmcaPrompt = true;
            _stateController.EnqueueWorkOnUnityThread(emoteUiInstance.ShowDmcaPrompt);
        });
    }
    
    public static float GetUIScale()
    {
        if (emoteUiInstance is null)
        {
            Debug.LogWarning("EmoteUiInstance is null!");
            return 0f;
        }
        
        return emoteUiInstance.transform.parent.GetComponent<Canvas>().scaleFactor;
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
    
    public static bool DontShowDmcaPrompt
    {
        get => _stateController!.DontShowDmcaPrompt;
        set => _stateController!.DontShowDmcaPrompt = value;
    }
}