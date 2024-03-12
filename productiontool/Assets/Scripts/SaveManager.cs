using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveManager
{
    private readonly string defaultFileName = "SaveFile";
    private bool overwrite = true;
    private readonly List<ISaveable> saveables;
    private SaveFile saveFile;
    
    public SaveManager(SaveFile _saveFile)
    {
        saveables = new List<ISaveable>();
        saveFile = _saveFile;
    }
    
    public void AddSaveable(ISaveable _saveable)
    {
        saveables.Add(_saveable);
    }

    public void ToggleOverWrite()
    {
        overwrite = !overwrite;
    }

    private string GetPath(string _fileName)
    {
        return Path.Combine(Application.isEditor ? Application.dataPath : Application.persistentDataPath, _fileName + ".json");
    }

    public void SaveTool(string _saveFileName)
    {
        string fullpath = GetPath(_saveFileName);
        
        // If file exists and overwrite give warning
        if (File.Exists(fullpath) && overwrite)
        {
            Debug.LogWarning("Save file already exists. Set overwrite to true to overwrite the existing file.");
        }
        
        foreach (ISaveable saveable in saveables)
        {
            saveable.Save();
        }

        string jsonData = JsonUtility.ToJson(saveFile, true);
        StreamWriter writer = new StreamWriter(fullpath, !overwrite);
        writer.WriteLine(jsonData);
        writer.Close();
        writer.Dispose();
        
        Debug.Log("Game saved to: " + fullpath);
    }

    public void LoadTool(string _saveFileName)
    {
        string fullpath = GetPath(_saveFileName);
        
        if (!File.Exists(fullpath))
        {
            Debug.LogWarning("Save file not found.");
            return;
        }
        
        StreamReader reader = new StreamReader(fullpath);
        string jsonData = reader.ReadToEnd();
        reader.Close();
        reader.Dispose();

        saveFile = JsonUtility.FromJson<SaveFile>(jsonData);

        foreach (ISaveable _saveable in saveables)
        {
            _saveable.Load();
        }
        
        Debug.Log("Game loaded from: " + fullpath);
    }
}
