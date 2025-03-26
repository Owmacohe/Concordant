using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text;

namespace Concordant.Data
{
    /// <summary>
    /// The ScriptableObject database which contains all of the translations
    /// </summary>
    [Serializable, CreateAssetMenu]
    public class ConcordantDatabase : ScriptableObject
    {
        [SerializeField, Tooltip("The languages that your game will be localized into")] public List<SystemLanguage> Languages = new();
        [SerializeField, Tooltip("The dictionary of terms that your game will pull from")] public ListDictionary<string, ConcordantDatabaseEntry> Entries = new();
        
        /// <summary>
        /// Method used to import CSV-formatted text, replacing this database with loaded data
        /// </summary>
        /// <param name="csv">the CSV-formatted text</param>
        public void Import(string csv)
        {
            Languages.Clear();
            Entries.Clear();
            
            var lines = csv.Split('\n'); // Splitting the text into lines
            var header = lines[0].Split(','); // Getting the first line (the header)

            // Parsing and adding the languages
            for (int i = 3; i < header.Length; i++)
            {
                Enum.TryParse(header[i].Trim(), true, out SystemLanguage result);
                Languages.Add(result);
            }

            // Going through each line, and adding it as an entry in the database
            for (int j = 1; j < lines.Length; j++)
            {
                if (string.IsNullOrEmpty(lines[j])) continue; // Skip over empty lines
                
                var split = lines[j].Split(',');
                
                var entry = new ConcordantDatabaseEntry();
                entry.Context = split[2].Trim();
                
                for (int k = 3; k < split.Length; k++)
                    entry.Translations.Add(Languages[k-3], split[k].Trim());
                
                Entries.Add(split[0].Trim() + "/" + split[1].Trim(), entry);
            }
        }
        
        #region Entry Manipulation
        
        /// <summary>
        /// Adds a new term entry to the database
        /// </summary>
        /// <param name="id">The entry's ID</param>
        /// <param name="category">The entry's category</param>
        /// <returns>The entry that was added</returns>
        public ConcordantDatabaseEntry Add(string id, string category)
        {
            var key = category + "/" + id;

            if (Entries.Contains(key))
            {
                Debug.LogError($"Error: The database already contains an entry with the key: {key}");
                return null;
            }
            
            var entry = new ConcordantDatabaseEntry();
            Entries.Add(key, entry);

            return entry;
        }

        /// <summary>
        /// Removes a term entry from the database
        /// </summary>
        /// <param name="id">The entry's ID</param>
        /// <param name="category">The entry's category</param>
        /// <returns>The entry that was removed</returns>
        public ConcordantDatabaseEntry Remove(string id, string category)
        {
            string key = category + "/" + id;

            if (!Entries.Contains(key))
            {
                Debug.LogError($"Error: The database does not contain an entry with the key: {key}");
                return null;
            }

            var entry = Entries.Get(key);
            Entries.Remove(key);

            return entry;
        }

        /// <summary>
        /// Renames an entry in the database with a new ID and category
        /// </summary>
        /// <param name="previousID">The entry's previous ID</param>
        /// <param name="previousCategory">The entry's previous category</param>
        /// <param name="newID">The entry's new ID</param>
        /// <param name="newCategory">The entry's new category</param>
        /// <returns></returns>
        public bool Rename(string previousID, string previousCategory, string newID, string newCategory)
        {
            string previousKey = previousCategory + "/" + previousID;
            string newKey = newCategory + "/" + newID;
            
            // Stop the rename if the new key is the same as the old, or if the database already contains the new key
            if (previousKey == newKey || Entries.Contains(newKey)) return false;

            var entry = Remove(previousID, previousCategory);
            Entries.Add(newKey, entry);

            return true;
        }
        
        #endregion
        
        #region Searching

        /// <summary>
        /// Method to get a list of all entries' unique categories
        /// </summary>
        /// <returns>All unique categories in the database</returns>
        public List<string> GetCategories() => Entries.Keys
            .Select(keys => keys.Split('/')[0])
            .Distinct()
            .OrderBy(category => category)
            .ToList();

        /// <summary>
        /// Method to get whether an entry exists in the database
        /// </summary>
        /// <param name="id">The entry's ID</param>
        /// <param name="category">The entry's category</param>
        /// <returns>Whether the database contains the entry</returns>
        public bool Contains(string id, string category) => Entries.Contains(category + "/" + id);

        /// <summary>
        /// Method to get a list of all entries in the database whose ID or category contain some query
        /// </summary>
        /// <param name="query">The query string</param>
        /// <returns>The keys of all the entries that match the query</returns>
        public List<string> Search(string query) => Entries.InnerList
            .Where(pair => pair.Key.ToLower().Contains(query.Trim().ToLower()))
            .Select(pair => pair.Key)
            .OrderBy(key => key)
            .ToList();
        
        #endregion

        /// <summary>
        /// Gets the entire database data as CSV-formatted text, ready to be exported as a file
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder("Category,ID,Context");

            // Appending the languages to the header line
            foreach (var i in Languages)
                builder.Append("," + i);

            // Adding the entries one by one
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

    /// <summary>
    /// A term entry within a ConcordantDatabase, containing context/usage information and translations
    /// </summary>
    [Serializable]
    public class ConcordantDatabaseEntry
    {
        [SerializeField, Tooltip("The dictionary of translations for the term")] public ListDictionary<SystemLanguage, string> Translations = new();
        [SerializeField, Tooltip("The context in which the term appears, or its formatting/usage information")] public string Context = "";
    }
}