using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class SaveData
{
    public Vector2 playerPosition;
    public List<ObjectSaveData> objectData = new List<ObjectSaveData>();
}
[System.Serializable]
public class ObjectSaveData
{
    public string type;
    public Vector2 position;
    public float rotation;
    public string color;
    public bool isActive;
}
