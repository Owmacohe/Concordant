#if UNITY_EDITOR

using System;
using System.Linq;
using UnityEngine.UIElements;
using Concordant.Data;
using UnityEngine.Serialization;

namespace Concordant.Editor
{
    /// <summary>
    /// A row in a ConcordantEditor, either used to add new entries or as an entry itself
    /// </summary>
    class ConcordantEditorRow : VisualElement
    {
        /// <summary>
        /// The current database that the ConcordantEditor is modifying
        /// </summary>
        internal ConcordantDatabase Database;
        
        /// <summary>
        /// The main container for the content of the row
        /// </summary>
        internal VisualElement TopRow;
        
        /// <summary>
        /// The entry's ID field
        /// </summary>
        internal TextField ID;
        
        /// <summary>
        /// The entry's category field
        /// </summary>
        internal TextField Category;
        
        /// <summary>
        /// The categories dropdown (for easier category selection)
        /// </summary>
        internal DropdownField Categories;
        
        Button button; // The button associated with this row that performs some action
            
        readonly string DISALLOWED_CHARACTERS = ",\\/\n\t\r"; // Characters not allowed in the entry fields
        
        /// <summary>
        /// Returns a string, without the disallowed characters
        /// </summary>
        /// <param name="value">The string to be checked</param>
        /// <returns>A validated string, with no disallowed characters</returns>
        string ValidateInputValue(string value) => new(value.Where(c => !DISALLOWED_CHARACTERS.Contains(c)).ToArray());

        /// <summary>
        /// A callback with information about this entry that gets called when an entry is created from this row
        /// </summary>
        public Action<string, string, ConcordantDatabaseEntry> OnEntryCreated;

        /// <summary>
        /// Parameterized constructor
        /// </summary>
        /// <param name="database">The current database that the ConcordantEditor is modifying</param>
        /// <param name="buttonText">The text that this row's button should have</param>
        public ConcordantEditorRow(ConcordantDatabase database, string buttonText = "+")
        {
            Database = database;
            
            AddToClassList("flex_column");

            TopRow = new();
            TopRow.AddToClassList("flex_row");
            Add(TopRow);
            
            button = new();
            button.text = buttonText;
            TopRow.Add(button);

            ID = new TextField("ID:");
            TopRow.Add(ID);
            
            Category = new TextField("Category:");
            TopRow.Add(Category);

            Categories = new DropdownField(Database.GetCategories(), 0);
            TopRow.Add(Categories);
            
            RefreshButtonVisibility();

            ID.RegisterValueChangedCallback(IDCallback);
            Category.RegisterValueChangedCallback(CategoryCallback);
            Categories.RegisterValueChangedCallback(CategoriesCallback);
            button.clicked += ButtonClicked;
        }

        /// <summary>
        /// The callback that occurs when the ID field's value is changed
        /// </summary>
        internal virtual void IDCallback(ChangeEvent<string> evt)
        {
            var newValue = ValidateInputValue(evt.newValue);
            ID.SetValueWithoutNotify(newValue);
                
            RefreshButtonVisibility();
        }

        /// <summary>
        /// The callback that occurs when the category field's value is changed
        /// </summary>
        internal virtual void CategoryCallback(ChangeEvent<string> evt)
        {
            var newValue = ValidateInputValue(evt.newValue);
            Category.SetValueWithoutNotify(ValidateInputValue(evt.newValue));

            Categories.SetValueWithoutNotify(!Database.GetCategories().Contains(newValue) ? "" : newValue);
                
            RefreshButtonVisibility();
        }

        /// <summary>
        /// The callback that occurs when the category dropdown's value is changed
        /// </summary>
        internal virtual void CategoriesCallback(ChangeEvent<string> evt)
        {
            Category.value = evt.newValue;
        }

        /// <summary>
        /// The callback that occurs when the button is clicked
        /// </summary>
        internal virtual void ButtonClicked()
        {
            var entry = Database.Add(ID.text, Category.text);
            OnEntryCreated?.Invoke(ID.text, Category.text, entry);

            ID.SetValueWithoutNotify("");
            Category.SetValueWithoutNotify("");
            RefreshCategories();
            Categories.SetValueWithoutNotify("");
            
            RefreshButtonVisibility();
        }
        
        /// <summary>
        /// Updates the button to ensure that it won't appear if the ID and category already appear in the database
        /// (for the input row, override if you want to change the functionality)
        /// </summary>
        internal virtual void RefreshButtonVisibility()
        {
            button.style.visibility = string.IsNullOrEmpty(ID.value) ||
                                   string.IsNullOrEmpty(Category.value) ||
                                   Database.Contains(ID.value, Category.value)
                ? new StyleEnum<Visibility>(Visibility.Hidden)
                : new StyleEnum<Visibility>(Visibility.Visible);
        }

        /// <summary>
        /// Updates the category dropdown with all categories in the database
        /// </summary>
        public void RefreshCategories()
        {
            Categories.choices = Database.GetCategories();
        }
    }
}

#endif