using System;
using LethalEmotesApi.Ui.Customize;
using LethalEmotesApi.Ui.Wheel;
using UnityEngine;

namespace LethalEmotesApi.Ui;

public class EmoteUiPanel : MonoBehaviour
{
    public EmoteWheelsController? emoteWheelsController;
    public CustomizePanel? customizePanel;
    public RectTransform? customizeButton;

    private void Awake()
    {
        EmoteUiManager.EmoteUiInstance = this;
    }

    public void ShowCustomizePanel()
    {
        if (customizePanel is null)
            return;
        
        customizePanel.gameObject.SetActive(true);
    }

    public void HideCustomizePanel()
    {
        if (customizePanel is null)
            return;
        
        customizePanel.gameObject.SetActive(false);
    }

    private enum UiView
    {
        None,
        EmoteWheels,
        Customize,
    }
}