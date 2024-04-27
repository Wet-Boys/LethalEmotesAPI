using System;
using System.Collections.Generic;
using System.Linq;

namespace LethalEmotesApi.Ui.EmoteHistory;

public class RecentEmote : IEquatable<RecentEmote>
{
    public readonly string EmoteKey;
    private readonly List<string> _playerNames;
    
    public string[] PlayerNames => _playerNames.AsEnumerable().Reverse().ToArray();
    
    public RecentEmote(string emoteKey, string playerName)
    {
        EmoteKey = emoteKey;
        _playerNames = [playerName];
    }
    
    public RecentEmote(string emoteKey, IEnumerable<string> playerNames)
    {
        EmoteKey = emoteKey;
        _playerNames = new List<string>(playerNames);
    }
    
    public bool AddPlayer(string playerName)
    {
        if (_playerNames.Contains(playerName))
            return false;
            
        _playerNames.Add(playerName);
        return true;
    }

    public bool Equals(RecentEmote? other)
    {
        if (ReferenceEquals(null, other))
            return false;
        if (ReferenceEquals(this, other))
            return true;
        return EmoteKey == other.EmoteKey && _playerNames.Equals(other._playerNames);
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
            _playerNames.Aggregate(19, (current, playerName) => current * 31 + playerName.GetHashCode());

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