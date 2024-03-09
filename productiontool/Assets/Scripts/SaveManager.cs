using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    [SerializeField] private string fileName = "StandardSaveFile";
    private string fullPath;
    public bool overwrite = false;
    private MonoBehaviour[] allMonoBehaviours;
    
    private void Awake()
    {
        allMonoBehaviours = FindObjectsOfType<MonoBehaviour>();

        fullPath = Path.Combine(Application.isEditor ? Application.dataPath : Application.persistentDataPath, fileName + ".json");

        SaveTool(fileName);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SaveTool(fileName);
        }
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            LoadTool();
        }
    }

    private void SaveTool(string saveFileName)
    {
        foreach (MonoBehaviour monoBehaviour in allMonoBehaviours)
        {
            if (monoBehaviour is ISaveable saveable)
            {
                //for every Isaveable save!
                saveable.Save();
            }
        }
        SaveToFile();
    }

    private void SaveToFile()
    {
        if (File.Exists(fullPath))
        {
            //ask the player to overwrite the file?
        }
        bool shouldOverwrite = overwrite;
        string jsonData = JsonUtility.ToJson(GameManager.Instance.DataManager, true);

        StreamWriter writer = new StreamWriter(fullPath, shouldOverwrite);
        writer.WriteLine(jsonData);
        writer.Close();
        writer.Dispose();

        Debug.Log("Game saved to: " + fullPath);
    }

    private void LoadTool()
    {
        LoadFromFile();

        foreach (MonoBehaviour monoBehaviour in allMonoBehaviours)
        {
            if (monoBehaviour is ISaveable saveable)
            {
                //for every Isaveable load!
                saveable.Load();
            }
        }
    }
    private void LoadFromFile()
    {
        if (!File.Exists(fullPath)) return;
        
        StreamReader reader = new StreamReader(fullPath);
        GameManager.Instance.DataManager = JsonUtility.FromJson<DataManager>(reader.ReadToEnd());
        reader.Close();
        reader.Dispose();

        Debug.Log("Game loaded from: " + fullPath);
    }
}
