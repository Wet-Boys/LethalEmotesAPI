using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LethalEmotesApi.Ui.Elements;

public class LeUiSelectOutline : UIBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image? selectImage;
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (selectImage is null)
            return;
        
        selectImage.enabled = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (selectImage is null)
            return;
        
        selectImage.enabled = false;
    }
}