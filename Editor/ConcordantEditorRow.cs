#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;
using Concordant.Data;

namespace Concordant.Editor
{
    class ConcordantEditorRow : VisualElement
    {
        internal ConcordantDatabase Database;
        bool showButton;
        
        Button add;
        internal VisualElement TopRow;
        internal TextField ID;
        internal TextField Category;
        internal DropdownField Categories;
            
        readonly string DISALLOWED_CHARACTERS = ",\\/\n\t\r";
        string ValidateInputValue(string value) => new(value.Where(c => !DISALLOWED_CHARACTERS.Contains(c)).ToArray());

        public Action<string, string, ConcordantDatabaseEntry> OnAdd;

        public ConcordantEditorRow(ConcordantDatabase database, bool showButton = false, string buttonText = "+")
        {
            Database = database;
            this.showButton = showButton;
            
            AddToClassList("flex_column");

            TopRow = new();
            TopRow.AddToClassList("flex_row");
            Add(TopRow);
            
            add = new();
            add.text = buttonText;
            TopRow.Add(add);

            ID = new TextField("ID:");
            TopRow.Add(ID);
            
            Category = new TextField("Category:");
            TopRow.Add(Category);

            Categories = new DropdownField(Database.GetCategories(), 0);
            TopRow.Add(Categories);
            
            RefreshButton();

            ID.RegisterValueChangedCallback(IDCallback);
            Category.RegisterValueChangedCallback(CategoryCallback);
            Categories.RegisterValueChangedCallback(CategoriesCallback);
            add.clicked += ButtonClicked;
        }

        internal virtual void IDCallback(ChangeEvent<string> evt)
        {
            var newValue = ValidateInputValue(evt.newValue);
            ID.SetValueWithoutNotify(newValue);
                
            RefreshButton();
        }

        internal virtual void CategoryCallback(ChangeEvent<string> evt)
        {
            var newValue = ValidateInputValue(evt.newValue);
            Category.SetValueWithoutNotify(ValidateInputValue(evt.newValue));

            Categories.SetValueWithoutNotify(!Database.GetCategories().Contains(newValue) ? "" : newValue);
                
            RefreshButton();
        }

        internal virtual void CategoriesCallback(ChangeEvent<string> evt)
        {
            Category.value = evt.newValue;
        }

        internal virtual void ButtonClicked()
        {
            var entry = Database.Add(ID.text, Category.text);
            OnAdd?.Invoke(ID.text, Category.text, entry);

            ID.SetValueWithoutNotify("");
            Category.SetValueWithoutNotify("");
            RefreshCategories();
            Categories.SetValueWithoutNotify("");
            
            RefreshButton();
        }
        
        internal virtual void RefreshButton()
        {
            add.style.visibility = !showButton ||
                                   string.IsNullOrEmpty(ID.value) ||
                                   string.IsNullOrEmpty(Category.value) ||
                                   Database.Contains(ID.value, Category.value)
                ? new StyleEnum<Visibility>(Visibility.Hidden)
                : new StyleEnum<Visibility>(Visibility.Visible);
        }

        public void RefreshCategories()
        {
            Categories.choices = Database.GetCategories();
        }
    }
}

#endif