using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private SaveFile saveFile;
    private SaveManager saveManager;
    private ToolManager toolManager;
    private UIManager uiManager;
    private NoteManager noteManager;
    private ToolCursor cursor;
    private Timeline timeLine;

    [SerializeField] private List<Button> legacyButtonsTools = new List<Button>();
    [SerializeField] private List<Button> legacyButtonsTimeline = new List<Button>();
    [SerializeField] private List<Button> legacyButtonSaving = new List<Button>();

    [SerializeField] private SpriteRenderer cursorImageRenderer;
    [SerializeField] private List<Sprite> cursorIcons = new List<Sprite>();
    [SerializeField] private GameObject overwriteIndicator;
    
    [SerializeField] private GameObject notePrefab = null;

    private void Awake()
    {
        InitializeManagers();
        Cursor.visible = true;
        SetCurrentSelectedTool(0);
        InitializeCustomButtons();

        saveManager.AddSaveable(timeLine);
        saveManager.AddSaveable(noteManager);
    }

    private void InitializeManagers()
    {
        Instance = this;
        saveFile = new SaveFile();
        toolManager = new ToolManager();
        uiManager = new UIManager(overwriteIndicator);
        
        saveManager = new SaveManager(saveFile);
        noteManager = new NoteManager(saveFile, notePrefab);
        timeLine = new Timeline(saveFile);
        cursor = new ToolCursor(cursorImageRenderer);
    }

    private void InitializeCustomButtons()
    {
        uiManager.InitializeToolButtons(legacyButtonsTools, SetCurrentSelectedTool);
        uiManager.InitializeTimelineButtons(legacyButtonsTimeline, SetTimeline);
        uiManager.InitializeSavingButtons(legacyButtonSaving, SaveOrLoad);
    }

    private void Update()
    {
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        cursor.UpdateCursorPosition(mouseWorldPosition);

        if (toolManager.GetSelectedTool() == 2 && Input.GetMouseButtonDown(0))
        {
            noteManager.PlaceNoteAtMousePosition(mouseWorldPosition);
        }
        
        if (toolManager.GetSelectedTool() == 3 && Input.GetMouseButtonDown(0))
        {
            noteManager.RemoveNoteAtMousePosition(mouseWorldPosition);
        }
    }

    private void OnDisable()
    {
        uiManager?.RemoveListeners();
        timeLine?.RemoveListener();
    }

    private void SetTimeline(int _timelineIndex)
    {
        switch (_timelineIndex)
        {
            case 0:
                Debug.Log("Play timeline...");
                timeLine.StartTimeline();
                break;
            case 1:
                Debug.Log("Pausing timeline...");
                timeLine.PauseTimeline();
                break;
            case 2:
                Debug.Log("Stopping timeline...");
                timeLine.StopTimeline();
                break;
            case 3:
                Debug.Log("Toggling repeat timeline...");
                timeLine.ToggleRepeatTimeline();
                break;
            default:
                Debug.LogWarning("Unknown timeline index: " + _timelineIndex);
                break;
        }
    }

    private void SaveOrLoad(int _saveIndex)
    {
        if (_saveIndex == 0)
        {
            saveManager.SaveTool("save");
        }

        if (_saveIndex == 1)
        {
            saveManager.LoadTool();
        }

        if (_saveIndex == 2)
        {
            uiManager.ToggleOverwriteIndicator();
            saveManager.ToggleOverWrite();
        }
    }

    private void SetCurrentSelectedTool(int _toolIndex)
    {
        Cursor.visible = _toolIndex == 0;
        cursor.ChangeCursorImage(cursorIcons[_toolIndex]);
        toolManager?.SetCurrentSelectedTool(_toolIndex);
    }
}