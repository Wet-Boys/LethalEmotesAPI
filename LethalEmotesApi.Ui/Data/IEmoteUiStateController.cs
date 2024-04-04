using System.Collections.Generic;
using LethalEmotesApi.Ui.Db;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LethalEmotesApi.Ui.Data;

public interface IEmoteUiStateController
{
    public void PlayEmote(string emoteKey);

    public void LockMouseInput();

    public void UnlockMouseInput();
    
    public void LockPlayerInput();

    public void UnlockPlayerInput();

    public bool CanOpenEmoteUi();

    public void PlayAnimationOn(Animator animator, string emoteKey);

    public IEmoteDb EmoteDb { get; }

    public IReadOnlyCollection<string> RandomPoolBlacklist { get; }
    
    public IReadOnlyCollection<string> EmotePoolBlacklist { get; }

    public void AddToRandomPoolBlacklist(string emoteKey);

    public void RemoveFromRandomPoolBlacklist(string emoteKey);
    
    public void AddToEmoteBlacklist(string emoteKey);

    public void RemoveFromEmoteBlacklist(string emoteKey);
    
    public void RefreshBothLists();

    public InputActionReference? GetEmoteKeybind(string emoteKey);

    public void EnableKeybinds();
    
    public void DisableKeybinds();

    #region Loading Config Data

    public EmoteWheelSetData LoadEmoteWheelSetData();

    public EmoteWheelSetDisplayData LoadEmoteWheelSetDisplayData();

    #endregion
    
    #region Saving Config Data

    public void SaveEmoteWheelSetData(EmoteWheelSetData dataToSave);

    public void SaveEmoteWheelSetDisplayData(EmoteWheelSetDisplayData dataToSave);

    public void SaveKeybinds();

    #endregion

    #region Settings

    public float EmoteVolume { get; set; }
    public bool HideJoinSpots { get; set; }
    public int RootMotionType { get; set; }
    public bool EmotesAlertEnemies { get; set; }
    public int DmcaFree { get; set; }
    public int ThirdPerson { get; set; }
    public bool UseGlobalSettings { get; set; }

    #endregion
}