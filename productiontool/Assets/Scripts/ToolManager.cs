using UnityEngine;

public class ToolManager
{
    private int currentSelectedTool = 0;
    private GameManager gameManager;
    
    public void SetCurrentSelectedTool(int _toolType)
    {
        Debug.Log(_toolType + "is the new selected tool");
        currentSelectedTool = _toolType;
    }

    public int GetSelectedTool()
    {
        return currentSelectedTool;
    }
}