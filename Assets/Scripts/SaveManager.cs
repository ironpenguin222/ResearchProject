using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SaveManager : MonoBehaviour
{
    public PlayerController player;
    private List<ISaveable> saveableObjects = new List<ISaveable>();

    private void Start()
    {
        MonoBehaviour[] gameObjects = FindObjectsOfType<MonoBehaviour>(); // Grabs all objects in scene

        foreach (MonoBehaviour objects in gameObjects) 
        {
            if (objects is ISaveable saveable) // Puts any object that uses Isaveable into the category
            {
                saveableObjects.Add(saveable);
            }
        }
    }

    public void SaveGame()
    {
        SaveData data = new SaveData();
        data.playerPosition = player.transform.position;
        data.objectData.Clear();
        foreach(ISaveable objects in saveableObjects) // Saves the data for each component
        {
            data.objectData.Add(objects.SaveData());
        }
        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString("SaveData", json);
        PlayerPrefs.Save();
    }

    public void LoadGame() // Loads saved data
    {
        if (!PlayerPrefs.HasKey("SaveData"))
        {
            return;
        }

        string json = PlayerPrefs.GetString("SaveData");
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        player.transform.position = data.playerPosition;

        int i = 0;
       foreach (ISaveable objects in saveableObjects)
        {
            if(i < data.objectData.Count)
            {
                ObjectSaveData objData = data.objectData[i];
                if (objData.color == "Blue")
                {
                    i++;
                    continue;
                }
                objects.LoadData(data.objectData[i]);
                i++;
            }
        }
    }
}
