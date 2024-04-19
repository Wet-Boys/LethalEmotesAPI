using UnityEngine;

namespace LethalEmotesApi.Ui.Dmca;

[CreateAssetMenu(fileName = "EmoteDmcaVerificationStatusEntry", menuName = "LethalEmotesAPI/DMCA/Verification Entry")]
public class EmoteDmcaVerificationStatusEntry : ScriptableObject
{
    public string modGuid = "";
    public DmcaVerificationStatus verificationStatus;
}