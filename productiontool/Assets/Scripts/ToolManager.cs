using UnityEngine;

public class ToolManager : ISaveable
{
    private int currentSelectedTool = 0;
    private GameManager gameManager; 
        
    public ToolManager(GameManager _gameManager)
    {
        this.gameManager = _gameManager;
    }
    
    public void SetCurrentSelectedTool(int _toolType)
    {
        Debug.Log(_toolType + "is the new selected tool");
        currentSelectedTool = _toolType;
    }

    public int GetSelectedTool()
    {
        return currentSelectedTool;
    }

    public void Load()
    {
        this.currentSelectedTool = gameManager.saveFile.CurrentSelectedTool;
    }

    public void Save()
    {
        gameManager.saveFile.CurrentSelectedTool = this.currentSelectedTool;
    }
}