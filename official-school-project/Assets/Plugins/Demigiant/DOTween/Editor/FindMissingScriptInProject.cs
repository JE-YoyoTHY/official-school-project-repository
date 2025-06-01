using UnityEngine;
using UnityEditor;
using System.IO;

public class ProjectWideMissingScriptFinder
{
    [MenuItem("Tools/Find Missing Scripts in Project")]
    static void FindMissingScriptsInProject()
    {
        string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab");
        string[] soGuids = AssetDatabase.FindAssets("t:ScriptableObject");

        int totalMissing = 0;

        foreach (string guid in prefabGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (go == null) continue;

            Component[] components = go.GetComponentsInChildren<Component>(true);
            foreach (var comp in components)
            {
                if (comp == null)
                {
                    Debug.LogWarning($"Missing script in prefab: {path}", go);
                    totalMissing++;
                    break;
                }
            }
        }

        foreach (string guid in soGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            var obj = AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);
            if (obj == null) continue;

            var serialized = new SerializedObject(obj);
            var prop = serialized.GetIterator();
            while (prop.NextVisible(true))
            {
                if (prop.propertyType == SerializedPropertyType.ObjectReference && prop.objectReferenceValue == null && prop.objectReferenceInstanceIDValue != 0)
                {
                    Debug.LogWarning($"Missing reference in ScriptableObject: {path}", obj);
                    totalMissing++;
                    break;
                }
            }
        }

        Debug.Log($"資產掃描完成。Missing script 共發現：{totalMissing} 個");
    }
}
