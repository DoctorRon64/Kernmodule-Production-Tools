using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;

[Serializable]
public class SaveFileOfAllData
{
    public Dictionary<string, SaveFile> saveData = new Dictionary<string, SaveFile>();
}

[Serializable]
public class SaveFile
{
    public List<object> saveData = new List<object>();
}

public interface ISaveable
{
    void Load();
    SaveFile Save();
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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SaveTool(fileName);
        }
    }

    private string GenerateId()
    {
        return Guid.NewGuid().ToString();
    }

    public void SaveTool(string saveFileName)
    {
        SaveFileOfAllData saveFileOfAllData = new SaveFileOfAllData();
        MonoBehaviour[] allMonoBehaviours = FindObjectsOfType<MonoBehaviour>();

        foreach (MonoBehaviour monoBehaviour in allMonoBehaviours)
        {
            if (monoBehaviour is ISaveable saveable)
            {
                //hasid toevoegen
                saveFileOfAllData.saveData.Add(GenerateId(), saveable.Save());
            }
        }

        SaveToFile(saveFileOfAllData);
    }

    private void SaveToFile(SaveFileOfAllData saveData)
    {
        //!File.Exists(fullPath);
        //if the file already excists ask the player if he wants to overwrite the save file
        bool shouldOverwrite = overwrite;

        string jsonData = JsonUtility.ToJson(saveData);

        StreamWriter writer = new StreamWriter(fullPath, shouldOverwrite);
        writer.WriteLine(jsonData);
        writer.Close();
        writer.Dispose();

        Debug.Log("Game saved to: " + fullPath);
    }
}
