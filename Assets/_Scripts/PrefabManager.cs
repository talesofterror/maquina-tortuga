using UnityEngine;
using System.Collections.Generic;

public class PrefabManager : MonoBehaviour
{
    public List<GameObject> prefabsList;
    private Dictionary<string, GameObject> prefabDictionary = new Dictionary<string, GameObject>();

    void Awake()
    {
        foreach (GameObject prefab in prefabsList)
        {
            if (!prefabDictionary.ContainsKey(prefab.name))
            {
                prefabDictionary.Add(prefab.name, prefab);
            }
        }

        Debug.Log("PrefabManager initialized with " + prefabDictionary.Count + " prefabs.");

    }

    public GameObject InstantiatePrefab(string name, Vector3 position, Quaternion rotation)
    {
        if (prefabDictionary.ContainsKey(name))
        {
            GameObject prefab = prefabDictionary[name];
            return Instantiate(prefab, position, rotation);
        }
        else
        {
            Debug.LogError("Prefab '" + name + "' not found in dictionary!");
            return null;
        }
    }

    public void DestroyPrefab(GameObject prefabInstance)
    {
        if (prefabInstance != null)
        {
            Destroy(prefabInstance);
        }
        else
        {
            Debug.LogError("Prefab instance is null, cannot destroy!");
        }
    }
}
