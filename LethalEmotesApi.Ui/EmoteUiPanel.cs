﻿using LethalEmotesApi.Ui.Customize;
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
    
    public GameObject? dmcaPromptPrefab;
    public GameObject? dmcaVerificationPromptPrefab;

    private TextMeshProUGUI? _customizeButtonLabel;
    private GameObject? _dmcaPromptInstance;
    private GameObject? _dmcaVerificationPromptInstance;

    public bool IsOpen { get; private set; }
    internal UiView CurrentView { get; private set; } = UiView.EmoteWheels;
    private UiView _prevView;

    private void Awake()
    {
        EmoteUiManager.emoteUiInstance = this;

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
        _prevView = UiView.None;
        
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
        CloseDmcaPrompt();
        CloseDmcaVerificationPrompt();
        
        CurrentView = UiView.EmoteWheels;
        _prevView = UiView.None;

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
        CloseDmcaVerificationPrompt();

        if (_prevView == UiView.DmcaPrompt)
        {
            CurrentView = _prevView;
            _prevView = UiView.None;
            
            EmoteUiManager.LockMouseInput();
            EmoteUiManager.LockPlayerInput();
            EmoteUiManager.DisableKeybinds();
            
            return;
        }
        
        CloseDmcaPrompt();
        
        CurrentView = UiView.EmoteWheels;
        _prevView = UiView.None;
        
        IsOpen = false;
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

    public void ShowDmcaPrompt()
    {
        if (dmcaPromptPrefab is null)
            return;

        CloseGracefully();

        if (_dmcaPromptInstance is not null && _dmcaPromptInstance)
        {
            DestroyImmediate(_dmcaPromptInstance);
            _dmcaPromptInstance = null;
        }

        CurrentView = UiView.DmcaPrompt;
        _dmcaPromptInstance = Instantiate(dmcaPromptPrefab, transform);
        
        EmoteUiManager.LockMouseInput();
        EmoteUiManager.LockPlayerInput();
        EmoteUiManager.DisableKeybinds();
        
        IsOpen = true;
    }

    private void CloseDmcaPrompt()
    {
        if (_dmcaPromptInstance is null)
            return;
        
        DestroyImmediate(_dmcaPromptInstance);
        _dmcaPromptInstance = null;
    }

    public void ShowDmcaVerificationPrompt()
    {
        if (dmcaVerificationPromptPrefab is null)
            return;

        if (CurrentView == UiView.DmcaPrompt)
            _prevView = UiView.DmcaPrompt;
        else
            CloseGracefully();

        if (_dmcaVerificationPromptInstance is not null && _dmcaVerificationPromptInstance)
        {
            DestroyImmediate(_dmcaVerificationPromptInstance);
            _dmcaVerificationPromptInstance = null;
        }

        CurrentView = UiView.DmcaVerificationPrompt;
        _dmcaVerificationPromptInstance = Instantiate(dmcaVerificationPromptPrefab, transform);
        
        EmoteUiManager.LockMouseInput();
        EmoteUiManager.LockPlayerInput();
        EmoteUiManager.DisableKeybinds();

        IsOpen = true;
    }

    private void CloseDmcaVerificationPrompt()
    {
        if (_dmcaVerificationPromptInstance is null)
            return;
        
        DestroyImmediate(_dmcaVerificationPromptInstance);
        _dmcaVerificationPromptInstance = null;
    }

    private void OnDestroy()
    {
        EmoteUiManager.emoteUiInstance = null;
    }

    internal enum UiView
    {
        None,
        EmoteWheels,
        Customize,
        DmcaPrompt,
        DmcaVerificationPrompt
    }
}