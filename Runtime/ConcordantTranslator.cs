using System;
using TMPro;
using UnityEngine;

namespace Concordant.Runtime
{
    /// <summary>
    /// A Component used to dynamically set the text of TMP_Text based on the game's current language
    /// </summary>
    public class ConcordantTranslator : MonoBehaviour
    {
        [SerializeField, Tooltip("The key to the entry in the current database, in the format 'category/id' " +
                                 "(e.g. PLAYERSTAT/HEALTH)")] string key;
        
        ConcordantManager manager; // The manager used to get translations
        TMP_Text text; // The TMP_Text Component on the GameObject

        void Start()
        {
            manager = FindFirstObjectByType<ConcordantManager>();
            
            if (manager == null)
            {
                Debug.LogError("Error: No ConcordantManager found! Please add one to the scene to use this ConcordantTranslator.");
                return;
            }

            text = GetComponent<TMP_Text>();
            SetText();

            manager.OnRefresh += SetText; // Ensures that the text is always updated when the manager refreshes
        }

        /// <summary>
        /// Sets the TMP_Text to the current translation for the term
        /// </summary>
        void SetText()
        {
            if (text != null)
            {
                text.text = manager.GetTranslation(key, out var font);
                text.font = font;
            }
            else Debug.LogError("Error: No TMP_Text found!");
        }
    }
}