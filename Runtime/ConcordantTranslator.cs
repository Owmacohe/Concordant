using System;
using TMPro;
using UnityEngine;

namespace Concordant.Runtime
{
    public class ConcordantTranslator : MonoBehaviour
    {
        [SerializeField] string key;
        
        ConcordantManager manager;
        TMP_Text text;

        void Start()
        {
            manager = FindFirstObjectByType<ConcordantManager>();
            
            if (manager == null)
            {
                Debug.LogError("Error: No ConcordantManager found!");
                return;
            }

            text = GetComponent<TMP_Text>();
            SetText();

            manager.OnRefresh += SetText;
        }

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