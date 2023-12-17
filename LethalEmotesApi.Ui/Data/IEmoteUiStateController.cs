namespace LethalEmotesApi.Ui.Data;

public interface IEmoteUiStateController
{
    public void PlayEmote(string emoteKey);
    
    

    #region Loading Config Data

    public EmoteWheelSetData LoadEmoteWheelSetData();

    #endregion
    
    

    #region Saving Config Data

    public void SaveEmoteWheelSetData(EmoteWheelSetData dataToSave);

    #endregion
}