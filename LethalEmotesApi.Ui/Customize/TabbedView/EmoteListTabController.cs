using UnityEngine;
using UnityEngine.UI;

namespace LethalEmotesApi.Ui.Customize.TabbedView;

public class EmoteListTabController : MonoBehaviour
{
    public EmoteListTab? allEmotesTab;
    public EmoteListTab? recentEmotesTab;

    public Image? outlineGraphic;

    private void Awake()
    {
        SwitchToAllEmotesTab();
    }

    private void OnEnable()
    {
        SwitchToAllEmotesTab();
    }

    public void SwitchToAllEmotesTab()
    {
        if (allEmotesTab is null || recentEmotesTab is null || outlineGraphic is null)
            return;

        allEmotesTab.Current = true;
        recentEmotesTab.Current = false;

        outlineGraphic.sprite = allEmotesTab.activeBgOutline;
    }

    public void SwitchToRecentEmotesTab()
    {
        if (allEmotesTab is null || recentEmotesTab is null || outlineGraphic is null)
            return;

        recentEmotesTab.Current = true;
        allEmotesTab.Current = false;
        
        outlineGraphic.sprite = recentEmotesTab.activeBgOutline;
    }
}