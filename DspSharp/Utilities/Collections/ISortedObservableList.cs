using System;
using System.Collections.Generic;

namespace DspSharp.Utilities.Collections
{
    public interface ISortedObservableList<TKey, TValue> : IObservableList<TValue>
    {
        IComparer<TKey> Comparer { get; }
        Func<TValue, TKey> KeyFunction { get; }
        //void Remove(TValue item);
        //void Add(TValue item);
        //void AddRange(IEnumerable<TValue> items);
        //void RemoveRange(IEnumerable<TValue> items);
        //void Reset(IEnumerable<TValue> items);
        //void Clear();
    }
}