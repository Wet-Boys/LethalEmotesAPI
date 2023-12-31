﻿using System.Collections.Generic;
using LethalEmotesApi.Ui.Db;
using UnityEngine;

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

    public void AddToRandomPoolBlacklist(string emoteKey);

    public void RemoveFromRandomPoolBlacklist(string emoteKey);
    
    #region Loading Config Data

    public EmoteWheelSetData LoadEmoteWheelSetData();

    #endregion
    
    #region Saving Config Data

    public void SaveEmoteWheelSetData(EmoteWheelSetData dataToSave);

    #endregion
    
    public float EmoteVolume { get; set; }
    public bool HideJoinSpots { get; set; }
    public int RootMotionType { get; set; }
    public bool EmotesAlertEnemies { get; set; }
    public int DmcaFree { get; set; }
    public int ThirdPerson { get; set; }
}