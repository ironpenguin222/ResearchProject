using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;

public class LevelExporter
{
    [MenuItem("Tools/Export Level")]
    public static void ExportLevel()
    {
        SaveData data = new SaveData();

        GameObject playerObj = GameObject.FindWithTag("Player"); // Player position saved
        if (playerObj != null)
            data.playerPosition = playerObj.transform.position;

        data.objectData = new List<ObjectSaveData>(); // Save all objects
        ISaveable[] allSaveables = GameObject.FindObjectsOfType<MonoBehaviour>(true)
            .OfType<ISaveable>()
            .ToArray();

        foreach (var obj in allSaveables)
            data.objectData.Add(obj.SaveData());

        string levelDir = Path.Combine(Application.streamingAssetsPath, "Levels"); // Level Folder
        Directory.CreateDirectory(levelDir);

        string defaultName = "NewLevel.json"; // Get level name

        string savePath = EditorUtility.SaveFilePanel(
            "Export Level",
            levelDir,      // The folder
            defaultName,   // Default file name
            "json"         // File extension
        );

        if (string.IsNullOrEmpty(savePath)) // If User cancelled
            return;

        string json = JsonUtility.ToJson(data, true); // Save json

        File.WriteAllText(savePath, json);

        Debug.Log($"Level exported to: {savePath}");
    }
}


