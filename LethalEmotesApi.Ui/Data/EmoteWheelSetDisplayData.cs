using System;
using System.Collections.Generic;
using System.Linq;

namespace LethalEmotesApi.Ui.Data;

[Serializable]
public class EmoteWheelSetDisplayData
{
    public Dictionary<string, string> EmoteKeyModNameLut { get; set; } = new();
    public Dictionary<string, string> EmoteKeyEmoteNameLut { get; set; } = new();

    public EmoteWheelSetDisplayData LoadFromWheelSetData(EmoteWheelSetData wheelSetData)
    {
        var wheelEmoteKeys = wheelSetData.EmoteWheels.SelectMany(wheel => wheel.Emotes);

        EmoteKeyModNameLut = EmoteKeyModNameLut.Where(kvp => wheelEmoteKeys.Contains(kvp.Key))
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        
        EmoteKeyEmoteNameLut = EmoteKeyEmoteNameLut.Where(kvp => wheelEmoteKeys.Contains(kvp.Key))
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        
        var stateController = EmoteUiManager.GetStateController()!;
        var emoteDb = stateController.EmoteDb;

        foreach (var emoteKey in wheelEmoteKeys)
        {
            if (!EmoteKeyModNameLut.ContainsKey(emoteKey))
                EmoteKeyModNameLut[emoteKey] = emoteDb.GetModName(emoteKey);

            if (!EmoteKeyEmoteNameLut.ContainsKey(emoteKey))
                EmoteKeyEmoteNameLut[emoteKey] = emoteDb.GetEmoteName(emoteKey);
        }

        return this;
    }
}