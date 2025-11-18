using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SaveHolder
{
    public static Dictionary<string, ISaveable> objects = new Dictionary<string, ISaveable>();

    public static void Register(ISaveable obj) // Registers the object into the list of objects in scene
    {
        if (!objects.ContainsKey(obj.SaveID))
            objects.Add(obj.SaveID, obj);
    }

    public static void Unregister(ISaveable obj) // Remove the registration of the specific object to rebuild
    {
        if (objects.ContainsKey(obj.SaveID))
            objects.Remove(obj.SaveID);
    }
}