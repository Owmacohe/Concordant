using System;
using Concordant.Data;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Concordant.Runtime
{
    public class ConcordantManager : MonoBehaviour
    {
        [SerializeField] ConcordantDatabase database;
        [SerializeField] string language;

        [Header("Fonts")]
        [SerializeField] TMP_FontAsset defaultFont;
        [SerializeField] ListDictionary<string, TMP_FontAsset> languageFonts;

        public Action OnRefresh;

        public void SetLanguage(string language)
        {
            this.language = language;
            OnRefresh?.Invoke();
        }

        public string GetTranslation(string key, out TMP_FontAsset font)
        {
            var temp = languageFonts.Get(language);
            font = temp == null ? defaultFont : temp;
            
            return database.Entries.Get(key).Translations.Get(language);
        }
    }
}