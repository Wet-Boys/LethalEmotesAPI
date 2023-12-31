using System.Collections.Generic;

namespace LethalEmotesApi.Ui.Db;

public interface IEmoteDb
{
    public IReadOnlyCollection<string> EmoteKeys { get; }

    public string GetEmoteName(string emoteKey);
    
    public IReadOnlyCollection<string> EmoteModNames { get; }

    public void AssociateEmoteKeyWithMod(string emoteKey, string modName);

    public string GetModName(string emoteKey);
}