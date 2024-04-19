using System.Linq;
using TMPro;
using UnityEngine;

namespace LethalEmotesApi.Ui.Dmca;

public class DmcaVerificationPromptLabel : MonoBehaviour
{
    public TextMeshProUGUI? label;
    public EmoteDmcaVerificationStatusDb? verificationStatusDb;

    private void Awake()
    {
        if (verificationStatusDb is null)
            return;
        
        Debug.Log(EmoteUiManager.EmoteDb.EmoteModGuids);

        var data = verificationStatusDb.GetVerificationData(EmoteUiManager.EmoteDb.EmoteModGuids.ToArray());

        if (data.Any(item => !item.Status.IsVerified()))
        {
            SetNonCompliant(data);
        }
        else
        {
            SetCompliant();
        }
    }

    private void SetCompliant()
    {
        if (label is null)
            return;
        
        label.SetText("All installed emote packs are verified to comply with the DMCA options provided by LethalEmotesAPI");
    }

    private void SetNonCompliant(EmoteDmcaVerificationStatusDb.VerificationStatus[] data)
    {
        if (label is null)
            return;

        var failing = data.Count(item => !item.Status.IsVerified());
        
        label.SetText($"{failing} installed emote packs have not yet been verified to comply with the DMCA options provided by LethalEmotesAPI");
    }

    public void ShowVerificationDetails()
    {
        EmoteUiManager.emoteUiInstance?.ShowDmcaVerificationPrompt();
    }
}