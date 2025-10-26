using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SaveManager : MonoBehaviour
{
    public PlayerController player;
    public List<BoxController> boxes = new List<BoxController>();

    private void Start()
    {
        foreach (GameObject boxObject in GameObject.FindGameObjectsWithTag("Box"))  //Finds each box in the map
        {
            BoxController box = boxObject.GetComponent<BoxController>();
            boxes.Add(box);
        }
    }

    public void SaveGame()
    {
        SaveData data = new SaveData();
        data.playerPosition = player.transform.position;
        data.objectData.Clear();
        foreach(BoxController box in boxes) // Saves the data for each part of the box
        {
            ObjectSaveData boxData = new ObjectSaveData();
            boxData.type = box.tag;
            boxData.position = box.transform.position;
            boxData.rotation = box.transform.eulerAngles.z;
            boxData.isActive = box.gameObject.activeSelf;
            data.objectData.Add(boxData);
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

        for(int i = 0; i < boxes.Count && i < data.objectData.Count; i++)
        {
            ObjectSaveData objData = data.objectData[i];
            BoxController box = boxes[i];

            box.transform.position = objData.position;
            box.transform.eulerAngles = new Vector3(0, 0, objData.rotation);
            box.gameObject.SetActive(objData.isActive);
        }
    }
}
