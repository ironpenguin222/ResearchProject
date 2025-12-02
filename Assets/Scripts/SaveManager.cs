using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public PlayerController player;
    private List<ISaveable> saveableObjects = new List<ISaveable>();
    private Dictionary<string, ISaveable> saveableSearch = new Dictionary<string, ISaveable>();
    public SavePrefabsManager prefabManager;
    public PoolManager poolManager;
    private string SavePath(int slotNumber)
    {
        return Path.Combine(Application.persistentDataPath, slotNumber.ToString());
    }

    public void Start()
    {
        string target = PlayerPrefs.GetString("TargetLevel", "");
        if (!string.IsNullOrEmpty(target))
        {
            LoadGame(-1, target);
        }
    }

    private bool IsColorAllowed(string color, int slotNumber)
    {
        if (string.IsNullOrEmpty(color)) // Always let colorless save/load
            return true;

        if (slotNumber == 1)
            return color == "Green"; //  Green allowed

        if (slotNumber == 2)
            return color == "Blue"; // Slot 2 = Blue allowed

        return true;
    }

    public void SaveGame(int slotNumber)
    {
        SaveData data = new SaveData();
        data.playerPosition = player.transform.position;
        data.objectData = new List<ObjectSaveData>();


        foreach (var obj in SaveHolder.objects.Values)  // Saves the data for each component
        {
            var dataForObj = obj.SaveData();
            string color = dataForObj.Get("color");

            if (!IsColorAllowed(color, slotNumber))
                continue;

            data.objectData.Add(dataForObj);
        }

        string json = JsonUtility.ToJson(data, true);

        string path = SavePath(slotNumber); 
        try
        {
            File.WriteAllText(path, json);
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Failed to save: " + ex.Message);
        }
        File.WriteAllText(path, json);
        Debug.Log(Application.persistentDataPath);
    }

    public void LoadingGame(int slotNumber)
    {
        LoadGame(slotNumber, "");
    }

    public void LoadGame(int slotNumber, string levelName = "") // Loads saved data
    {
        Debug.Log("bup");
        string path;

        if (slotNumber == -1)
        {
            path = Path.Combine(Application.streamingAssetsPath, "Levels", levelName + ".json");
        }
        else
        {
            path = SavePath(slotNumber);
        }

        if (!File.Exists(path))
            return;

        string json = File.ReadAllText(path);
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        player.transform.position = data.playerPosition; // Sets player to saved position

        List<GameObject> toPool = new List<GameObject>();

        foreach (var obj in SaveHolder.objects.Values)
        {
            MonoBehaviour allObjects = obj as MonoBehaviour; // Finds all objects
            if (allObjects == null)
                continue;

            ISaveable saveable = allObjects.GetComponent<ISaveable>();
            var saved = saveable.SaveData();
            string color = saved.Get("color");

            if (slotNumber != -1 && IsColorAllowed(color, slotNumber))
                toPool.Add(allObjects.gameObject);

            if (slotNumber == -1)
                toPool.Add(allObjects.gameObject);
        }

        foreach (GameObject g in toPool)
        {
            ISaveable saveable = g.GetComponent<ISaveable>();
            SaveHolder.Unregister(saveable);
            string type = saveable.SaveData().type;
            poolManager.Return(type, g); // Returns objects to pool instead of destroying

        }

        foreach (var objData in data.objectData)
        {
            if (slotNumber != -1 && !IsColorAllowed(objData.Get("color"), slotNumber))
                continue;

            GameObject prefab = prefabManager.GetPrefab(objData.type); // Data type to load prefab for
            if (prefab == null)
            {
                Debug.LogError("Missing prefab " + objData.type);
                continue;
            }

            GameObject newObj = poolManager.Get(objData.type, prefab); // Reuses pooled object or instantiates new one
            ISaveable saveable = newObj.GetComponent<ISaveable>(); // Adds ISaveable

            saveable.LoadData(objData); // Loads the object data
            SaveHolder.Register(saveable);
        }

        Debug.Log("Game loaded.");
    }
}

