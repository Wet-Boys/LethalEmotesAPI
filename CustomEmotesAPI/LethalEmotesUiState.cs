using System.Collections.Generic;
using System.Linq;
using EmotesAPI;
using LethalEmotesAPI.Data;
using LethalEmotesApi.Ui.Data;
using LethalEmotesApi.Ui.Db;
using LethalEmotesAPI.Utils;
using UnityEngine;
using LethalEmotesApi.Ui;
using UnityEngine.InputSystem;
using LethalEmotesAPI.Patches.ModCompat;

namespace LethalEmotesAPI;

public class LethalEmotesUiState : IEmoteUiStateController
{
    public static readonly LethalEmotesUiState Instance = new();
    
    private LethalEmotesUiState() { }
    
    public void PlayEmote(string emoteKey)
    {
        CustomEmotesAPI.PlayAnimation(emoteKey);
    }

    public void LockMouseInput()
    {
        GameNetworkManager.Instance.localPlayerController.disableLookInput = true;

        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }

    public void UnlockMouseInput()
    {
        GameNetworkManager.Instance.localPlayerController.disableLookInput = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void LockPlayerInput()
    {
        GameNetworkManager.Instance.localPlayerController.quickMenuManager.isMenuOpen = true;
    }

    public void UnlockPlayerInput()
    {
        GameNetworkManager.Instance.localPlayerController.quickMenuManager.isMenuOpen = false;
    }

    public bool CanOpenEmoteUi()
    {
        var localPlayer = GameNetworkManager.Instance.localPlayerController;

        if (localPlayer is null)
            return false;

        return !localPlayer.inTerminalMenu && !localPlayer.isTypingChat && !localPlayer.quickMenuManager.isMenuOpen;
    }
    public void RefreshTME()
    {
        if (CustomEmotesAPI.TMEPresent)
        {
            TooManyEmotesCompat.ReloadTooManyEmotesVisibility();
        }
    }

    public void PlayAnimationOn(Animator animator, string emoteKey)
    {
        BoneMapper.PreviewAnimations(animator, emoteKey);
    }

    private EmoteDb _emoteDb;

    public IEmoteDb EmoteDb
    {
        get
        {
            _emoteDb ??= new EmoteDb();
            return _emoteDb;
        }
    }

    public IReadOnlyCollection<string> RandomPoolBlacklist => BlacklistSettings.emotesExcludedFromRandom;
    
    public IReadOnlyCollection<string> EmotePoolBlacklist => BlacklistSettings.emotesDisabled;

    public void AddToRandomPoolBlacklist(string emoteKey)
    {
        BlacklistSettings.AddToExcludeList(emoteKey);
    }

    public void RemoveFromRandomPoolBlacklist(string emoteKey)
    {
        BlacklistSettings.RemoveFromExcludeList(emoteKey);
    }

    public void AddToEmoteBlacklist(string emoteKey)
    {
        BlacklistSettings.AddToDisabledList(emoteKey);
    }

    public void RemoveFromEmoteBlacklist(string emoteKey)
    {
        BlacklistSettings.RemoveFromDisabledList(emoteKey);
    }
    
    public void RefreshBothLists()
    {
        BlacklistSettings.RefreshBothLists();
    }

    public InputActionReference GetEmoteKeybind(string emoteKey) => Keybinds.GetOrCreateInputRef(emoteKey);

    public void EnableKeybinds() => Keybinds.EnableKeybinds();

    public void DisableKeybinds() => Keybinds.DisableKeybinds();
    
    public string[] GetEmoteKeysForBindPath(string bindPath) => Keybinds.GetEmoteKeysForBindPath(bindPath);

    internal static void FixLegacyEmotes()
    {
        EmoteWheelSetData e = EmoteWheelSetDataConverter.EmoteWheelSetDataFromJson(Settings.EmoteWheelSetDataEntryString.Value);
        foreach (var wheel in e.EmoteWheels)
        {
            for (int i = 0; i < wheel.Emotes.Length; i++)
            {
                if (!BoneMapper.animClips.ContainsKey(wheel.Emotes[i]))
                {
                    foreach (var key in BoneMapper.animClips.Keys)
                    {
                        if (key.Contains(wheel.Emotes[i]) || ((BoneMapper.animClips[key] is not null && BoneMapper.animClips[key].animates) && BoneMapper.animClips[key].clip[0].name == wheel.Emotes[i]))
                        {
                            wheel.Emotes[i] = key;
                            break;
                        }
                    }
                }
            }
        }
        Settings.EmoteWheelSetDataEntryString.Value = e.ToJson();
    }

    public EmoteWheelSetData LoadEmoteWheelSetData()
    {
        return EmoteWheelSetDataConverter.EmoteWheelSetDataFromJson(Settings.EmoteWheelSetDataEntryString.Value);
    }

    public EmoteWheelSetDisplayData LoadEmoteWheelSetDisplayData()
    {
        return EmoteWheelSetDataConverter.EmoteWheelSetDisplayDataFromJson(Settings.EmoteWheelSetDisplayDataString.Value);
    }

    public void LoadKeybinds() => Keybinds.LoadKeybinds();

    public void SaveEmoteWheelSetData(EmoteWheelSetData dataToSave)
    {
        Settings.EmoteWheelSetDataEntryString.Value = dataToSave.ToJson();
    }

    public void SaveEmoteWheelSetDisplayData(EmoteWheelSetDisplayData dataToSave)
    {
        Settings.EmoteWheelSetDisplayDataString.Value = dataToSave.ToJson();
    }

    public void SaveKeybinds() => Keybinds.SaveKeybinds();

    public float EmoteVolume
    {
        get => Settings.EmotesVolume.Value;
        set => Settings.EmotesVolume.Value = value;
    }

    public bool HideJoinSpots
    {
        get => Settings.HideJoinSpots.Value;
        set => Settings.HideJoinSpots.Value = value;
    }

    public int RootMotionType
    {
        get => (int)Settings.rootMotionType.Value;
        set => Settings.rootMotionType.Value = (RootMotionType)value;
    }

    public bool EmotesAlertEnemies
    {
        get => Settings.EmotesAlertEnemies.Value;
        set => Settings.EmotesAlertEnemies.Value = value;
    }

    public int DmcaFree
    {
        get => (int)Settings.DMCAFree.Value;
        set => Settings.DMCAFree.Value = (DMCAType)value;
    }

    public int ThirdPerson
    {
        get => (int)Settings.thirdPersonType.Value;
        set => Settings.thirdPersonType.Value = (ThirdPersonType)value;
    }
    
    public bool UseGlobalSettings
    {
        get => Settings.useGlobalConfig.Value;
        set => Settings.useGlobalConfig.Value = value;
    }
}