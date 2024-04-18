using UnityEngine;

namespace LethalEmotesApi.Ui.Dmca;

public class DmcaPromptController : MonoBehaviour
{
    public void Close()
    {
        EmoteUiManager.EmoteUiInstance?.CloseGracefully();
    }
}