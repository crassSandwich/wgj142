﻿using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace WitchOS.Editor
{
    public class SOLookupTableAutoPopulator : AssetPostprocessor
    {
        const string SO_LOOKUP_TABLE_PATH = "Assets/Prefabs/SO Lookup Table.prefab";

        // every ScriptableObject in these directories is automatically added to the lookup table prefab
        static readonly string[] DIRECTORIES_TO_TRACK = new string[]
        {
            "Assets/ScriptableObjects/Emails",
            "Assets/ScriptableObjects/Invoices",
            "Assets/ScriptableObjects/Wiki Pages"
        };

        static GameObject _lutPrefab;
        static GameObject lutPrefab => _lutPrefab ?? (_lutPrefab = AssetDatabase.LoadAssetAtPath(SO_LOOKUP_TABLE_PATH, typeof(Object)) as GameObject);

        static SOLookupTable lookupTable => lutPrefab.GetComponent<SOLookupTable>();

        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "this is an AssetPostProcessor message")]
        static void OnPostprocessAllAssets (string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            if (importedAssets.Length == 1 && importedAssets[0] == SO_LOOKUP_TABLE_PATH)
            {
                return;
            }

            // remove first in case asset moved between two tracked directories
            foreach (var path in pathsInTrackedDirectories(deletedAssets.Concat(movedFromAssetPaths)))
            {
                lookupTable.Delete(pathToID(path));
            }

            var toAdd = pathsInTrackedDirectories(importedAssets.Concat(movedAssets))
                .Where(p => AssetDatabase.GetMainAssetTypeAtPath(p).IsSubclassOf(typeof(ScriptableObject)));

            foreach (var path in toAdd)
            {
                lookupTable.Write(pathToID(path), AssetDatabase.LoadAssetAtPath(path, typeof(ScriptableObject)) as ScriptableObject);
            }

            PrefabUtility.SavePrefabAsset(lutPrefab);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "this is a Tools menu item")]
        [MenuItem("Tools/Manually repopulate SO lookup table")]
        static void manuallyRepopulateTable ()
        {
            lookupTable.Clear();

            var toAdd = AssetDatabase.FindAssets("t:ScriptableObject", DIRECTORIES_TO_TRACK).Select(guid => AssetDatabase.GUIDToAssetPath(guid));

            foreach (var path in toAdd)
            {
                lookupTable.Write(pathToID(path), AssetDatabase.LoadAssetAtPath(path, typeof(ScriptableObject)) as ScriptableObject);
            }

            PrefabUtility.SavePrefabAsset(lutPrefab);
        }

        static IEnumerable<string> pathsInTrackedDirectories (IEnumerable<string> pathList)
        {
            return pathList.Where(p => DIRECTORIES_TO_TRACK.Any(d => p.StartsWith(d)));
        }

        static string pathToID (string path)
        {
            for (int i = 0; i < DIRECTORIES_TO_TRACK.Length; i++)
            {
                string directory = DIRECTORIES_TO_TRACK[i];

                if (path.StartsWith(directory)) return path.Replace(directory, $"d{i}").Replace(".asset", "");
            }

            throw new System.ArgumentException($"'{path}' is not a path to any tracked SOs");
        }
    }
}
