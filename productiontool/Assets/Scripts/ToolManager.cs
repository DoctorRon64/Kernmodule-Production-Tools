using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SelectedTool {
    None,
    Selector,
    Scale,
    Brush,
    Eraser,
}

public class ToolManager
{
    private SelectedTool selectedTool = SelectedTool.None;
}
