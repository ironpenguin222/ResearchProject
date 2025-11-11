using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISaveable
{
    string SaveID { get; }
    ObjectSaveData SaveData(); // Able to use the save data easily
    void LoadData(ObjectSaveData data); // Able to load the sava data easily
}
