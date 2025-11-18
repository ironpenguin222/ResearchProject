using UnityEngine;
using System.Collections.Generic;

public class SavePrefabsManager : MonoBehaviour
{
    public List<SaveablePrefabEntry> prefabs;

    private Dictionary<string, GameObject> prefabCheck;

    private void Awake()
    {
        prefabCheck = new Dictionary<string, GameObject>();

        foreach (var entry in prefabs)
        {
            prefabCheck[entry.type] = entry.prefab;
        }
    }

    public GameObject GetPrefab(string type)
    {
        prefabCheck.TryGetValue(type, out GameObject prefab);
        return prefab;
    }
}

[System.Serializable]
public class SaveablePrefabEntry
{
    public string type;
    public GameObject prefab;
}
