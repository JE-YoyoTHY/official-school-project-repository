using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public static class GameObjectMethods
{
    public static List<GameObject> GetAllChildren(GameObject m_obj)
    {
        List<GameObject> allChildren = new List<GameObject>();
        for (int i = 0; i < m_obj.transform.childCount; i++)
        {
            allChildren.Add(m_obj.transform.GetChild(i).transform.gameObject);
        }
        return allChildren;
    }

    public static GameObject GetLastChild(GameObject m_obj)
    {
        return m_obj.transform.GetChild(m_obj.transform.childCount - 1).gameObject;
    }

    public static void DeactivateAllGameObjectByName(string targetName)
    {
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>(true); // ¥]§t inactive ªº
        foreach (GameObject obj in allObjects)
        {
            Debug.Log(obj.name);
            if (obj.name == targetName)
            {
                obj.SetActive(false);
            }
        }
    }




}
