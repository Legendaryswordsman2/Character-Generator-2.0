using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

public class SaveSystem
{
    const string extention = ".json";
    public static void SaveFile<T>(string filePath, string fileName, T objectToWrite)
    {
        string json;
        //if (useNewSaveSystem)
        json = JsonConvert.SerializeObject(objectToWrite);
        //else
        //    json = JsonUtility.ToJson(objectToWrite);

        File.WriteAllTextAsync(Application.persistentDataPath + "/" + filePath + "/" + fileName + extention, json);
    }
}