using LethalEmotesApi.Ui.Elements.Recycle;
using TMPro;
using UnityEngine;

namespace LethalEmotesApi.Ui.Dmca;

[RequireComponent(typeof(RectTransform))]
public class DmcaVerificationListItem : MonoBehaviour, IRecycleViewItem<EmoteDmcaVerificationStatusDb.VerificationStatus>
{
    public TextMeshProUGUI? modLabel;
    public TextMeshProUGUI? verificationLabel;
    
    private RectTransform? _rectTransform;
    
    public int ConstraintIndex { get; set; }

    public RectTransform RectTransform => _rectTransform!;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    public void BindData(EmoteDmcaVerificationStatusDb.VerificationStatus data)
    {
        if (modLabel is null || verificationLabel is null)
            return;

        modLabel.text = data.ModName;
        verificationLabel.text = $"Status: {data.Status.AsColorTag()}{data.Status.Name()}</color>";
    }
}