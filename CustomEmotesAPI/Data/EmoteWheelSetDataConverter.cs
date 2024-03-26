using System;
using BepInEx.Configuration;
using LethalEmotesApi.Ui.Data;
using Newtonsoft.Json;

namespace LethalEmotesAPI.Data;

internal static class EmoteWheelSetDataConverter
{
    internal static string ToJson(this EmoteWheelSetData data)
    {
        var text = JsonConvert.SerializeObject(data, Formatting.None);
        return text;
    }
    
    internal static EmoteWheelSetData EmoteWheelSetDataFromJson(string text)
    {
        return (EmoteWheelSetData)JsonConvert.DeserializeObject(text, typeof(EmoteWheelSetData));
    }

    internal static string ToJson(this EmoteWheelSetDisplayData data)
    {
        var text = JsonConvert.SerializeObject(data, Formatting.None);
        return text;
    }

    internal static EmoteWheelSetDisplayData EmoteWheelSetDisplayDataFromJson(string text)
    {
        return (EmoteWheelSetDisplayData)JsonConvert.DeserializeObject(text, typeof(EmoteWheelSetDisplayData));
    }
}