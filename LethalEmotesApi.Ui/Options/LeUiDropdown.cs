using System;
using TMPro;
using UnityEngine;

namespace LethalEmotesApi.Ui.Options;

public abstract class LeUiDropdown : MonoBehaviour
{
    public TMP_Dropdown? dropdown;

    private bool _hasListener;

    private void Awake()
    {
        UpdateDropdown();
        EnsureListener();
    }

    private void Start()
    {
        UpdateDropdown();
        EnsureListener();
    }

    private void OnEnable()
    {
        UpdateDropdown();
        EnsureListener();
    }

    private void UpdateDropdown()
    {
        if (dropdown is null)
            return;

        dropdown.value = GetCurrentValue();
    }
    
    private void EnsureListener()
    {
        if (dropdown is null || _hasListener)
            return;
        
        dropdown.onValueChanged.AddListener(DropdownChanged);
        _hasListener = true;
    }

    private void DropdownChanged(int value)
    {
        SetCurrentValue(value);
        SetValueWithoutNotify(value);
    }

    protected abstract int GetCurrentValue();

    protected abstract void SetCurrentValue(int value);

    private void SetValueWithoutNotify(int value)
    {
        if (dropdown is null)
            return;
        
        dropdown.SetValueWithoutNotify(value);
    }
}