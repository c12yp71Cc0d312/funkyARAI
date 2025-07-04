// Place this script in an "Editor" folder (Assets/Editor/MissingScriptFinder.cs)
using UnityEngine;
using UnityEditor;

public class MissingScriptFinder : MonoBehaviour
{
    [MenuItem("Tools/Find Missing Scripts In Scene")]
    static void FindMissingScripts()
    {
        int goCount = 0;
        int componentsCount = 0;
        int missingCount = 0;

        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();

        foreach (GameObject go in allObjects)
        {
            goCount++;
            Component[] components = go.GetComponents<Component>();

            for (int i = 0; i < components.Length; i++)
            {
                componentsCount++;
                if (components[i] == null)
                {
                    missingCount++;
                    string path = GetGameObjectPath(go);
                    Debug.LogWarning($"Missing script in: {path}", go);
                }
            }
        }

        Debug.Log($"Scanned {goCount} GameObjects, {componentsCount} components. Found {missingCount} missing scripts.");
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
