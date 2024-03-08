using UnityEngine;

public enum SelectedTool
{
    None,
    Selector,
    Scale,
    Brush,
    Eraser,
}

public class ToolManager : MonoBehaviour
{
    private SelectedTool SelectedTool = SelectedTool.None;
}
