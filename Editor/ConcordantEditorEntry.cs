#if UNITY_EDITOR

using System;
using UnityEngine;
using UnityEngine.UIElements;
using Concordant.Data;
using UnityEngine.Serialization;

namespace Concordant.Editor
{
    /// <summary>
    /// An override of the ConcordantEditorRow, this time representing an actual entry in the database
    /// </summary>
    class ConcordantEditorEntry : ConcordantEditorRow
    {
        ConcordantDatabaseEntry Entry; // The database entry that this row is representing

        bool expanded; // Whether the row is expanded to display the translation and context info

        TextElement Key; // The generated key for this entry

        /// <summary>
        /// A callback for when this entry's category is changed
        /// </summary>
        public Action OnChangeCategory;
        
        /// <summary>
        /// A callback for when this entry is removed from the database
        /// </summary>
        public Action OnRemoved;
        
        /// <summary>
        /// Refreshes the display of this entry's database key
        /// </summary>
        void RefreshKey() => Key.text = Key.text = $"Key: {Category.value}/{ID.value}";

        /// <summary>
        /// Parameterized constructor
        /// </summary>
        /// <param name="database">The current database that the ConcordantEditor is modifying</param>
        /// <param name="id">The ID of this entry</param>
        /// <param name="category">This entry's category</param>
        /// <param name="entry">The actual entry data</param>
        public ConcordantEditorEntry(
            ConcordantDatabase database,
            string id, string category, ConcordantDatabaseEntry entry)
            : base(database, "-")
        {
            Entry = entry;
            
            // Setting the values when we add it from the database
            ID.SetValueWithoutNotify(id);
            Category.SetValueWithoutNotify(category);
            Categories.SetValueWithoutNotify(category);

            Key = new();
            Key.AddToClassList("green");
            RefreshKey();
            TopRow.Add(Key);
            
            // A toggle to expand and contract the context and translations info for this entry
            Button toggle = new Button();
            toggle.text = "v";
            toggle.style.position = new StyleEnum<Position>(Position.Absolute);
            toggle.style.right = new StyleLength(0f);
            TopRow.Add(toggle);
            
            VisualElement translations = new();
            translations.AddToClassList("flex_column");
            translations.style.display = new StyleEnum<DisplayStyle>(StyleKeyword.None);
            Add(translations);
            
            TextField context = new TextField("Context:");
            context.multiline = true;
            context.style.marginBottom = new StyleLength(20);
            context.SetValueWithoutNotify(Entry.Context);
            translations.Add(context);

            // Setting the context in the data when it is changed
            context.RegisterValueChangedCallback(evt =>
            {
                Entry.Context = evt.newValue;
            });
            
            // Expanding and contracting the entry
            toggle.clicked += () =>
            {
                expanded = !expanded;
                translations.style.display = new StyleEnum<DisplayStyle>(expanded ? StyleKeyword.Auto : StyleKeyword.None);
                toggle.text = expanded ? "^" : "v";
            };

            // Adding the language fields and their loaded values
            foreach (var i in Database.Languages)
            {
                if (!Entry.Translations.Contains(i)) Entry.Translations.Add(i, "");
                
                TextField language = new TextField(i + ":");
                language.multiline = true;
                language.SetValueWithoutNotify(Entry.Translations.Get(i));
                translations.Add(language);
                
                language.RegisterValueChangedCallback(evt =>
                {
                    Entry.Translations.Set(i, evt.newValue);
                });
            }
        }
        
        internal override void IDCallback(ChangeEvent<string> evt)
        {
            base.IDCallback(evt);
            
            // Renaming the entry's ID (and therefore key)
            // Resetting it if the name is already contained in the database
            if (!Database.Rename(evt.previousValue, Category.value, ID.value, Category.value))
                ID.SetValueWithoutNotify(evt.previousValue);

            RefreshKey();
        }

        internal override void CategoryCallback(ChangeEvent<string> evt)
        {
            base.CategoryCallback(evt);

            // Renaming the entry's category (and therefore key)
            if (!Database.Rename(ID.value, evt.previousValue, ID.value, Category.value))
            {
                // Resetting it if the name is already contained in the database
                Category.SetValueWithoutNotify(evt.previousValue);
                Categories.SetValueWithoutNotify(evt.previousValue);
            }
            
            OnChangeCategory?.Invoke();

            RefreshKey();
        }

        internal override void ButtonClicked()
        {
            // Removing the entry from teh database
            Database.Remove(ID.value, Category.value);
            OnRemoved?.Invoke();
        }

        /// <summary>
        /// The remove button is always visible
        /// </summary>
        internal override void RefreshButtonVisibility()
        {
            // Intentionally left blank
        }
    }
}

#endif