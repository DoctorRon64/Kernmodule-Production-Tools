using System.Collections.Generic;
using UnityEngine;

public class ToolManager
{
    private int currentSelectedTool = 0;
    private GameManager gameManager;
    private readonly SpriteRenderer cursorImageRenderer;
    private readonly List<Sprite> cursorIcons;
    private bool isStopWhatPlayerIsDoing = false;
    private CustomCursor cursor;
        
    public ToolManager(SpriteRenderer _cursorImageRenderer, List<Sprite> _cursorIcons)
    {
        cursorImageRenderer = _cursorImageRenderer;
        cursorIcons = _cursorIcons;
        EventManager.AddListener<int>(EventType.SelectTool, SetCurrentSelectedTool);
        
        cursor = new CustomCursor(cursorImageRenderer);
    }

    public void UpdateCursor(Vector3 _pos)
    {
        cursor?.UpdateCursorPosition(_pos);
    }

    public int GetSelectedTool()
    {
        return currentSelectedTool;
    }

    public void SetIsStoPWhatPlayerDoing()
    {
        isStopWhatPlayerIsDoing = !isStopWhatPlayerIsDoing;
    }
    private void SetCurrentSelectedTool(int _toolIndex)
    {
        if (isStopWhatPlayerIsDoing) return;
        Cursor.visible = _toolIndex == 0;
        cursor.ChangeCursorImage(cursorIcons[_toolIndex]);
        currentSelectedTool = _toolIndex;
    }
}