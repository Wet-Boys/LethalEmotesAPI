using System;
using BepInEx.Configuration;
using LethalEmotesApi.Ui.Data;
using Newtonsoft.Json;

namespace LethalEmotesAPI.Data;

public static class EmoteWheelSetDataConverter
{
    internal static void Init()
    {
        var converter = new TypeConverter
        {
            ConvertToString = ConvertToString,
            ConvertToObject = ConvertToObject
        };

        TomlTypeConverter.AddConverter(typeof(EmoteWheelSetData), converter);
    }

    private static string ConvertToString(object obj, Type type)
    {
        if (type != typeof(EmoteWheelSetData))
            throw new InvalidOperationException();

        return JsonConvert.SerializeObject((EmoteWheelSetData)obj, Formatting.None);
    }
    
    private static object ConvertToObject(string text, Type type)
    {
        if (type != typeof(EmoteWheelSetData))
            throw new InvalidOperationException();

        return JsonConvert.DeserializeObject(text, type);
    }
}