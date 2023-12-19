using LethalEmotesApi.Ui.Data;
using TMPro;
using UnityEngine.EventSystems;

namespace LethalEmotesApi.Ui.Customize.Wheel;

public class CustomizeWheelController : UIBehaviour
{
    public CustomizeWheel? customizeWheel;
    public TMP_InputField? wheelLabel;

    private int _wheelIndex = -1;

    protected override void Awake()
    {
        base.Awake();

        if (customizeWheel is null)
            return;

        if (wheelLabel is null)
            return;
        
        customizeWheel.OnEmoteChanged.AddListener(EmoteAtIndexChanged);
        wheelLabel.onValueChanged.AddListener(WheelLabelChanged);

        if (!HasWheels)
            return;

        _wheelIndex = 0;
        UpdateState();
    }

    public void NextWheel()
    {
        if (customizeWheel is null)
            return;

        _wheelIndex++;
        if (_wheelIndex >= WheelCount)
            _wheelIndex = 0;
        
        UpdateState();
    }

    public void PrevWheel()
    {
        if (customizeWheel is null)
            return;

        _wheelIndex--;
        if (_wheelIndex < 0)
            _wheelIndex = WheelCount - 1;
        
        UpdateState();
    }

    public void LoadWheelIndex(int wheelIndex)
    {
        if (customizeWheel is null)
            return;
        
        var wheelData = WheelSetData.EmoteWheels[wheelIndex];
        
        customizeWheel.LoadEmoteData(wheelData.Emotes);

        if (wheelLabel is null)
            return;
        
        wheelLabel.SetTextWithoutNotify(wheelData.Name);
        _wheelIndex = wheelIndex;
    }
    
    private void WheelLabelChanged(string wheelName)
    {
        var wheelSetData = WheelSetData;
        wheelSetData.EmoteWheels[_wheelIndex].Name = wheelName;
        
        EmoteUiManager.SaveEmoteWheelSetData(wheelSetData);
        UpdateState();
    }
    
    private void EmoteAtIndexChanged(int segmentIndex, string emoteKey)
    {
        var wheelSetData = WheelSetData;
        wheelSetData.EmoteWheels[_wheelIndex].Emotes[segmentIndex] = emoteKey;
        
        EmoteUiManager.SaveEmoteWheelSetData(wheelSetData);
        UpdateState();
    }

    private void UpdateState()
    {
        LoadWheelIndex(_wheelIndex);
    }
    
    private EmoteWheelSetData WheelSetData => EmoteUiManager.LoadEmoteWheelSetData();
    private int WheelCount => WheelSetData.EmoteWheels.Length;
    private bool HasWheels => WheelCount > 0;
}