using System;

namespace LethalEmotesApi.Ui.Data;

[Serializable]
public class EmoteWheelData(string name)
{
    public string Name { get; set; } = name;
    public string[] Emotes { get; set; } = new string[8];

    public static EmoteWheelData Default(int wheelIndex = 0)
    {
        var wheel = new EmoteWheelData($"Wheel {wheelIndex + 1}");
        Array.Fill(wheel.Emotes, "none");

        return wheel;
    }
}