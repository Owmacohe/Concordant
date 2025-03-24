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
    public class ConcordantEditor : EditorWindow
    {
        ConcordantDatabase database;
        
        [MenuItem("Window/Concordant/Editor"), MenuItem("Tools/Concordant/Editor")]
        public static void Open()
        {
            GetWindow<ConcordantEditor>("Concordant Database Editor");
        }
        
        void CreateGUI()
        {
            ConcordantEditorUtilities.AddStyleSheet(rootVisualElement.styleSheets, "ConcordantEditorStyleSheet");

            if (database == null) return;

            CreateToolbar();
            CreateDatabaseList();
            CreateInputRow();

            RefreshEntries();
        }

        void ClearEntries()
        {
            int count = databaseList.childCount;
            
            for (int i = 0; i < count; i++)
                databaseList.RemoveAt(0);
            
            entries.Clear();
        }

        void SetEntries(List<string> ids)
        {
            foreach (var i in ids)
            {
                var split = i.Split('/');
                AddEntry(split[1], split[0], database.Entries.Get(i));
            }
        }

        void ClearGUI()
        {
            ClearEntries();
                
            while (rootVisualElement.childCount > 0)
                rootVisualElement.RemoveAt(0);
        }

        void RefreshEntries()
        {
            ClearEntries();
            SetEntries(database.Entries.Keys.OrderBy(key => key).ToList());
            count.text = $"{database.Entries.Count} entries";
        }

        TextElement count;
        ConcordantEditorRow inputRow;
        ScrollView databaseList;
        List<ConcordantEditorEntry> entries = new();
        bool isEven;

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

            search.RegisterValueChangedCallback(evt =>
            {
                ClearEntries();
                SetEntries(database.Search(evt.newValue));
            });

            export.clicked += () =>
            {
                var path = StandaloneFileBrowser.SaveFilePanel("Export Concordant Database", "", database.name, "csv");
                File.WriteAllText(path, database.ToString());
            };

            import.clicked += () =>
            {
                ClearGUI();

                var name = database.name;

                var paths = StandaloneFileBrowser.OpenFilePanel("Import Concordant Database", "", "csv", false);
                database.Import(File.ReadAllText(paths[0]));
                database.name = name;
                
                ConcordantEditorUtilities.SaveSerializedObject(database);
                
                CreateGUI();
            };
        }
        
        void CreateDatabaseList()
        {
            VisualElement rule = new();
            rule.AddToClassList("hr");
            rootVisualElement.Add(rule);
            
            databaseList = new();
            databaseList.AddToClassList("flex_column");
            rootVisualElement.Add(databaseList);
        }

        void CreateInputRow()
        {
            inputRow = new(database, true);
            inputRow.style.minHeight = new StyleLength(30);
            rootVisualElement.Insert(2, inputRow);
            
            inputRow.OnAdd += (id, category, entry) =>
            {
                AddEntry(id, category, entry);
                RefreshEntries();
            };
        }

        void AddEntry(string id, string category, ConcordantDatabaseEntry entry)
        {
            RefreshCategories();
                
            ConcordantEditorEntry editorEntry = new ConcordantEditorEntry(
                database,
                id, category, entry);
            
            editorEntry.AddToClassList(isEven ? "entry_even" : "entry_odd");
            isEven = !isEven;
            
            editorEntry.OnChangeCategory += RefreshCategories;
            editorEntry.OnRemoved += () =>
            {
                databaseList.Remove(editorEntry);
                entries.Remove(editorEntry);
                    
                inputRow.RefreshButton();
                
                RefreshEntries();
            };
                
            databaseList.Add(editorEntry);
            entries.Add(editorEntry);
        }

        void RefreshCategories()
        {
            inputRow.RefreshCategories();
            
            foreach (var i in entries)
                i.RefreshCategories();
        }

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