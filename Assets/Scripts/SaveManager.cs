using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SaveManager : MonoBehaviour
{
    public PlayerController player;
    private List<ISaveable> saveableObjects = new List<ISaveable>();
    private Dictionary<string, ISaveable> saveableSearch = new Dictionary<string, ISaveable>();
    public SavePrefabsManager prefabManager;
    private string SavePath(int slotNumber)
    {
        return Path.Combine(Application.persistentDataPath, $"saveSlot_{slotNumber}.json");
    }

    private void Start()
    {
        MonoBehaviour[] gameObjects = FindObjectsOfType<MonoBehaviour>(true); // Grabs all objects in scene

        foreach (MonoBehaviour objects in gameObjects)
        {
            if (objects is ISaveable saveable) // Puts any object that uses Isaveable into the category
            {
                saveableObjects.Add(saveable);
                saveableSearch[saveable.SaveID] = saveable;
            }
        }
    }

    public void SaveGame(int slotNumber)
    {
        SaveData data = new SaveData();
        data.playerPosition = player.transform.position;
        data.objectData.Clear();
        foreach (ISaveable objects in saveableObjects) // Saves the data for each component
        {
            data.objectData.Add(objects.SaveData());
        }

        string json = JsonUtility.ToJson(data, true);

        string path = SavePath(slotNumber);
        File.WriteAllText(path, json);
        Debug.Log(Application.persistentDataPath);
    }

    public void LoadGame(int slotNumber) // Loads saved data
    {
        string path = SavePath(slotNumber);

        if (!File.Exists(path))
            return;

        string json = File.ReadAllText(path);
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        player.transform.position = data.playerPosition; // Sets player to saved position

        HashSet<string> existingObjects = new HashSet<string>();

        foreach (var s in saveableObjects)
        {
            existingObjects.Add(s.SaveID);
        }

        foreach (var objData in data.objectData)
        {
            if (objData.Get("color") == "Blue") // Skip all data that uses the blue color
                continue;

            if (saveableSearch.TryGetValue(objData.id, out ISaveable existing)) // Loads object if it exists
            {
                existing.LoadData(objData);
                continue;
            }

            GameObject prefab = prefabManager.GetPrefab(objData.type); // Instantiates if object does not exist 

            if (prefab == null)
            {
                Debug.LogError("None " + objData.type);
                continue;
            }

            GameObject newObj = Instantiate(prefab);
            ISaveable newSaveable = newObj.GetComponent<ISaveable>();

            newSaveable.LoadData(objData);

            saveableObjects.Add(newSaveable); // Register new spawned objects
            saveableSearch[newSaveable.SaveID] = newSaveable;

            Debug.Log("Spawned missing " + objData.id);
        }
    }
}
