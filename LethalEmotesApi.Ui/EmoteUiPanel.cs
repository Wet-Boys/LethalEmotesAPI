using LethalEmotesApi.Ui.Customize;
using LethalEmotesApi.Ui.Wheel;
using TMPro;
using UnityEngine;

namespace LethalEmotesApi.Ui;

public class EmoteUiPanel : MonoBehaviour
{
    public EmoteWheelsController? emoteWheelsController;
    public CustomizePanel? customizePanel;
    public RectTransform? customizeButton;
    public GameObject? previewCube;
    
    private TextMeshProUGUI? _customizeButtonLabel;
    private static GameObject? _previewInstance;

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

        if (previewCube is null)
            return;

        if (_previewInstance is not null)
        {
            DestroyImmediate(_previewInstance);
            _previewInstance = null;
        }

        _previewInstance = Instantiate(previewCube, new Vector3(0, 0, 10000f), Quaternion.Euler(0, 0, 0));
        _previewInstance.SetActive(true);

        var animator = _previewInstance.GetComponentInChildren<Animator>();
        EmoteUiManager.PlayAnimationOn(animator, "CaliforniaGirls");
    }

    public void HideCustomizePanel()
    {
        if (customizePanel is null)
            return;
        
        customizePanel.gameObject.SetActive(false);
        EmoteUiManager.UnlockPlayerInput();
        EmoteUiManager.UnlockMouseInput();
        
        if (_previewInstance is null)
            return;
        
        _previewInstance.SetActive(false);
        DestroyImmediate(_previewInstance);
        _previewInstance = null;
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