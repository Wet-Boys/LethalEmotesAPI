using System.Collections.Generic;
using System.Linq;
using EmotesAPI;
using LethalEmotesAPI.Data;
using LethalEmotesApi.Ui.Data;
using LethalEmotesApi.Ui.Db;
using LethalEmotesAPI.Utils;
using UnityEngine;

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
    
    public void AddToRandomPoolBlacklist(string emoteKey)
    {
        BlacklistSettings.AddToExcludeList(emoteKey);
    }

    public void RemoveFromRandomPoolBlacklist(string emoteKey)
    {
        BlacklistSettings.RemoveFromExcludeList(emoteKey);
    }

    public EmoteWheelSetData LoadEmoteWheelSetData()
    {
        return EmoteWheelSetDataConverter.FromJson(Settings.EmoteWheelSetDataEntryString.Value);
    }

    public void SaveEmoteWheelSetData(EmoteWheelSetData dataToSave)
    {
        Settings.EmoteWheelSetDataEntryString.Value = dataToSave.ToJson();
    }

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
}