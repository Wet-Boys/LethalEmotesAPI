using System;
using LethalEmotesApi.Ui.Customize;
using LethalEmotesApi.Ui.Wheel;
using UnityEngine;

namespace LethalEmotesApi.Ui;

public class EmoteUiPanel : MonoBehaviour
{
    public EmoteWheelsController? emoteWheelsController;
    public CustomizePanel? customizePanel;

    private void Awake()
    {
        EmoteUiManager.EmoteUiInstance = this;
    }

    private enum UiView
    {
        None,
        EmoteWheels,
        Customize,
    }
}