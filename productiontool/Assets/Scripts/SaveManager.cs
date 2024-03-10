using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveManager
{
    private readonly string fileName = "SaveFile";
    private readonly bool overwrite = false;
    private readonly List<ISaveable> savebles = new List<ISaveable>();

    private SaveManager(string _fileName)
    {
        //SaveTool(fileName);
        //LoadTool();
        //Addsaveble adds stuff to saves
    }

    public void AddSaveble(ISaveable _saveable)
    {
        savebles.Add(_saveable);
    }

    private string GetPath()
    {
        return Path.Combine(Application.isEditor ? Application.dataPath : Application.persistentDataPath, fileName + ".json");
    }
    
    private void SaveTool(string saveFileName)
    {
        string _fullpath = GetPath();
        
        if (File.Exists(_fullpath))
        {
            //ask the player to overwrite the file?
            if (!overwrite)
            {
                return;
            }
        }
        
        foreach (ISaveable _saveable in savebles)
        {
            _saveable.Save();
        }

        string jsonData = JsonUtility.ToJson(GameManager.Instance.saveFile, true);

        StreamWriter writer = new StreamWriter(_fullpath, overwrite);
        writer.WriteLine(jsonData);
        writer.Close();
        writer.Dispose();
        
        
        Debug.Log("Game saved to: " + _fullpath);
    }

    private void LoadTool()
    {
        string _fullpath = GetPath();
        
        if (!File.Exists(_fullpath))
        {
            Debug.LogError("Could not find file");
            return;
        }

        foreach (ISaveable _saveable in savebles)
        {
            _saveable.Load();
        }
        
        StreamReader reader = new StreamReader(_fullpath);
        GameManager.Instance.saveFile = JsonUtility.FromJson<SaveFile>(reader.ReadToEnd());
        reader.Close();
        reader.Dispose();

        Debug.Log("Game loaded from: " + _fullpath);
    }
}
