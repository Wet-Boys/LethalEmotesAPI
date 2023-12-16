using System;

namespace LethalEmotesApi.Ui.Data;

[Serializable]
public class EmoteWheelSetData
{
    public EmoteWheelData[] EmoteWheels { get; set; } = [];

    public static EmoteWheelSetData Default()
    {
        return new EmoteWheelSetData
        {
            EmoteWheels = [EmoteWheelData.Default()]
        };
    }
}