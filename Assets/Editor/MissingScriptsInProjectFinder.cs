// Place this script in Assets/Editor/MissingScriptsInProjectFinder.cs
using UnityEditor;
using UnityEngine;

public class MissingScriptsInProjectFinder
{
    [MenuItem("Tools/Find Missing Scripts In Project")]
    static void FindMissingScriptsInProject()
    {
        string[] allAssetPaths = AssetDatabase.GetAllAssetPaths();
        int missingCount = 0;

        foreach (string assetPath in allAssetPaths)
        {
            if (!assetPath.EndsWith(".prefab") &&
                !assetPath.EndsWith(".unity") &&
                !assetPath.EndsWith(".asset"))
            {
                continue;
            }

            Object asset = AssetDatabase.LoadMainAssetAtPath(assetPath);
            if (asset == null)
                continue;

            GameObject go = asset as GameObject;

            // For scenes and prefabs
            if (go != null)
            {
                missingCount += CheckGameObject(go, assetPath);
            }
            else
            {
                // For other assets like ScriptableObjects
                SerializedObject so = new SerializedObject(asset);
                SerializedProperty prop = so.GetIterator();

                while (prop.NextVisible(true))
                {
                    if (prop.propertyType == SerializedPropertyType.ObjectReference && prop.objectReferenceValue == null && prop.objectReferenceInstanceIDValue != 0)
                    {
                        Debug.LogWarning($"Missing reference in asset: {assetPath}", asset);
                        missingCount++;
                    }
                }
            }
        }

        if (missingCount == 0)
            Debug.Log("No missing scripts found in project.");
        else
            Debug.LogWarning($"Found {missingCount} missing script references in project.");
    }

    static int CheckGameObject(GameObject go, string assetPath)
    {
        int missingCount = 0;

        Component[] components = go.GetComponents<Component>();
        for (int i = 0; i < components.Length; i++)
        {
            if (components[i] == null)
            {
                Debug.LogWarning($"Missing script in prefab/scene: {assetPath} on GameObject: {GetGameObjectPath(go)}", go);
                missingCount++;
            }
        }

        foreach (Transform child in go.transform)
        {
            missingCount += CheckGameObject(child.gameObject, assetPath);
        }

        return missingCount;
    }

    static string GetGameObjectPath(GameObject go)
    {
        string path = go.name;
        Transform current = go.transform;
        while (current.parent != null)
        {
            current = current.parent;
            path = current.name + "/" + path;
        }
        return path;
    }
}
