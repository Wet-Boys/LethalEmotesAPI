using System;
using System.Collections;
using LethalEmotesApi.Ui.Data;
using UnityEngine;

namespace LethalEmotesApi.Ui;

public class EmoteWheelsController : MonoBehaviour
{
    [Range(10, 2000)] public float xScale = 650f;
    [Range(10, 2000)] public float zScale = 360f;
    
    public GameObject? wheelPrefab;
    public AnimationCurve tweenCurve;
    public AnimationCurve distScaleCurve;

    public RectTransform? wheelContainer;

    private EmoteWheel[] _wheels = Array.Empty<EmoteWheel>();
    private EmoteWheel? _focusedWheel;
    private IEnumerator? _moveCoroutine;
    private EmoteWheelSetData? _wheelSetData = EmoteWheelManager.GetEmoteWheelSetData?.Invoke();
    private int _currentWheelTotal;
    private string _selectedEmote = "";

    private void Awake()
    {
        EmoteWheelManager.WheelControllerInstance = this;
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
        
        var pos = transform.position;
        float degPer = (float)(Math.PI * 2 / totalWheels);
            
        _wheels = new EmoteWheel[totalWheels];
        for (int i = 0; i < _wheels.Length; i++)
        {
            float rad = i * degPer;
            float x = (float)(pos.x + Math.Sin(rad) * xScale);
            float z = (float)(pos.z + -Math.Cos(rad) * zScale);
            
            var wheel = Instantiate(wheelPrefab, wheelContainer);
            wheel.transform.position = new Vector3(x, pos.y, z + zScale);

            _wheels[i] = wheel.GetComponent<EmoteWheel>();

            var emoteWheelData = _wheelSetData.EmoteWheels[i];
            _wheels[i].LoadEmotes(emoteWheelData.Emotes);
        }

        _currentWheelTotal = totalWheels;

        UpdateWheelState();
        UpdateWheelPositions();
    }

    public void NextWheel()
    {
        EmoteWheel[] temp = new EmoteWheel[_currentWheelTotal];
        Array.Copy(_wheels, temp, _currentWheelTotal);

        for (int i = 0; i < _currentWheelTotal; i++)
        {
            _wheels[i].Focused = false;
            _wheels[i].DeSelectAll();
            
            int index = i + 1;

            if (index >= _currentWheelTotal)
                index = 0;

            _wheels[i] = temp[index];
        }

        UpdateWheelState();
        UpdateWheelPositions();
    }

    public void PrevWheel()
    {
        EmoteWheel[] temp = new EmoteWheel[_currentWheelTotal];
        Array.Copy(_wheels, temp, _currentWheelTotal);

        for (int i = 0; i < _currentWheelTotal; i++)
        {
            _wheels[i].Focused = false;
            _wheels[i].DeSelectAll();
            
            int index = i - 1;

            if (index < 0)
                index = _currentWheelTotal - 1;

            _wheels[i] = temp[index];
        }

        UpdateWheelState();
        UpdateWheelPositions();
    }

    private void OnDisable()
    {
        if (string.IsNullOrEmpty(_selectedEmote))
            return;
        
        EmoteWheelManager.EmoteSelected(_selectedEmote);
    }

    private void UpdateWheelState()
    {
        _focusedWheel = _wheels[0];
        _focusedWheel.Focused = true;
        
        _focusedWheel.AddOnEmoteSelectedCallback(UpdateSelectedEmote);
    }

    private void UpdateWheelPositions()
    {
        if (_moveCoroutine is not null)
            StopCoroutine(_moveCoroutine);

        _moveCoroutine = MoveWheelCoroutine();
        StartCoroutine(_moveCoroutine);
    }

    private void UpdateSelectedEmote(string selectedEmote)
    {
        _selectedEmote = selectedEmote;
    }

    private IEnumerator MoveWheelCoroutine()
    {
        var pos = transform.position;
        float degPer = (float)(Math.PI * 2 / _currentWheelTotal);
            
        float deltaTime = 0f;
        float percent;
        do
        {
            deltaTime += Time.unscaledDeltaTime;
            percent = tweenCurve.Evaluate(deltaTime);
                
            for (int i = 0; i < _currentWheelTotal; i++)
            {
                var wheel = _wheels[i];
                var wheelTransform = wheel.transform;
                    
                var startPos = wheelTransform.position;
                    
                float rad = i * degPer;
                float x = (float)(pos.x + Math.Sin(rad) * xScale);
                float z = (float)(pos.z + -Math.Cos(rad) * zScale);

                var endPos = new Vector3(x, pos.y, z + zScale);
                        
                wheelTransform.position = Vector3.Lerp(startPos, endPos, percent);
                    
                var dist = Mathf.Min(i, _currentWheelTotal - i);

                var startScale = wheelTransform.localScale;
                var scale = distScaleCurve.Evaluate(dist);
                var endScale = new Vector3(scale, scale, scale);

                wheelTransform.localScale = Vector3.Lerp(startScale, endScale, percent);
                
                Debug.Log($"{i}: {dist}");
            }

            yield return new WaitForEndOfFrame();
        } while (percent != 1);
    }
}