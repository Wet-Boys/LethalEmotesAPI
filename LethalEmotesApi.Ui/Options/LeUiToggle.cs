using System;
using UnityEngine;
using UnityEngine.UI;

namespace LethalEmotesApi.Ui.Options;

public abstract class LeUiToggle : MonoBehaviour
{
    public Image? checkboxImage;

    private void Awake()
    {
        UpdateCheckbox();
    }

    private void Start()
    {
        UpdateCheckbox();
    }

    private void OnEnable()
    {
        UpdateCheckbox();
    }

    public void Toggle()
    {
        SetCurrentValue(!GetCurrentValue());
        UpdateCheckbox();
    }

    private void UpdateCheckbox()
    {
        if (checkboxImage is null)
            return;

        checkboxImage.enabled = GetCurrentValue();
    }

    protected abstract bool GetCurrentValue();

    protected abstract void SetCurrentValue(bool value);
}