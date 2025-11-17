using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SaveManager : MonoBehaviour
{
    public PlayerController player;
    private List<ISaveable> saveableObjects = new List<ISaveable>();
    private Dictionary<string, ISaveable> saveableSearch = new Dictionary<string, ISaveable>();
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
        System.IO.File.WriteAllText(path, json);
        Debug.Log(Application.persistentDataPath);
    }

    public void LoadGame(int slotNumber) // Loads saved data
    {
        string path = SavePath(slotNumber);

        string json = File.ReadAllText(path);
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        player.transform.position = data.playerPosition; // Sets player to saved position

        foreach (var objData in data.objectData) // Loop through all objects stored in savedata
        {
            if (objData.Get("color") == "Blue")
                continue;


            if (saveableSearch.TryGetValue(objData.id, out ISaveable saveable)) // Looks for object in scene with the ID
            {
                Debug.Log("Loaded " + objData.id);
                saveable.LoadData(objData); // Load the saved data into the scene
            }
        }
    }
}
