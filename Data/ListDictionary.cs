using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Concordant.Data
{
    [Serializable]
    public class ListDictionaryEntry<K, V> where V : class
    {
        [SerializeField] public K Key;
        [SerializeField] public V Value;
        
        public ListDictionaryEntry(K key, V value)
        {
            Key = key;
            Value = value;
        }
    }
    
    [Serializable]
    public class ListDictionary<K, V> where V : class
    {
        [SerializeField] public List<ListDictionaryEntry<K, V>> InnerList = new();

        public void Add(K key, V value) => InnerList.Add(new(key, value));
        public ListDictionaryEntry<K, V> GetEntry(K key) => InnerList.Find(pair => pair.Key.Equals(key));
        public V Get(K key) => GetEntry(key)?.Value;
        public void Set(K key, V value)
        {
            var entry = GetEntry(key);
            if (entry != null) entry.Value = value;
        }
        public void Remove(K key) => InnerList.Remove(GetEntry(key));
        public bool Contains(K key) => Get(key) != null;
        public void Clear() => InnerList.Clear();

        public List<K> Keys => InnerList.Select(pair => pair.Key).ToList();
        public List<V> Values => InnerList.Select(pair => pair.Value).ToList();
        public int Count => InnerList.Count;
    }
}