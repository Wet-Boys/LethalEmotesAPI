using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LethalEmotesApi.Ui.Data;

public class SearchableEmoteArray : IReadOnlyList<string>
{
    private readonly string[] _emoteKeys;
    private CacheState _state;
    private int[] _lut = null!;

    private string[] _cachedKeys = [];

    public SearchableEmoteArray(string[] emoteKeys)
    {
        _emoteKeys = emoteKeys;
        _state = new CacheState(SortOrder.Descending, "");
        
        UpdateCachedKeys();
    }
    
    private int[] CreateLut(IEnumerable<string> emoteKeys)
    {
        return emoteKeys
            .Select((key, index) => new KeyValuePair<string, int>(key, index))
            .OrderBy(kvp => EmoteUiManager.GetEmoteName(kvp.Key))
            .Where(kvp => EmoteUiManager.GetEmoteVisibility(kvp.Key))
            .Where(kvp => MatchesFilter(kvp.Key, Filter))
            .Select(kvp => kvp.Value)
            .ToArray();
    }

    private void UpdateCachedKeys()
    {
        _lut = CreateLut(_emoteKeys);

        _cachedKeys = new string[_lut.Length];
        for (int i = 0; i < _lut.Length; i++)
            _cachedKeys[i] = _emoteKeys[_lut[i]];
    }

    public IEnumerator<string> GetEnumerator()
    {
        return ((IEnumerable<string>)_cachedKeys).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public int Count => _cachedKeys.Length;

    public string this[int index] => _cachedKeys[index];

    public SortOrder Order
    {
        get => _state.Order;
        set
        {
            var oldState = _state;
            _state = new CacheState(value, _state.Filter);
            if (_state != oldState)
                UpdateCachedKeys();
        }
    }

    public string Filter
    {
        get => _state.Filter;
        set
        {
            var oldState = _state;
            _state = new CacheState(_state.Order, value);
            if (_state != oldState)
                UpdateCachedKeys();
        }
    }

    private static bool MatchesFilter(string emoteKey, string filter)
    {
        if (string.IsNullOrEmpty(filter))
            return true;

        if (filter.StartsWith('@'))
        {
            var emoteModName = EmoteUiManager.GetEmoteModName(emoteKey);
            var modFilter = filter[1..];

            return emoteModName.Contains(modFilter, StringComparison.InvariantCultureIgnoreCase);
        }

        var emoteName = EmoteUiManager.GetEmoteName(emoteKey);

        return emoteKey.Contains(filter, StringComparison.InvariantCultureIgnoreCase) ||
               emoteName.Contains(filter, StringComparison.InvariantCultureIgnoreCase);
    }

    public enum SortOrder
    {
        Descending,
        Ascending
    }
    
    private readonly struct CacheState(SortOrder order, string filter) : IEquatable<CacheState>
    {
        public readonly SortOrder Order = order;
        public readonly string Filter = filter;

        public bool Equals(CacheState other)
        {
            return Order == other.Order && Filter == other.Filter;
        }

        public override bool Equals(object? obj)
        {
            return obj is CacheState other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine((int)Order, Filter);
        }

        public static bool operator ==(CacheState left, CacheState right) => left.Equals(right);
        public static bool operator !=(CacheState left, CacheState right) => !(left == right);
    }
}