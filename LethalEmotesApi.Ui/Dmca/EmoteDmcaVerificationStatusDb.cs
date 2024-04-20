using System.Collections.Generic;
using UnityEngine;

namespace LethalEmotesApi.Ui.Dmca;

[CreateAssetMenu(fileName = "EmoteDmcaVerificationStatusDb", menuName = "LethalEmotesAPI/DMCA/Verification Db")]
public class EmoteDmcaVerificationStatusDb : ScriptableObject
{
    private static EmoteDmcaVerificationStatusDb? _instance;
    
    public static EmoteDmcaVerificationStatusDb Instance {
        get
        {
            _instance ??= EmoteUiManager.GetStateController()!
                .LoadAsset<EmoteDmcaVerificationStatusDb>("assets/LethalEmotesApi-Ui/data/EmoteDmcaVerificationStatusDb.asset");

            return _instance;
        }
    }
    
    public EmoteDmcaVerificationStatusEntry[] entries = [];

    private readonly Dictionary<string, EmoteDmcaVerificationStatusEntry> _lut = new();

    private void Awake()
    {
        _instance ??= this;

        foreach (var entry in entries)
            _lut[entry.modGuid] = entry;
    }

    public VerificationStatus[] GetVerificationData(string[] modGuids)
    {
        var verificationStatusList = new List<VerificationStatus>();

        for (var i = 0; i < modGuids.Length; i++)
        {
            var modGuid = modGuids[i];
            
            var modName = EmoteUiManager.EmoteDb.GetModNameFromModGuid(modGuid);
            var status = DmcaVerificationStatus.Unknown;

            if (_lut.TryGetValue(modGuid, out var entry))
                status = entry.verificationStatus;

            if (status == DmcaVerificationStatus.Ignore)
                continue;

            verificationStatusList.Add(new VerificationStatus(modName, status));
        }

        return verificationStatusList.ToArray();
    }

    public static bool IsNonDmcaCompliant(string modGuid)
    {
        var data = Instance.GetVerificationData([modGuid]);

        return data.Length > 0 && data[0].Status == DmcaVerificationStatus.NonDmcaCompliant;
    }
    
    public readonly struct VerificationStatus(string modName, DmcaVerificationStatus status)
    {
        public readonly string ModName = modName;
        public readonly DmcaVerificationStatus Status = status;
    }
}