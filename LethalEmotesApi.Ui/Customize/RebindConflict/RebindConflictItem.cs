using TMPro;
using UnityEngine;

namespace LethalEmotesApi.Ui.Customize.RebindConflict;

public class RebindConflictItem : MonoBehaviour
{
    public TextMeshProUGUI? label;

    public void SetLabelText(string emoteName)
    {
        if (label is null)
            return;
        
        label.SetText(emoteName);
    }
}