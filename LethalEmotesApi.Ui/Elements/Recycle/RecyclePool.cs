using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LethalEmotesApi.Ui.Elements.Recycle;

internal class RecyclePool<TItem, TData> : IEnumerable<TItem>
    where TItem : Component, IRecycleViewItem<TData>
{
    private readonly List<TItem> _items = [];
    private int _firstIndex;
    private int _lastIndex;
    
    public int FirstIndex { get; private set; }
    public int LastIndex { get; private set; }

    public int Size => _items.Count;

    public void Add(TItem item)
    {
        _items.Add(item);

        _lastIndex = _items.Count - 1;
        LastIndex = _lastIndex;
    }
    
    public TItem RecycleForwards()
    {
        _lastIndex = _firstIndex;
        _firstIndex = (_firstIndex + 1) % _items.Count;

        FirstIndex++;
        LastIndex++;

        return _items[_lastIndex];
    }

    public TItem RecycleBackwards()
    {
        _firstIndex = _lastIndex;
        _lastIndex = (_lastIndex - 1 + _items.Count) % _items.Count;

        FirstIndex--;
        LastIndex--;

        return _items[_firstIndex];
    }

    public TItem GetFirst() => _items[_firstIndex];

    public TItem GetLast() => _items[_lastIndex];

    public void Clear()
    {
        foreach (var item in _items)
            Object.DestroyImmediate(item.gameObject);
        
        _items.Clear();

        _firstIndex = 0;
        _lastIndex = 0;
        FirstIndex = _firstIndex;
        LastIndex = _lastIndex;
    }
    
    public IEnumerator<TItem> GetEnumerator() => new RecyclePoolEnumerator(_items.ToArray(), _firstIndex, _lastIndex);

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    private class RecyclePoolEnumerator : IEnumerator<TItem>
    {
        private readonly TItem[] _items;
        private readonly int _firstIndex;
        private readonly int _lastIndex; // Todo: look into why this was needed in my old implementation.
        private int _recyclePos;
        private int _arrayPos;

        public RecyclePoolEnumerator(TItem[] items, int firstIndex, int lastIndex)
        {
            _items = items;
            _firstIndex = firstIndex;
            _lastIndex = lastIndex;

            _recyclePos = firstIndex - 1;
            _arrayPos = -1;
        }

        public bool MoveNext()
        {
            _arrayPos++;
            _recyclePos++;

            if (_recyclePos >= _items.Length)
                _recyclePos = 0;

            return _arrayPos < _items.Length;
        }

        public void Reset()
        {
            _recyclePos = _firstIndex;
            _arrayPos = -1;
        }

        public TItem Current => _items[_recyclePos];

        object IEnumerator.Current => Current;

        public void Dispose() { }
    }
}