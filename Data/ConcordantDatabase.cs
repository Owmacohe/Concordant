using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text;

namespace Concordant.Data
{
    [Serializable, CreateAssetMenu]
    public class ConcordantDatabase : ScriptableObject
    {
        [SerializeField] public List<string> Languages = new();
        [SerializeField] public ListDictionary<string, ConcordantDatabaseEntry> Entries = new();

        public void Import(string csv)
        {
            Languages.Clear();
            Entries.Clear();
            
            var lines = csv.Split('\n');

            var header = lines[0].Split(',');

            for (int i = 3; i < header.Length; i++)
                Languages.Add(header[i].Trim());

            for (int j = 1; j < lines.Length; j++)
            {
                if (string.IsNullOrEmpty(lines[j])) continue;
                
                var split = lines[j].Split(',');
                
                var entry = new ConcordantDatabaseEntry();
                entry.Context = split[2].Trim();
                
                for (int k = 3; k < split.Length; k++)
                    entry.Translations.Add(Languages[k-3].Trim(), split[k].Trim());
                
                Entries.Add(split[0].Trim() + "/" + split[1].Trim(), entry);
            }
        }
        
        public ConcordantDatabaseEntry Add(string id, string category)
        {
            var entry = new ConcordantDatabaseEntry();
            Entries.Add(category + "/" + id, entry);

            return entry;
        }

        public ConcordantDatabaseEntry Remove(string id, string category)
        {
            string key = category + "/" + id;

            var entry = Entries.Get(key);
            Entries.Remove(key);

            return entry;
        }

        public bool Rename(string previousID, string previousCategory, string newID, string newCategory)
        {
            string previousKey = previousCategory + "/" + previousID;
            string newKey = newCategory + "/" + newID;
            
            if (previousKey == newKey || Entries.Contains(newKey)) return false;

            var entry = Remove(previousID, previousCategory);
            Entries.Add(newKey, entry);

            return true;
        }

        public List<string> GetCategories() => Entries.InnerList
            .Select(pair => pair.Key.Split('/')[0])
            .OrderBy(key => key)
            .ToList();

        public bool Contains(string id, string category) => Entries.Contains(category + "/" + id);

        public List<string> Search(string id) => Entries.InnerList
            .Where(pair => pair.Key.ToLower().Contains(id.Trim().ToLower()))
            .Select(pair => pair.Key)
            .OrderBy(key => key)
            .ToList();

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder("Category,ID,Context");

            foreach (var i in Languages)
                builder.Append("," + i);

            foreach (var j in Entries.InnerList)
            {
                var split = j.Key.Split('/');
                
                builder.Append($"\n{split[0]},{split[1]},{j.Value.Context}");

                foreach (var k in Languages)
                    builder.Append("," + j.Value.Translations.Get(k));
            }

            return builder.ToString();
        }
    }

    [Serializable]
    public class ConcordantDatabaseEntry
    {
        [SerializeField] public ListDictionary<string, string> Translations = new();
        [SerializeField] public string Context = "";
    }
}