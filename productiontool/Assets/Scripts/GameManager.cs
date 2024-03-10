using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [field: Header("Managers")]
    public static GameManager Instance { get; private set; }
    public SaveFile saveFile;
    private ToolManager toolManager;
    private UIManager uiManager;

    [Header("ToolButtons")] [SerializeField]
    private List<Button> buttons = new List<Button>();
    private readonly List<ToolButton> toolButtons = new List<ToolButton>();
    
    private void Awake()
    {
        Instance = this;
        saveFile = new SaveFile();
        toolManager = new ToolManager();
        uiManager = new UIManager(Instance);

        for (var i = 0; i < buttons.Count; i++)
        {
            toolButtons.Add(new ToolButton(buttons[i], i));
        }
        
        uiManager.InitializeToolButtons(toolButtons, SetCurrentSelectedTool);
    }
    private void OnDisable()
    {
        uiManager?.RemoveListeners();
    }
    
    private void SetCurrentSelectedTool(int _toolIndex)
    {
        toolManager?.SetCurrentSelectedTool(_toolIndex);
    }
}