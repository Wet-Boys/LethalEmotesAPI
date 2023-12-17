using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LethalEmotesApi.Ui.Elements;

public class LeUiButton : UIBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Graphic? targetGraphic;
    public ColorBlock colors;
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        throw new System.NotImplementedException();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        throw new System.NotImplementedException();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        throw new System.NotImplementedException();
    }
}