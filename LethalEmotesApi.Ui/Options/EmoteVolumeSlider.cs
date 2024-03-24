using UnityEngine;
using UnityEngine.UI;

namespace LethalEmotesApi.Ui.Options;

public class EmoteVolumeSlider : MonoBehaviour
{
    public Slider? volumeSlider;

    private bool _hasListener;

    private void Awake()
    {
        UpdateSliderValue();
        EnsureListener();
    }

    private void Start()
    {
        UpdateSliderValue();
        EnsureListener();
    }

    private void OnEnable()
    {
        UpdateSliderValue();
        EnsureListener();
    }
    private void UpdateStateBroadcast()
    {
        UpdateSliderValue();
    }
    private void UpdateSliderValue()
    {
        if (volumeSlider is null)
            return;

        volumeSlider.value = EmoteUiManager.EmoteVolume;
    }

    private void EnsureListener()
    {
        if (volumeSlider is null || _hasListener)
            return;
        
        volumeSlider.onValueChanged.AddListener(SliderChanged);
        _hasListener = true;
    }

    private void SliderChanged(float value)
    {
        EmoteUiManager.EmoteVolume = value;
        SetValueWithoutNotify(value);
    }

    private void SetValueWithoutNotify(float value)
    {
        if (volumeSlider is null)
            return;
        
        volumeSlider.SetValueWithoutNotify(value);
    }
}