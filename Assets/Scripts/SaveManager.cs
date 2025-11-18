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

    public void SaveGame(int slotNumber)
    {
        SaveData data = new SaveData();
        data.playerPosition = player.transform.position;
        data.objectData = new List<ObjectSaveData>();


        foreach (var obj in SaveHolder.objects.Values) // Saves the data for each component
        {
            data.objectData.Add(obj.SaveData());
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

        List<GameObject> toDestroy = new List<GameObject>();

        foreach (var obj in SaveHolder.objects.Values)
        {
            MonoBehaviour allObjects = obj as MonoBehaviour; // Finds all objects
            if (allObjects != null)
            {
                ISaveable saveable = allObjects.GetComponent<ISaveable>();
                var saved = saveable.SaveData();
                if (saved.Get("color") == "Blue") // Skip killing it if this is a blue object
                    continue;

                toDestroy.Add(allObjects.gameObject); // Objects that need to go
            }
        }

        SaveHolder.objects.Clear();

        foreach (GameObject destObject in toDestroy)
        {
            if (destObject != null)
                Destroy(destObject); // Destroys objects to reload
        }


        foreach (var objData in data.objectData)
        {
            if (objData.Get("color") == "Blue") // Skip skip
            {
                continue;
            }
            GameObject prefab = prefabManager.GetPrefab(objData.type); // Data type to load prefab for
            if (prefab == null)
            {
                Debug.LogError("Missing prefab " + objData.type);
                continue;
            }

            GameObject newObj = Instantiate(prefab); // Instantiates new objects to be objects in scene
            ISaveable saveable = newObj.GetComponent<ISaveable>(); // Adds ISaveable


            saveable.LoadData(objData); // Loads the object data
        }

        Debug.Log("Game loaded.");
    }
    }

