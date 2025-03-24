#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using Concordant.Data;

namespace Concordant.Editor
{
    /// <summary>
    /// Static class for managing the creation and editing of new Concordant files
    /// </summary>
    public static class ConcordantFileHandler
    {
        /// <summary>
        /// Gets a ScriptableObject from the instanceID of a file
        /// </summary>
        /// <param name="instanceID">The instanceID of the file being checked</param>
        static T GetAssetFromInstanceID<T>(int instanceID) where T : ScriptableObject
        {
            AssetDatabase.TryGetGUIDAndLocalFileIdentifier(instanceID, out string guid, out long _);
            
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            Object asset = AssetDatabase.LoadAssetAtPath(assetPath, typeof(Object));

            if (asset.GetType() == typeof(ConcordantDatabase)) return (T) asset;
            return null;
        }
        
        /// <summary>
        /// Checks to see whether the file with the supplied instanceID is a ConcordantDatabase
        /// </summary>
        /// <param name="instanceID">The instanceID of the file being checked</param>
        /// <returns>Whether the file is a ConcordantDatabase</returns>
        static bool IsConcordantDatabaseFile(int instanceID)
        {
            return GetAssetFromInstanceID<ConcordantDatabase>(instanceID) != null;
        }
        
        /// <summary>
        /// Project view contextual menu edit option for Concordant Database files
        /// </summary>
        [MenuItem("Assets/Edit Concordant Database")]
        static void EditGraphFile() {
            EditorWindow.GetWindow<ConcordantEditor>("Concordant Database Editor").Load(
                GetAssetFromInstanceID<ConcordantDatabase>(Selection.activeObject.GetInstanceID()));
        }
 
        /// <summary>
        /// Method to confirm that the edit option only shows up for Concordant Database files
        /// </summary>
        /// <returns>Whether the selected file is a Concordant Database file</returns>
        [MenuItem("Assets/Edit Concordant Database", true)]
        static bool ConfirmEditGraphFile() {
            return IsConcordantDatabaseFile(Selection.activeObject.GetInstanceID());
        }
    }
}
#endif