using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public interface ISaveable
{
    Dictionary<string, object> SaveData();
}

public class SaveManager : MonoBehaviour
{
    [SerializeField] private string fileName = "StandardSaveFile";
    private string fullPath;
    public bool overwrite = false;

    private void Awake()
    {
        if (Application.isEditor)
        {
            fullPath = Path.Combine(Application.dataPath, fileName + ".json");
        }
        else
        {
            fullPath = Path.Combine(Application.persistentDataPath, fileName + ".json");
        }

        Save(fileName);
    }

    public void Save(string saveFileName)
    {
        List<Dictionary<string, object>> saveDataList = new List<Dictionary<string, object>>();
        MonoBehaviour[] allMonoBehaviours = FindObjectsOfType<MonoBehaviour>();
        try
        {
            foreach (MonoBehaviour monoBehaviour in allMonoBehaviours)
            {
                if (monoBehaviour is ISaveable saveable)
                {
                    saveDataList.Add(saveable.SaveData());
                    SaveToFile(saveDataList);
                }
            }
        }
        catch (NullReferenceException e)
        {
            Debug.LogError("no reference found! " + e);
        }
    }

    private void SaveToFile<T>(T saveData)
    {
        //!File.Exists(fullPath);
        //if the file already excists ask the player if he wants to overwrite the save file
        bool shouldOverwrite = overwrite;

        string jsonData = JsonUtility.ToJson(saveData);
        using (StreamWriter writer = new StreamWriter(fullPath, shouldOverwrite))
        {
            writer.WriteLine(jsonData);
            Debug.Log("Game saved to: " + fullPath);
        }
    }

}
