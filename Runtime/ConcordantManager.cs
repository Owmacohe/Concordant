using System;
using Concordant.Data;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Concordant.Runtime
{
    /// <summary>
    /// A manager from which to get translations for terms in ConcordantDatabases
    /// </summary>
    public class ConcordantManager : MonoBehaviour
    {
        [SerializeField, Tooltip("The database in which the terms and their translations are stored")] ConcordantDatabase database;

        [Header("Fonts")]
        [SerializeField, Tooltip("The default font used by ConcordantTranslator Components")] TMP_FontAsset defaultFont;
        [SerializeField, Tooltip("Additional override fonts used by specific languages")] ListDictionary<SystemLanguage, TMP_FontAsset> languageFonts;

        SystemLanguage language; // The current language set by the game
        bool initialized; // Whether this manager has been initialized

        // A callback that gets triggered whether any ConcordantTranslators need to refresh their text
        // (e.g. when the language is changed)
        public Action OnRefresh;

        /// <summary>
        /// Initializes the manager with the player's system's language
        /// </summary>
        void Initialize()
        {
            language = Application.systemLanguage;
            initialized = true;
        }

        /// <summary>
        /// Sets the current game language
        /// </summary>
        /// <param name="language">The language to set</param>
        public void SetLanguage(SystemLanguage language)
        {
            this.language = database.Languages.Contains(language) ? language : database.Languages[0];
            OnRefresh?.Invoke();
        }
        
        /// <summary>
        /// Sets the current game language
        /// </summary>
        /// <param name="language">
        /// The language to set
        /// (do NOT use a language code; this string must be parse-able to the SystemLanguage enum)
        /// </param>
        public void SetLanguage(string language)
        {
            Enum.TryParse(language, true, out SystemLanguage result);
            SetLanguage(result);
        }

        /// <summary>
        /// Gets a translation for a term in the database
        /// </summary>
        /// <param name="key">The key for the term (in the format 'category/id')</param>
        /// <param name="font">The font to be used with the translation</param>
        /// <returns>The current translation for the given term</returns>
        public string GetTranslation(string key, out TMP_FontAsset font)
        {
            if (!initialized) Initialize();
            
            // We use the default font if the current language has no override font
            var temp = languageFonts.Get(language);
            font = temp == null ? defaultFont : temp;

            if (database == null)
            {
                Debug.LogError("Error: No ConcordantDatabase has been assigned to this ConcordantManager!");
                return null;
            }
            
            return database.Entries.Get(key).Translations.Get(language);
        }
    }
}