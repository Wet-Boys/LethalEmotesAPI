using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LethalEmotesApi.Ui.Customize.TabbedView;

public class EmoteListTab : Selectable, IPointerClickHandler
{
    public GameObject? activeBg;
    public GameObject? activeMask;
    public GameObject? activeOutline;
    public GameObject? activeView;
    
    public Sprite? activeBgOutline;
    
    public GameObject? inactiveBg;

    private bool _current;

    public bool Current
    {
        get => _current;
        set
        {
            activeBg?.SetActive(value);
            activeMask?.SetActive(value);
            activeOutline?.SetActive(value);
            activeView?.SetActive(value);
                
            inactiveBg?.SetActive(!value);

            _current = value;
        }
    }

    public UnityEvent? onClick;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;
        
        onClick?.Invoke();
    }
}