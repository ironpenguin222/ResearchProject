using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISaveable
{
    ObjectSaveData SaveData();
    void LoadData(ObjectSaveData data);
}
