#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Concordant.Data;
using SFB;

namespace Concordant.Editor
{
    /// <summary>
    /// The main editor window for modifying ConcordantDatabase files
    /// </summary>
    public class ConcordantEditor : EditorWindow
    {
        ConcordantDatabase database; // The database currently being modified
        
        TextElement count; // The display of the number of entries in the database
        ConcordantEditorRow inputRow; // The row & fields used to add new entries to the database
        ScrollView databaseList; // The parent of all the entry rows
        ListDictionary<string, ConcordantEditorEntry> entries = new(); // A collection of all entry rows for quick access
        bool isEven; // Used to ensure that rows alternate background colours when they are added to the database
        
        [MenuItem("Window/Concordant/Editor"), MenuItem("Tools/Concordant/Editor")]
        public static void Open()
        {
            GetWindow<ConcordantEditor>("Concordant Database Editor");
        }
        
        void CreateGUI()
        {
            ConcordantEditorUtilities.AddStyleSheet(rootVisualElement.styleSheets, "ConcordantEditorStyleSheet");
            rootVisualElement.style.backgroundColor = new StyleColor(new Color(0.17f, 0.17f, 0.17f));

            if (database == null) return;

            CreateToolbar();
            CreateDatabaseList();
            CreateInputRow();

            // Adding in the pre-existing entries from the database
            ClearEntries();
            SetEntries();
            RefreshEntryCount();
        }
        
        #region Entries
        
        /// <summary>
        /// Updates the count text which displays the total number of entries in the database
        /// </summary>
        void RefreshEntryCount() => count.text = $"{database.Entries.Count} entries";
        
        /// <summary>
        /// Removes all VisualElements from the window
        /// </summary>
        void ClearGUI()
        {
            ClearEntries();
                
            while (rootVisualElement.childCount > 0)
                rootVisualElement.RemoveAt(0);
        }

        /// <summary>
        /// Removes all entry rows from the GUI (expensive method)
        /// </summary>
        void ClearEntries()
        {
            int count = databaseList.childCount;
            
            for (int i = 0; i < count; i++)
                databaseList.RemoveAt(0);
            
            entries.Clear();
        }

        /// <summary>
        /// Adds all the entries from the database to the GUI (expensive method)
        /// </summary>
        void SetEntries()
        {
            foreach (var i in database.Entries.Keys.OrderBy(key => key)) // The entries are sorted
            {
                var split = i.Split('/');
                AddEntry(split[1], split[0], database.Entries.Get(i));
            }
        }

        /// <summary>
        /// An inexpensive method to quickly show only a certain set of keys in the GUI
        /// (all other entries are simply hidden)
        /// </summary>
        /// <param name="keys">The keys from the database to show</param>
        void SetSelection(List<string> keys)
        {
            foreach (var i in entries.InnerList)
                i.Value.style.display =
                    new StyleEnum<DisplayStyle>(keys.Contains(i.Key) ? StyleKeyword.Auto : StyleKeyword.None);
        }
        
        /// <summary>
        /// Adds a new entry row to the GUI
        /// </summary>
        /// <param name="id">The ID of the new entry</param>
        /// <param name="category">the new entry's category</param>
        /// <param name="entry">The new entry itself</param>
        void AddEntry(string id, string category, ConcordantDatabaseEntry entry)
        {
            RefreshCategories();

            var key = category + "/" + id;
                
            ConcordantEditorEntry editorEntry = new ConcordantEditorEntry(
                database,
                id, category, entry);
            
            // Ensuring that the background colour is correct
            editorEntry.AddToClassList(isEven ? "entry_even" : "entry_odd");
            isEven = !isEven;
            
            editorEntry.OnChangeCategory += RefreshCategories;
            
            // Adding the functionality for when the entry is removed from the database
            editorEntry.OnRemoved += () =>
            {
                databaseList.Remove(editorEntry);
                entries.Remove(key);
                    
                inputRow.RefreshButtonVisibility();
                
                RefreshEntryCount();
            };
                
            databaseList.Add(editorEntry);
            entries.Add(key, editorEntry);
        }
        
        #endregion
        
        #region GUI

        /// <summary>
        /// Creates the toolbar that runs along the top of the window
        /// </summary>
        void CreateToolbar()
        {
            VisualElement toolbar = new();
            toolbar.AddToClassList("flex_row");
            toolbar.style.minHeight = new StyleLength(40);
            rootVisualElement.Add(toolbar);

            TextElement title = new();
            title.text = database.name;
            title.AddToClassList("h1");
            toolbar.Add(title);
            
            Button save = new();
            save.text = "Save";
            save.AddToClassList("h2");
            toolbar.Add(save);
            
            TextField search = new TextField("Search:");
            search.AddToClassList("h2");
            toolbar.Add(search);

            count = new();
            count.AddToClassList("green");
            toolbar.Add(count);

            Button export = new();
            export.text = "Export";
            export.AddToClassList("h2");
            toolbar.Add(export);

            Button import = new();
            import.text = "Import";
            import.AddToClassList("h2");
            toolbar.Add(import);
            
            VisualElement rule = new();
            rule.AddToClassList("hr");
            rootVisualElement.Add(rule);
            
            save.clicked += () => ConcordantEditorUtilities.SaveSerializedObject(database);

            // Adding functionality for the search field
            search.RegisterValueChangedCallback(evt => SetSelection(database.Search(evt.newValue)));

            // Adding functionality to get a desired file path and save the database as a CSV file
            export.clicked += () =>
            {
                var path = StandaloneFileBrowser.SaveFilePanel(
                    "Export Concordant Database", "", database.name, "csv");
                File.WriteAllText(path, database.ToString());
            };

            // Adding functionality to import a CSV file database
            import.clicked += () =>
            {
                ClearGUI();

                var paths = StandaloneFileBrowser.OpenFilePanel(
                    "Import Concordant Database", "", "csv", false);
                database.Import(File.ReadAllText(paths[0]));
                                
                ConcordantEditorUtilities.SaveSerializedObject(database);
                
                CreateGUI();
            };
        }
        
        /// <summary>
        /// Adds the parent of all the database entries
        /// </summary>
        void CreateDatabaseList()
        {
            VisualElement rule = new();
            rule.AddToClassList("hr");
            rootVisualElement.Add(rule);
            
            databaseList = new();
            databaseList.AddToClassList("flex_column");
            rootVisualElement.Add(databaseList);
        }

        /// <summary>
        /// Adds the input row to add new entries to the database
        /// </summary>
        void CreateInputRow()
        {
            inputRow = new(database);
            inputRow.style.minHeight = new StyleLength(30);
            rootVisualElement.Insert(2, inputRow);
            
            inputRow.OnEntryCreated += (id, category, entry) =>
            {
                AddEntry(id, category, entry);
                RefreshEntryCount();
            };
        }
        
        #endregion

        /// <summary>
        /// Goes through all rows in the GUI and updates their categories dropdown
        /// </summary>
        void RefreshCategories()
        {
            inputRow.RefreshCategories();
            
            foreach (var i in entries.InnerList)
                i.Value.RefreshCategories();
        }

        /// <summary>
        /// Loads an existing ConcordantDatabase ScriptableObject into the editor
        /// </summary>
        /// <param name="database">The ScriptableObject database to be loaded</param>
        public void Load(ConcordantDatabase database)
        {
            if (this.database == database) return;
            
            if (this.database != null) ClearGUI();
            this.database = database;
            CreateGUI();
        }
    }
}

#endif