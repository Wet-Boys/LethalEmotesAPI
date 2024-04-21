using System.Collections.Generic;
using System.Linq;
using BepInEx.Bootstrap;
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
                //.Where(kvp => kvp.Value is null || kvp.Value.visibility)
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

        if (clip is not null && clip.usesNewImportSystem)
        {
            return string.IsNullOrEmpty(clip.displayName) ? emoteKey : clip.displayName;
        }

        return clip is null || string.IsNullOrEmpty(clip.customInternalName) ? emoteKey : clip.customInternalName; //this is just the old return
    }

    private IReadOnlyCollection<string> _emoteModNames;

    public IReadOnlyCollection<string> EmoteModNames
    {
        get
        {
            // Todo: after some investigation, it looks like this is incorrectly implemented, need to verify that first - Rune
            _emoteModNames ??= BoneMapper.animClips
                .Where(kvp => kvp.Value is not null)
                .Select(kvp => kvp.Value.ownerPlugin.Name)
                .GroupBy(modName => modName)
                .Select(kvp => kvp.First())
                .ToArray();

            return _emoteModNames;
        }
    }
    
    private IReadOnlyCollection<string> _emoteModGuids;

    public IReadOnlyCollection<string> EmoteModGuids
    {
        get
        {
            _emoteModGuids ??= BoneMapper.animClips
                .Where(kvp => kvp.Value is not null)
                .Select(kvp => kvp.Value.ownerPlugin.GUID)
                .Distinct()
                .ToArray();

            return _emoteModGuids;
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

    public string GetModNameFromModGuid(string modGuid)
    {
        if (!Chainloader.PluginInfos.TryGetValue(modGuid, out var pluginInfo))
            return modGuid;

        return pluginInfo.Metadata.Name;
    }

    public bool GetEmoteVisibility(string emoteKey)
    {
        if (BoneMapper.animClips.TryGetValue(emoteKey, out var clip))
        {
            return clip is null || clip.visibility;
        }

        return false;
    }
}