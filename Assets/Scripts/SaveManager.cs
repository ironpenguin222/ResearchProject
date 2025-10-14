using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public PlayerController player;
    public List<BoxController> boxes = new List<BoxController>();

    private void Start()
    {
        foreach (GameObject boxObject in GameObject.FindGameObjectsWithTag("Box"))
        {
            BoxController box = boxObject.GetComponent<BoxController>();
            boxes.Add(box);
        }
    }

    public void SaveGame()
    {
        SaveData data = new SaveData();
        data.playerPosition = player.transform.position;
        data.boxPositions.Clear();
        foreach(BoxController box in boxes)
        {
            data.boxPositions.Add(box.transform.position);
        }
        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString("SaveData", json);
        PlayerPrefs.Save();
    }

    public void LoadGame()
    {
        if (!PlayerPrefs.HasKey("SaveData"))
        {
            return;
        }

        string json = PlayerPrefs.GetString("SaveData");
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        player.transform.position = data.playerPosition;

        for(int i = 0; i < boxes.Count && i < data.boxPositions.Count; i++)
        {
            boxes[i].transform.position = data.boxPositions[i];
        }
    }
}
