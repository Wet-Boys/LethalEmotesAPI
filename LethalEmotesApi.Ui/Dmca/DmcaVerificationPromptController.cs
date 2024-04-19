using UnityEngine;

namespace LethalEmotesApi.Ui.Dmca;

public class DmcaVerificationPromptController : MonoBehaviour
{
    public void Close()
    {
        EmoteUiManager.emoteUiInstance?.CloseGracefully();
    }
}