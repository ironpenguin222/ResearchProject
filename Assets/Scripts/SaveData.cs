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

    // Set key value, it it exists it should update, otherwise it should make a new one
    public void Set(string key, string value)
    {
        var kv = data.Find(x => x.key == key);
        if (kv != null) kv.value = value;
        else data.Add(new KeyValue { key = key, value = value }); // Add new pair
    }

    // Retrive the value for a key
    public string Get(string key)
    {
        var kv = data.Find(x => x.key == key);
        if (kv != null)
        {
            return kv.value; // Return value
        }
        else
        {
            return null; // If it doesn't exist then its null
        }
    }
}

[System.Serializable]
public class KeyValue
{
    public string key; // Name
    public string value; // Value
}


