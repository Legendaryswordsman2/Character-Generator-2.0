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

    public static T LoadFile<T>(string filePath)
    {
        filePath = Application.persistentDataPath + filePath + extention;

        if (!File.Exists(filePath)) return default;

        string savedJSON = File.ReadAllText(filePath);

        if (string.IsNullOrEmpty(savedJSON) || savedJSON == "{}") return default;
        try
        {
            //return JsonUtility.FromJson<T>(savedJSON);
            return JsonConvert.DeserializeObject<T>(savedJSON);
        }
        catch (System.Exception)
        {
            //Debug.Log("Error loading JSON from file: " + filePath + "\n" + ex);
            return default;
        }
    }
}