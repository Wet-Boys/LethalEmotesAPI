using System;
using System.Collections.Generic;
using System.Linq;

namespace LethalEmotesApi.Ui.EmoteHistory;

public class RecentEmote : IEquatable<RecentEmote>
{
    public readonly string EmoteKey;
    public readonly List<string> PlayerNames;
    
    public RecentEmote(string emoteKey, string playerName)
    {
        EmoteKey = emoteKey;
        PlayerNames = [playerName];
    }
    
    public RecentEmote(string emoteKey, List<string> playerNames)
    {
        EmoteKey = emoteKey;
        PlayerNames = playerNames;
    }
    
    public bool AddPlayer(string playerName)
    {
        if (PlayerNames.Contains(playerName))
            return false;
            
        PlayerNames.Add(playerName);
        return true;
    }

    public bool Equals(RecentEmote? other)
    {
        if (ReferenceEquals(null, other))
            return false;
        if (ReferenceEquals(this, other))
            return true;
        return EmoteKey == other.EmoteKey && PlayerNames.Equals(other.PlayerNames);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj))
            return false;
        if (ReferenceEquals(this, obj))
            return true;
        if (obj.GetType() != GetType())
            return false;
        return Equals((RecentEmote)obj);
    }

    public override int GetHashCode()
    {
        var playerNamesHash =
            PlayerNames.Aggregate(19, (current, playerName) => current * 31 + playerName.GetHashCode());

        return HashCode.Combine(EmoteKey, playerNamesHash);
    }

    public static bool operator ==(RecentEmote? left, RecentEmote? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(RecentEmote? left, RecentEmote? right)
    {
        return !Equals(left, right);
    }
}