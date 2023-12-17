using UnityEngine;

namespace LethalEmotesApi.Ui;

public class EmoteUiPanel : MonoBehaviour
{
    public EmoteWheelsController? emoteWheelsController;
    
    private enum UiView
    {
        EmoteWheels,
        Customize,
    }
}