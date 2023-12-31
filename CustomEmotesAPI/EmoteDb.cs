using System.Collections.Generic;
using System.Linq;
using BepInEx;
using LethalEmotesApi.Ui.Db;

namespace LethalEmotesAPI;

public class EmoteDb : IEmoteDb
{
    private IReadOnlyCollection<string> _emoteKeys;

    public IReadOnlyCollection<string> EmoteKeys
    {
        get
        {
            _emoteKeys ??= BoneMapper.animClips
                .Where(kvp => kvp.Value is null || kvp.Value.visibility)
                .Select(kvp => kvp.Key)
                .ToArray();
            
            return _emoteKeys;
        }
    }

    public string GetEmoteName(string emoteKey)
    {
        if (!BoneMapper.animClips.ContainsKey(emoteKey))
            return emoteKey;
        
        var clip = BoneMapper.animClips[emoteKey];
        return clip is null || string.IsNullOrEmpty(clip.customName) ? emoteKey : clip.customName;
    }
    
    private IReadOnlyCollection<string> _emoteModNames;

    public IReadOnlyCollection<string> EmoteModNames
    {
        get
        {
            _emoteModNames ??= BoneMapper.animClips
                .Where(kvp => kvp.Value is not null)
                .Select(kvp => kvp.Value.ownerPlugin.Name)
                .GroupBy(modName => modName)
                .Select(kvp => kvp.First())
                .ToList();

            return _emoteModNames;
        }
    }

    private readonly Dictionary<string, string> _emoteKeyModNameLut = new();

    public void AssociateEmoteKeyWithMod(string emoteKey, string modName) => _emoteKeyModNameLut[emoteKey] = modName;

    public string GetModName(string emoteKey)
    {
        if (BoneMapper.animClips.TryGetValue(emoteKey, out var clip))
        {
            if (clip is null)
                return _emoteKeyModNameLut.GetValueOrDefault(emoteKey, "Unknown");

            var ownerPlugin = clip.ownerPlugin;
            return ownerPlugin is null ? "Unknown" : ownerPlugin.Name;
        }
        
        return _emoteKeyModNameLut.GetValueOrDefault(emoteKey, "Unknown");
    }
}