using LethalEmotesApi.Ui.Customize;
using LethalEmotesApi.Ui.Customize.RebindConflict;
using LethalEmotesApi.Ui.Wheel;
using TMPro;
using UnityEngine;

namespace LethalEmotesApi.Ui;

public class EmoteUiPanel : MonoBehaviour
{
    public EmoteWheelsController? emoteWheelsController;
    public CustomizePanel? customizePanel;
    public RectTransform? customizeButton;

    private TextMeshProUGUI? _customizeButtonLabel;

    public bool IsOpen { get; private set; }
    internal UiView CurrentView { get; private set; } = UiView.EmoteWheels;

    private void Awake()
    {
        EmoteUiManager.EmoteUiInstance = this;

        if (customizeButton is null)
            return;

        if (_customizeButtonLabel is null)
            _customizeButtonLabel = customizeButton.GetComponentInChildren<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        if (customizeButton is null)
            return;

        if (_customizeButtonLabel is null)
            _customizeButtonLabel = customizeButton.GetComponentInChildren<TextMeshProUGUI>();
    }

    public void ReloadData()
    {
        if (emoteWheelsController is null)
            return;

        emoteWheelsController.ReloadWheels();
    }

    public void Show()
    {
        CurrentView = UiView.EmoteWheels;
        UpdateCustomizeButton();
        ShowCustomizeButton();
        ShowEmoteWheels();
        EmoteUiManager.LockMouseInput();

        IsOpen = true;
    }

    public void Hide()
    {
        HideCustomizePanel();
        HideCustomizeButton();
        HideEmoteWheels();
        CurrentView = UiView.EmoteWheels;

        EmoteUiManager.UnlockMouseInput();
        EmoteUiManager.UnlockPlayerInput();
        EmoteUiManager.EnableKeybinds();

        IsOpen = false;
    }

    public void CloseGracefully()
    {
        HideCustomizePanel();
        HideCustomizeButton();
        CloseEmoteWheelsGracefully();
        CurrentView = UiView.EmoteWheels;
    }

    public void ToggleCustomizePanel()
    {
        if (emoteWheelsController is null)
            return;
        if (CurrentView == UiView.EmoteWheels)
        {
            EmoteUiManager.GetStateController()?.RefreshTME();
            CloseEmoteWheelsGracefully();
            ShowCustomizePanel();
            CurrentView = UiView.Customize;

            EmoteUiManager.LockPlayerInput();
        }
        else if (CurrentView == UiView.Customize)
        {
            Hide();
        }

        UpdateCustomizeButton();
    }

    public void ShowEmoteWheels()
    {
        if (emoteWheelsController is null)
            return;

        emoteWheelsController.Show();
        emoteWheelsController.wheelLabel!.gameObject.SetActive(true);
    }

    public void HideEmoteWheels()
    {
        if (emoteWheelsController is null)
            return;

        emoteWheelsController.Hide();
        emoteWheelsController.wheelLabel!.gameObject.SetActive(false);
    }

    public void CloseEmoteWheelsGracefully()
    {
        if (emoteWheelsController is null)
            return;

        emoteWheelsController.CloseGracefully();
        emoteWheelsController.wheelLabel!.gameObject.SetActive(false);
    }

    public void ShowCustomizePanel()
    {
        if (customizePanel is null)
            return;

        customizePanel.gameObject.SetActive(true);
    }

    public void HideCustomizePanel()
    {
        RebindConflictController.CancelExisting();

        if (customizePanel is null)
            return;

        customizePanel.dragDropController!.CancelDrag();

        customizePanel.gameObject.SetActive(false);
        EmoteUiManager.UnlockPlayerInput();
        EmoteUiManager.UnlockMouseInput();
    }

    public void ShowCustomizeButton()
    {
        if (customizeButton is null)
            return;

        customizeButton.gameObject.SetActive(true);
    }

    public void HideCustomizeButton()
    {
        if (customizeButton is null)
            return;

        customizeButton.gameObject.SetActive(false);
    }

    private void UpdateCustomizeButton()
    {
        if (_customizeButtonLabel is null)
            return;

        _customizeButtonLabel.SetText(CurrentView == UiView.EmoteWheels ? "Customize" : "Close");
    }

    private void OnDestroy()
    {
        EmoteUiManager.EmoteUiInstance = null;
    }

    internal enum UiView
    {
        EmoteWheels,
        Customize
    }
}