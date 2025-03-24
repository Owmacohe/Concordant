#if UNITY_EDITOR

using System;
using UnityEngine;
using UnityEngine.UIElements;
using Concordant.Data;

namespace Concordant.Editor
{
    class ConcordantEditorEntry : ConcordantEditorRow
    {
        ConcordantDatabaseEntry Entry;

        bool clicked;

        TextElement Key;

        public Action OnClicked;
        public Action OnChangeCategory;
        public Action OnRemoved;

        public ConcordantEditorEntry(
            ConcordantDatabase database,
            string id, string category, ConcordantDatabaseEntry entry)
            : base(database, true, "-")
        {
            Entry = entry;
            
            ID.SetValueWithoutNotify(id);
            Category.SetValueWithoutNotify(category);
            Categories.SetValueWithoutNotify(category);

            Key = new();
            RefreshKey();
            TopRow.Add(Key);
            
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

            context.RegisterValueChangedCallback(evt =>
            {
                Entry.Context = evt.newValue;
            });
            
            toggle.clicked += () =>
            {
                clicked = !clicked;
                translations.style.display = new StyleEnum<DisplayStyle>(clicked ? StyleKeyword.Auto : StyleKeyword.None);
                toggle.text = clicked ? "^" : "v";
                
                OnClicked?.Invoke();
            };

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

        void RefreshKey() => Key.text = Key.text = $"Key: {Category.value}/{ID.value}";

        internal override void IDCallback(ChangeEvent<string> evt)
        {
            base.IDCallback(evt);
            
            if (!Database.Rename(evt.previousValue, Category.value, ID.value, Category.value))
                ID.SetValueWithoutNotify(evt.previousValue);

            RefreshKey();
        }

        internal override void CategoryCallback(ChangeEvent<string> evt)
        {
            base.CategoryCallback(evt);

            if (!Database.Rename(ID.value, evt.previousValue, ID.value, Category.value))
            {
                Category.SetValueWithoutNotify(evt.previousValue);
                Categories.SetValueWithoutNotify(evt.previousValue);
            }
            
            OnChangeCategory?.Invoke();

            RefreshKey();
        }

        internal override void ButtonClicked()
        {
            Database.Remove(ID.value, Category.value);
            OnRemoved?.Invoke();
        }

        internal override void RefreshButton()
        {
            
        }
    }
}

#endif