using System.Collections.Generic;
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

    public IReadOnlyCollection<string> GetAllEmoteNames();
    
    #region Loading Config Data

    public EmoteWheelSetData LoadEmoteWheelSetData();

    #endregion
    
    #region Saving Config Data

    public void SaveEmoteWheelSetData(EmoteWheelSetData dataToSave);

    #endregion
}