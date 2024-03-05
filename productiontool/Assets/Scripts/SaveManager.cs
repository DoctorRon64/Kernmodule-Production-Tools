using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveFileOfAllData
{
    public Dictionary<string, object> saveData = new Dictionary<string, object>();
}

public interface ISaveable
{
    void Load();
    object Save();
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

        SaveTool(fileName);
    }

    public void SaveTool(string saveFileName)
    {
        SaveFileOfAllData saveFileOfAllData = new SaveFileOfAllData();
        MonoBehaviour[] allMonoBehaviours = FindObjectsOfType<MonoBehaviour>();
        try
        {
            foreach (MonoBehaviour monoBehaviour in allMonoBehaviours)
            {
                if (monoBehaviour is ISaveable saveable)
                {
                    //hasid toevoegen
                    saveFileOfAllData.saveData.Add(saveable.GetType().Name , saveable.Save());
                }
            }
        }
        catch (NullReferenceException e)
        {
            Debug.LogError("no reference found! " + e);
        }
        SaveToFile(saveFileOfAllData);
    }

    private void SaveToFile(SaveFileOfAllData saveData)
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
