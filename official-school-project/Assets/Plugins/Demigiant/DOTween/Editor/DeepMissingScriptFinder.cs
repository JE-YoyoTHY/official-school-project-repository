using UnityEngine;
using UnityEditor;

public class DeepMissingScriptFinder
{
    [MenuItem("Tools/Find All Missing Scripts in Scene")]
    private static void FindAllMissingScriptsInScene()
    {
        int count = 0;
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>(true);

        foreach (GameObject go in allObjects)
        {
            var components = go.GetComponents<Component>();
            for (int i = 0; i < components.Length; i++)
            {
                if (components[i] == null)
                {
                    Debug.LogWarning($"Missing script found on GameObject: '{GetFullPath(go)}'", go);
                    count++;
                }
            }
        }

        Debug.Log($"完成掃描，共發現 {count} 個 missing script。");
    }

    private static string GetFullPath(GameObject obj)
    {
        string path = "/" + obj.name;
        while (obj.transform.parent != null)
        {
            obj = obj.transform.parent.gameObject;
            path = "/" + obj.name + path;
        }
        return path;
    }
}
