using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Concordant.Data
{
    /// <summary>
    /// A serializable List-based Dictionary class
    /// </summary>
    /// <typeparam name="K">Some key type used to reference entries in the dictionary</typeparam>
    /// <typeparam name="V">Some value type that the keys reference to (must be a class of some type)</typeparam>
    [Serializable]
    public class ListDictionary<K, V> where V : class
    {
        [SerializeField, Tooltip("The inner list of entries within the dictionary")] public List<ListDictionaryEntry<K, V>> InnerList = new();

        /// <summary>
        /// Adds a key/value pair to the dictionary
        /// </summary>
        /// <param name="key">The entry's key</param>
        /// <param name="value">The entry's value</param>
        public void Add(K key, V value) => InnerList.Add(new(key, value));
        
        /// <summary>
        /// Gets a full key/value pair entry
        /// </summary>
        /// <param name="key">The entry's key</param>
        public ListDictionaryEntry<K, V> GetEntry(K key) => InnerList.Find(pair => pair.Key.Equals(key));
        
        /// <summary>
        /// Gets a key/value pair's value
        /// </summary>
        /// <param name="key">The entry's key</param>
        public V Get(K key) => GetEntry(key)?.Value;
        
        /// <summary>
        /// Sets a key/value pair's value
        /// </summary>
        /// <param name="key">The entry's key</param>
        /// <param name="value">The entry's new value</param>
        public void Set(K key, V value)
        {
            var entry = GetEntry(key);
            if (entry != null) entry.Value = value;
        }
        
        /// <summary>
        /// Removes a key/value pair from the dictionary
        /// </summary>
        /// <param name="key">The entry's key</param>
        public void Remove(K key) => InnerList.Remove(GetEntry(key));
        
        /// <summary>
        /// Whether the dictionary contains a key/value pair
        /// </summary>
        /// <param name="key">The entry's key</param>
        public bool Contains(K key) => Get(key) != null;
        
        /// <summary>
        /// Clears the entire dictionary of key/value pairs
        /// </summary>
        public void Clear() => InnerList.Clear();

        /// <summary>
        /// A full list of all keys in the dictionary
        /// </summary>
        public List<K> Keys => InnerList.Select(pair => pair.Key).ToList();
        
        /// <summary>
        /// A full list of all values in the dictionary
        /// </summary>
        public List<V> Values => InnerList.Select(pair => pair.Value).ToList();
        
        /// <summary>
        /// The total number of key/value pairs in the dictionary
        /// </summary>
        public int Count => InnerList.Count;
    }
    
    /// <summary>
    /// An entry in a ListDictionary
    /// </summary>
    /// <typeparam name="K">Some key type used to reference the entry in the dictionary</typeparam>
    /// <typeparam name="V">Some value type that the key references (must be a class of some type)</typeparam>
    [Serializable]
    public class ListDictionaryEntry<K, V> where V : class
    {
        [SerializeField, Tooltip("The entry's key")] public K Key;
        [SerializeField, Tooltip("The entry's value")] public V Value;
        
        public ListDictionaryEntry(K key, V value)
        {
            Key = key;
            Value = value;
        }
    }
}