using System;
using System.Collections.Generic;

namespace LethalEmotesApi.Ui.Data;

[Serializable]
public class EmoteWheelSetDisplayData
{
    public Dictionary<string, string> EmoteKeyModNameLut { get; set; } = new();
    public Dictionary<string, string> EmoteKeyEmoteNameLut { get; set; } = new();

    public static EmoteWheelSetDisplayData FromEmoteWheelSetData(EmoteWheelSetData wheelSetData)
    {
        var stateController = EmoteUiManager.GetStateController()!;
        var emoteDb = stateController.EmoteDb;

        var displayData = new EmoteWheelSetDisplayData();
        
        foreach (var wheel in wheelSetData.EmoteWheels)
        {
            foreach (var emoteKey in wheel.Emotes)
            {
                displayData.EmoteKeyModNameLut[emoteKey] = emoteDb.GetModName(emoteKey);
                displayData.EmoteKeyEmoteNameLut[emoteKey] = emoteDb.GetEmoteName(emoteKey);
            }
        }

        return displayData;
    }
}