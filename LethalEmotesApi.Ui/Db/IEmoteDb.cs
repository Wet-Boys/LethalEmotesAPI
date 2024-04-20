using System.Collections.Generic;
using System.Linq;

namespace LethalEmotesApi.Ui.Db;

public interface IEmoteDb
{
    public IReadOnlyCollection<string> EmoteKeys { get; }

    public string GetEmoteName(string emoteKey);
    
    public IReadOnlyCollection<string> EmoteModNames { get; }
    
    public IReadOnlyCollection<string> EmoteModGuids { get; }

    public void AssociateEmoteKeyWithMod(string emoteKey, string modName);

    public string GetModName(string emoteKey);

    public string GetModNameFromModGuid(string modGuid);

    public bool EmoteExists(string emoteKey) => EmoteKeys.Contains(emoteKey);
    
    public bool GetEmoteVisibility(string emoteKey);
}