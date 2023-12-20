using System;

namespace LethalEmotesApi.Ui.Data;

[Serializable]
public class EmoteWheelSetData
{
    public EmoteWheelData[] EmoteWheels { get; set; } = [];

    public int IndexOfDefault()
    {
        int index = -1;
        for (int i = 0; i < EmoteWheels.Length; i++)
        {
            if (EmoteWheels[i].IsDefaultWheel())
            {
                index = i;
                break;
            }
        }

        return index;
    }

    public static EmoteWheelSetData Default()
    {
        return new EmoteWheelSetData
        {
            EmoteWheels = [EmoteWheelData.CreateDefault()]
        };
    }
}