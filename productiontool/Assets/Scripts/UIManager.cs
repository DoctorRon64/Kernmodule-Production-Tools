using System;
using UnityEngine;

public class UIManager
{
    public event Action<ToolType> OnToolSelected;

    public void SelectTool(ToolType _toolType) {
        OnToolSelected?.Invoke(_toolType);
        Debug.Log(_toolType);
    }
}

public enum ToolType
{
    None,
    Selector,
    Scale,
    Brush,
    Eraser,
}

public class ToolManager
{
    public ToolType ToolType = ToolType.None;
    public void SetSelectedTool(ToolType _toolType)
    {
        ToolType = _toolType;
    }
}