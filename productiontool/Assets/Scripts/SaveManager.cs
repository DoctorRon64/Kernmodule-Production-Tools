using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveManager
{
    private readonly string fileName = "SaveFile";
    private readonly bool overwrite = false;
    private readonly List<ISaveable> saveables = new List<ISaveable>();

    public void AddSaveable(ISaveable _saveable)
    {
        saveables.Add(_saveable);
    }

    private string GetPath()
    {
        return Path.Combine(Application.isEditor ? Application.dataPath : Application.persistentDataPath, fileName + ".json");
    }

    public void SaveTool(string _saveFileName)
    {
        string fullpath = GetPath();
        
        // If file exists and overwrite is false, do not save
        if (File.Exists(fullpath) && !overwrite)
        {
            Debug.LogWarning("Save file already exists. Set overwrite to true to overwrite the existing file.");
            return;
        }
        
        foreach (ISaveable saveable in saveables)
        {
            saveable.Save();
        }

        string jsonData = JsonUtility.ToJson(GameManager.Instance.saveFile, true);

        StreamWriter writer = new StreamWriter(fullpath, overwrite);
        writer.WriteLine(jsonData);
        writer.Close();
        writer.Dispose();
        
        Debug.Log("Game saved to: " + fullpath);
    }

    public void LoadTool()
    {
        string fullpath = GetPath();
        
        if (!File.Exists(fullpath))
        {
            Debug.LogWarning("Save file not found.");
            return;
        }

        StreamReader reader = new StreamReader(fullpath);
        string jsonData = reader.ReadToEnd();
        reader.Close();
        reader.Dispose();

        GameManager.Instance.saveFile = JsonUtility.FromJson<SaveFile>(jsonData);

        foreach (ISaveable _saveable in saveables)
        {
            _saveable.Load();
        }
        
        Debug.Log("Game loaded from: " + fullpath);
    }
}
