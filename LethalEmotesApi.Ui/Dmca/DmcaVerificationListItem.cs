using LethalEmotesApi.Ui.Elements.Recycle;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace LethalEmotesApi.Ui.Dmca;

[RequireComponent(typeof(RectTransform))]
public class DmcaVerificationListItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IRecycleViewItem<EmoteDmcaVerificationStatusDb.VerificationStatus>
{
    public TextMeshProUGUI? modLabel;
    public TextMeshProUGUI? verificationLabel;

    public GameObject? tooltip;
    public TextMeshProUGUI? tooltipLabel;
    
    private RectTransform? _rectTransform;
    
    public int ConstraintIndex { get; set; }

    public RectTransform RectTransform => _rectTransform!;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();

        if (tooltip is null)
            return;
        
        tooltip.SetActive(false);
    }

    public void BindData(EmoteDmcaVerificationStatusDb.VerificationStatus data)
    {
        if (modLabel is null || verificationLabel is null || tooltipLabel is null)
            return;

        var colorTag = data.Status.AsColorTag();
        var status = data.Status.Name();
        
        if (data.Status == DmcaVerificationStatus.NonDmcaCompliant)
        {
            colorTag = DmcaVerificationStatus.Verified.AsColorTag();
            status = $"{DmcaVerificationStatus.Verified.Name()}*";
        }
        
        modLabel.text = data.ModName;
        verificationLabel.text = $"Status: {colorTag}{status}</color>";
        tooltipLabel.text = data.Status.Tooltip();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (tooltip is null)
            return;
        
        tooltip.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (tooltip is null)
            return;
        
        tooltip.SetActive(false);
        
    }
}