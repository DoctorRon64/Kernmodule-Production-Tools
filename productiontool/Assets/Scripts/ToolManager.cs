using UnityEngine;

public class ToolManager
{
    private int currentSelectedTool = 0;
    private GameManager gameManager;
    
    public void SetCurrentSelectedTool(int _toolType)
    {
        currentSelectedTool = _toolType;
    }

    public int GetSelectedTool()
    {
        return currentSelectedTool;
    }
}