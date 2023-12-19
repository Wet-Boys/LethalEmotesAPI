using System;
using BepInEx.Configuration;
using LethalEmotesApi.Ui.Data;
using Newtonsoft.Json;

namespace LethalEmotesAPI.Data;

internal static class EmoteWheelSetDataConverter
{
    internal static string ToJson(this EmoteWheelSetData data)
    {
        DebugClass.Log("I'm gonna pre serialize");
        var text = JsonConvert.SerializeObject(data, Formatting.None);
        DebugClass.Log(text);
        return text;
    }
    
    internal static EmoteWheelSetData FromJson(string text)
    {
        return (EmoteWheelSetData)JsonConvert.DeserializeObject(text, typeof(EmoteWheelSetData));
    }
}