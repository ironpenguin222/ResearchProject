using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class SaveData
{
    public Vector2 playerPosition; // Player's position
    public List<ObjectSaveData> objectData = new List<ObjectSaveData>(); // List of save data for all objects
}

[System.Serializable]
public class ObjectSaveData
{
    public string id; // Object id (name)
    public string type; // Object type (Ie: Box, Conveyor for now since those are the ones I got)
    public List<KeyValue> data = new List<KeyValue>(); // data stored into as names/properties
    private Dictionary<string, string> dataDictionary; // dictionary to store data

    private void CheckDictionary()
    {
        if (dataDictionary != null) return;

        dataDictionary = new Dictionary<string, string>();
        foreach (var kv in data)
            dataDictionary[kv.key] = kv.value;
    }


    // Set key value, it it exists it should update, otherwise it should make a new one
    public void Set(string key, string value)
    {
        CheckDictionary();

        // Update runtime dictionary
        dataDictionary[key] = value;

        // Sync changes back to serializable list
        bool found = false;
        for (int i = 0; i < data.Count; i++)
        {
            if (data[i].key == key)
            {
                data[i].value = value;
                found = true;
                break;
            }
        }

        if (!found)
            data.Add(new KeyValue { key = key, value = value });
    }

    // Retrive the value for a key
    public string Get(string key)
    {
        CheckDictionary();
        return dataDictionary.TryGetValue(key, out string value) ? value : null;
    }
}

[System.Serializable]
public class KeyValue
{
    public string key; // Name
    public string value; // Value
}


