using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; } = new();
    private ToolManager toolManager;
    public UIManager UIManager;
    public DataManager DataManager;
    
    private void Awake()
    {
        DataManager = new DataManager();
        toolManager = new ToolManager();
        UIManager = new UIManager();
        UIManager.OnToolSelected += toolManager.SetSelectedTool;
    }

    public void SetSelectedTool(ToolType _toolType)
    {
        toolManager.SetSelectedTool(_toolType);
    }
}