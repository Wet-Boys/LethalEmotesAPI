using System;
using LethalEmotesApi.Ui.Data;
using TMPro;
using UnityEngine;

namespace LethalEmotesApi.Ui.Wheel;

public class EmoteWheelsController : MonoBehaviour
{
    public GameObject? wheelPrefab;
    public RectTransform? wheelContainer;
    public TextMeshProUGUI? wheelLabel;

    public float fadeDist = 500f;
    public float fadeDuration = 0.5f;
    public AnimationCurve? fadeCurve;

    private EmoteWheel[] _wheels = Array.Empty<EmoteWheel>();
    private EmoteWheelSetData WheelSetData => EmoteUiManager.LoadEmoteWheelSetData();
    private int _currentWheelIndex;
    private string _selectedEmote = "";
    private bool _wheelLock;
    
    public void Start()
    {
        InitWheels();
    }

    private void OnEnable()
    {
        ReloadWheels();
    }

    public void ReloadWheels()
    {
        if (WheelSetData.EmoteWheels.Length != _wheels.Length)
        {
            foreach (var wheel in _wheels)
                DestroyImmediate(wheel.gameObject);
            
            InitWheels();
            return;
        }

        for (var i = 0; i < _wheels.Length; i++)
        {
            var wheel = _wheels[i];
            var wheelData = WheelSetData.EmoteWheels[i];
            
            wheel.gameObject.SetActive(true);
            wheel.LoadEmotes(wheelData.Emotes);
        }
    }

    public void LockWheels()
    {
        _wheelLock = true;

        if (_currentWheelIndex < 0)
            return;
        
        _wheels[_currentWheelIndex].DeSelectAll();
    }

    public void UnlockWheels()
    {
        _wheelLock = false;
    }

    private void InitWheels()
    {
        if (wheelPrefab is null)
            return;
        if (wheelContainer is null)
            return;
        
        int totalWheels = WheelSetData.EmoteWheels.Length;
        if (_wheels.Length == totalWheels)
            return;
        
        _wheels = new EmoteWheel[totalWheels];
        
        for (int i = 0; i < _wheels.Length; i++)
        {
            var wheelGameObject = Instantiate(wheelPrefab, wheelContainer);
            wheelGameObject.transform.localPosition = Vector3.zero;

            var wheel = wheelGameObject.GetComponent<EmoteWheel>();

            var emoteWheelData = WheelSetData.EmoteWheels[i];
            wheel.LoadEmotes(emoteWheelData.Emotes);
            wheel.Focused = false;
            wheel.gameObject.SetActive(false);

            _wheels[i] = wheel;
        }

        _currentWheelIndex = 0;
        
        var defaultIndex = WheelSetData.IndexOfDefault();
        if (defaultIndex >= 0)
            _currentWheelIndex = defaultIndex;
        
        UpdateWheelState();
    }

    public void NextWheel()
    {
        if (_wheelLock)
            return;
        
        var prevWheelIndex = _currentWheelIndex;
        
        _currentWheelIndex++;
        if (_currentWheelIndex >= _wheels.Length)
            _currentWheelIndex = 0;

        var prevWheel = _wheels[prevWheelIndex];
        prevWheel.Focused = false;
        prevWheel.DeSelectAll();
        
        FadeWheelLeft(prevWheelIndex);
        UpdateWheelState();
    }

    public void PrevWheel()
    {
        if (_wheelLock)
            return;
        
        var prevWheelIndex = _currentWheelIndex;
        
        _currentWheelIndex--;
        if (_currentWheelIndex < 0)
            _currentWheelIndex = _wheels.Length - 1;

        var prevWheel = _wheels[prevWheelIndex];
        prevWheel.Focused = false;
        prevWheel.DeSelectAll();
        
        FadeWheelRight(prevWheelIndex);
        UpdateWheelState();
    }

    public void Show()
    {
        UnlockWheels();
        if (wheelContainer is null)
            return;

        var defaultIndex = WheelSetData.IndexOfDefault();
        if (defaultIndex >= 0)
            _currentWheelIndex = defaultIndex;

        var wheel = GetCurrentWheel();
        wheel.gameObject.SetActive(true);
        
        wheelContainer.gameObject.SetActive(true);

        if (wheelLabel is null)
            return;
        
        wheelLabel.gameObject.SetActive(true);
        UpdateWheelState();
    }
    
    public void Hide()
    {
        UnlockWheels();
        if (wheelContainer is null)
            return;

        foreach (var wheel in _wheels)
            wheel.gameObject.SetActive(false);
        
        wheelContainer.gameObject.SetActive(false);
        
        if (string.IsNullOrEmpty(_selectedEmote))
            return;
        
        EmoteUiManager.PlayEmote(_selectedEmote);
        _selectedEmote = "none";

        if (wheelLabel is null)
            return;
        
        wheelLabel.gameObject.SetActive(false);
    }

    public void CloseGracefully()
    {
        if (wheelContainer is null)
            return;

        foreach (var wheel in _wheels)
            wheel.gameObject.SetActive(false);
        
        wheelContainer.gameObject.SetActive(false);
        _selectedEmote = "";
        
        if (wheelLabel is null)
            return;
        
        wheelLabel.gameObject.SetActive(false);
    }

    private EmoteWheel GetCurrentWheel()
    {
        return _wheels[_currentWheelIndex];
    }

    private void UpdateWheelState()
    {
        var wheel = GetCurrentWheel();
        
        wheel.gameObject.SetActive(true);

        var wheelTransform = wheel.transform;
        
        wheelTransform.SetAsLastSibling();
        wheelTransform.localPosition = Vector3.zero;
        
        wheel.ResetState();
        wheel.Focused = true;
        wheel.AddOnEmoteSelectedCallback(UpdateSelectedEmote);

        var data = WheelSetData.EmoteWheels[_currentWheelIndex];
        wheelLabel!.SetText(data.Name);
    }

    private void UpdateSelectedEmote(string selectedEmote)
    {
        _selectedEmote = selectedEmote;
    }
    
    private void FadeWheelLeft(int wheelIndex, bool instant = false) => FadeWheel(wheelIndex, true, instant);
    private void FadeWheelRight(int wheelIndex, bool instant = false) => FadeWheel(wheelIndex, false, instant);

    private void FadeWheel(int wheelIndex, bool left, bool instant = false)
    {
        if (fadeCurve is null)
            return;
        
        var wheel = _wheels[wheelIndex];

        var targetPos = new Vector3(left ? -fadeDist : fadeDist, 0, 0);        
        wheel.TweenPos(targetPos, fadeCurve, instant ? 0f : fadeDuration, true);
        wheel.TweenAlpha(0f, fadeCurve, instant ? 0f : fadeDuration, true);
        wheel.DisableAfterDuration(instant ? 0f : fadeDuration, true);
    }
}