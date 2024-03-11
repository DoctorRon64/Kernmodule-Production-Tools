using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [field: Header("Managers")]
    public static GameManager Instance { get; private set; }
    public SaveFile saveFile;
    private SaveManager saveManager;
    private ToolManager toolManager;
    private UIManager uiManager;
    private ToolCursor cursor;

    [Header("ToolButtons")] [SerializeField]
    private List<Button> buttons = new List<Button>();
    private readonly List<ToolButton> toolButtons = new List<ToolButton>();

    [Header("CursorIcons")] 
    [SerializeField] private SpriteRenderer cursorImageRenderer;
    [SerializeField] private List<Sprite> cursorIcons = new List<Sprite>(); 
    
    private void Awake()
    {
        InitializeManagers();
        
        for (var i = 0; i < buttons.Count; i++)
        {
            toolButtons.Add(new ToolButton(buttons[i], i));
        }
        
        uiManager.InitializeToolButtons(toolButtons, SetCurrentSelectedTool);
        saveManager.AddSaveble(toolManager);
        
    }

    private void InitializeManagers()
    {
        Instance = this;
        saveFile = new SaveFile();
        toolManager = new ToolManager(Instance);
        saveManager = new SaveManager();
        uiManager = new UIManager(Instance);
        cursor = new ToolCursor(cursorImageRenderer);
    }

    private void Update()
    {
        var mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        cursor.UpdateCursorPosition(mouseWorldPosition);
    }

    private void OnDisable()
    {
        uiManager?.RemoveListeners();
    }
    
    private void SetCurrentSelectedTool(int _toolIndex)
    {
        cursor.ChangeCursorImage(cursorIcons[_toolIndex]);
        toolManager?.SetCurrentSelectedTool(_toolIndex);
    }
}