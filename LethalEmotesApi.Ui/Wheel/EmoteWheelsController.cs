using System;
using LethalEmotesApi.Ui.Data;
using UnityEngine;

namespace LethalEmotesApi.Ui.Wheel;

public class EmoteWheelsController : MonoBehaviour
{
    public GameObject? wheelPrefab;
    public RectTransform? wheelContainer;

    public float fadeDist = 500f;
    public float fadeDuration = 0.5f;
    public AnimationCurve? fadeCurve;

    public bool test;

    private EmoteWheel[] _wheels = Array.Empty<EmoteWheel>();
    private EmoteWheelSetData? _wheelSetData = EmoteWheelManager.GetEmoteWheelSetData?.Invoke();
    private int _currentWheelIndex;
    private string _selectedEmote = "";
    
    private void Awake()
    {
        if (test)
        {
            _wheelSetData = new EmoteWheelSetData
            {
                EmoteWheels = [EmoteWheelData.Default(), EmoteWheelData.Default(1), EmoteWheelData.Default(2), EmoteWheelData.Default(3), EmoteWheelData.Default(4)]
            };
        }
    }

    public void Start()
    {
        InitWheels();
    }

    private void InitWheels()
    {
        if (wheelPrefab is null)
            return;
        if (wheelContainer is null)
            return;
        if (_wheelSetData is null)
            return;
        
        int totalWheels = _wheelSetData.EmoteWheels.Length;
        if (_wheels.Length == totalWheels)
            return;
        
        _wheels = new EmoteWheel[totalWheels];
        
        for (int i = 0; i < _wheels.Length; i++)
        {
            var wheelGameObject = Instantiate(wheelPrefab, wheelContainer);
            wheelGameObject.transform.localPosition = Vector3.zero;

            var wheel = wheelGameObject.GetComponent<EmoteWheel>();

            var emoteWheelData = _wheelSetData.EmoteWheels[i];
            wheel.LoadEmotes(emoteWheelData.Emotes);
            wheel.Focused = false;
            wheel.gameObject.SetActive(false);

            _wheels[i] = wheel;
        }

        _currentWheelIndex = 0;
        UpdateWheelState();
    }

    public void NextWheel()
    {
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
        if (wheelContainer is null)
            return;
        wheelContainer.gameObject.SetActive(true);
    }
    
    public void Hide()
    {
        if (wheelContainer is null)
            return;
        
        wheelContainer.gameObject.SetActive(false);
        
        if (string.IsNullOrEmpty(_selectedEmote))
            return;
        
        EmoteWheelManager.EmoteSelected(_selectedEmote);
        _selectedEmote = EmoteWheelManager.EmoteNone;
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