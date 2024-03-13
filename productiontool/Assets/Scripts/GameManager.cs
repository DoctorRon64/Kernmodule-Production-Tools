using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private static GameManager Instance { get; set; }
    public SaveFile SaveFile;
    private SaveManager saveManager;
    private ToolManager toolManager;
    private UIManager uiManager;
    private NoteManager noteManager;
    private CustomCursor cursor;
    private Timeline timeLine;

    [Header("Buttons")]
    [SerializeField] private List<Button> legacyButtonsTools = new List<Button>();
    [SerializeField] private List<Button> legacyButtonsTimeline = new List<Button>();
    [SerializeField] private List<Button> legacyButtonSaving = new List<Button>();
    [SerializeField] private GameObject overwriteIndicator;
    [SerializeField] private TMP_InputField saveFileInputField;
    
    [Header("Cursors")]
    [SerializeField] private SpriteRenderer cursorImageRenderer;
    [SerializeField] private List<Sprite> cursorIcons = new List<Sprite>();
    
    [Header("notes")]
    [SerializeField] private GameObject notePrefab = null;
    [SerializeField] private Transform allNotesParents = null;

    private void Awake()
    {
        InitializeManagers();
        SetCurrentSelectedTool(0);
        InitializeCustomButtons();

        saveManager.AddSaveable(timeLine);
        saveManager.AddSaveable(noteManager);
    }

    private void InitializeManagers()
    {
        Instance = this;
        SaveFile = new SaveFile();
        toolManager = new ToolManager();
        
        uiManager = new UIManager(overwriteIndicator);
        saveManager = new SaveManager(Instance);
        noteManager = new NoteManager(Instance, notePrefab, allNotesParents);
        timeLine = new Timeline(Instance);
        cursor = new CustomCursor(cursorImageRenderer);
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

        if (toolManager?.GetSelectedTool() == 2 && Input.GetMouseButton(0))
        {
            noteManager?.PlaceNoteAtMousePosition(mouseWorldPosition);
        }
        
        if (toolManager?.GetSelectedTool() == 3 && Input.GetMouseButton(0))
        {
            noteManager?.RemoveNoteAtMousePosition(mouseWorldPosition);
        }
    }

    private void OnDisable()
    {
        uiManager?.RemoveListeners();
        timeLine?.RemoveListener();
    }
    
    private void SetCurrentSelectedTool(int _toolIndex)
    {
        Cursor.visible = _toolIndex == 0;
        cursor.ChangeCursorImage(cursorIcons[_toolIndex]);
        toolManager?.SetCurrentSelectedTool(_toolIndex);
    }

    private void SetTimeline(int _timelineIndex)
    {
        switch (_timelineIndex)
        {
            case 0: timeLine.StartTimeline(); break;
            case 1: timeLine.PauseTimeline(); break;
            case 2: timeLine.StopTimeline(); break;
            case 3: timeLine.ToggleRepeatTimeline(); break;
            default: Debug.LogWarning("Unknown timeline index: " + _timelineIndex); break;
        }
    }

    private void SaveOrLoad(int _saveIndex)
    {
        switch (_saveIndex)
        {
            case 0: 
                saveManager.SaveTool(saveFileInputField.text); 
                break;
            case 1: 
                saveManager.LoadTool(saveFileInputField.text); 
                break;
            case 2: uiManager.ToggleOverwriteIndicator(); 
                saveManager.ToggleOverWrite(); 
                break;
            case 3: noteManager.ClearAllNotes();
                saveFileInputField.text = "";
                break;
            default: Debug.LogWarning("Unknown save index: " + _saveIndex); break;
        }
    }
}