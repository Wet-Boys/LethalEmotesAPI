using System.Linq;
using LethalEmotesApi.Ui.Data;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace LethalEmotesApi.Ui.Customize.Wheel;

public class CustomizeWheelController : UIBehaviour
{
    public CustomizeWheel? customizeWheel;
    public TMP_InputField? wheelLabel;
    public GameObject? deleteConfirmationPrefab;
    private DeleteWheelPopup? _deleteWheelPopupInstance;

    private EmoteUiPanel? _emoteUiPanel;

    private int _wheelIndex = -1;

    protected override void Awake()
    {
        base.Awake();

        if (_emoteUiPanel is null)
            _emoteUiPanel = GetComponentInParent<EmoteUiPanel>();

        if (customizeWheel is null)
            return;

        if (wheelLabel is null)
            return;
        
        customizeWheel.OnEmoteChanged.AddListener(EmoteAtIndexChanged);
        customizeWheel.OnEmotesSwapped.AddListener(EmotesSwapped);
        wheelLabel.onValueChanged.AddListener(WheelLabelChanged);

        if (!HasWheels)
            return;

        _wheelIndex = 0;
        UpdateState();
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        if (_deleteWheelPopupInstance is not null)
        {
            DestroyImmediate(_deleteWheelPopupInstance.gameObject);
            _deleteWheelPopupInstance = null;
        }
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

    public void NewWheel()
    {
        if (customizeWheel is null)
            return;

        var newIndex = _wheelIndex + 1;
        var wheels = WheelSetData.EmoteWheels;
        
        var newWheel = EmoteWheelData.CreateDefault(wheels.Length);
        
        var newWheels = wheels[..newIndex].Concat([newWheel]);
        if (newIndex < wheels.Length)
            newWheels = newWheels.Concat(wheels[newIndex..]);

        var newData = new EmoteWheelSetData
        {
            EmoteWheels = newWheels.ToArray()
        };
        EmoteUiManager.SaveEmoteWheelSetData(newData);

        _wheelIndex = newIndex;
        UpdateState();
    }

    public void TryDelete()
    {
        if (deleteConfirmationPrefab is null)
            return;

        var instance = Instantiate(deleteConfirmationPrefab, _emoteUiPanel!.transform);

        _deleteWheelPopupInstance = instance.GetComponent<DeleteWheelPopup>();
        _deleteWheelPopupInstance.customizeWheelController = this;
    }

    public void DeleteWheel()
    {
        if (_deleteWheelPopupInstance is null)
            return;
        
        DestroyImmediate(_deleteWheelPopupInstance.gameObject);
        _deleteWheelPopupInstance = null;
        
        if (customizeWheel is null)
            return;

        if (_wheelIndex < 0)
            return;

        var wheels = WheelSetData.EmoteWheels;
        if (wheels.Length <= 0)
            return;
        
        if (wheels.Length == 1)
        {
            wheels[0] = EmoteWheelData.CreateDefault();
            
            var resetData = new EmoteWheelSetData
            {
                EmoteWheels = wheels
            };
            EmoteUiManager.SaveEmoteWheelSetData(resetData);
            _wheelIndex = 0;
            UpdateState();
            return;
        }

        var newWheels = new EmoteWheelData[wheels.Length - 1];

        var i = 0;
        var offset = 0;
        do
        {
            if (i == _wheelIndex)
            {
                offset = 1;
                i++;
                continue;
            }

            newWheels[i - offset] = wheels[i]; 
            i++;
        } while (i < wheels.Length);

        var newData = new EmoteWheelSetData
        {
            EmoteWheels = newWheels
        };
        EmoteUiManager.SaveEmoteWheelSetData(newData);
        PrevWheel();
    }

    public void ToggleDefault()
    {
        if (_wheelIndex < 0)
            return;

        if (customizeWheel is null)
            return;
        
        var wheels = WheelSetData.EmoteWheels;

        var existingDefaultIndex = WheelSetData.IndexOfDefault();
        if (existingDefaultIndex < 0)
        {
            wheels[_wheelIndex].IsDefault = true;

            var newData = new EmoteWheelSetData
            {
                EmoteWheels = wheels
            };
            EmoteUiManager.SaveEmoteWheelSetData(newData);
            UpdateState();
            return;
        }

        wheels[existingDefaultIndex].IsDefault = false;
        if (existingDefaultIndex != _wheelIndex)
            wheels[_wheelIndex].IsDefault = true;
        
        var data = new EmoteWheelSetData
        {
            EmoteWheels = wheels
        };
        EmoteUiManager.SaveEmoteWheelSetData(data);
        UpdateState();
    }

    public void LoadWheelIndex(int wheelIndex)
    {
        if (customizeWheel is null)
            return;
        
        var wheelData = WheelSetData.EmoteWheels[wheelIndex];
        
        customizeWheel.LoadEmoteData(wheelData);

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

    private void EmotesSwapped(int fromIndex, int toIndex)
    {
        var wheels = WheelSetData.EmoteWheels;
        var emotes = wheels[_wheelIndex].Emotes;
        
        (emotes[fromIndex], emotes[toIndex]) = (emotes[toIndex], emotes[fromIndex]);

        wheels[_wheelIndex].Emotes = emotes;
        var newData = new EmoteWheelSetData
        {
            EmoteWheels = wheels
        };
        EmoteUiManager.SaveEmoteWheelSetData(newData);
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