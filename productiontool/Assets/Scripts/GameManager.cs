using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private static GameManager Instance { get; set; }
    private SaveManager saveManager;
    private ToolManager toolManager;
    private UIManager uiManager;
    private NoteManager noteManager;
    private AudioManager audioManager;
    public SaveFile SaveFile;
    
    private CustomCursor cursor;
    private Timeline timeLine;
    CustomPopup overwriteConfirmationPopup;
    
    private bool isStopWhatPlayerIsDoing = false;
    
    [Header("Buttons")]
    [SerializeField] private List<Button> legacyButtonsTools = new List<Button>();
    [SerializeField] private List<Button> legacyButtonsTimeline = new List<Button>();
    [SerializeField] private List<Button> legacyButtonSaving = new List<Button>();
    [SerializeField] private TMP_InputField saveFileInputField;
    [SerializeField] private GameObject overwriteIndicator;
    [SerializeField] private GameObject loopTimelineIndicator;
    
    [Header("Popup")]
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button declineButton;
    [SerializeField] private GameObject popUp;
    
    [Header("Cursors")]
    [SerializeField] private SpriteRenderer cursorImageRenderer;
    [SerializeField] private List<Sprite> cursorIcons = new List<Sprite>();
    
    [Header("notes")]
    [SerializeField] private GameObject notePrefab = null;
    [SerializeField] private Transform allNotesParents = null;

    [Header("Timeline")] 
    [SerializeField] private Slider progressBarTimeline;

    [Header("Audio")] 
    [SerializeField] private AudioSource audioSource;

    private void Awake()
    {
        InitializeManagers();
        SetCurrentSelectedTool(0);
        InitializeCustomButtons();
        
        timeLine.OnTimeLineElapsed += uiManager.UpdateTimelineSlider;
        timeLine.OnTimeLineElapsed += noteManager.PlayNotesAtPosition;
        
        if (saveManager == null) return;
        saveManager.OnOverwriteConfirmation += HandleOverwriteConfirmation;
        saveManager.AddSaveable(timeLine);
        saveManager.AddSaveable(noteManager);
    }

    private void OnDisable()
    {
        if (saveManager != null) saveManager.OnOverwriteConfirmation -= HandleOverwriteConfirmation;
        if (timeLine != null) timeLine.OnTimeLineElapsed -= uiManager!.UpdateTimelineSlider;
        timeLine?.RemoveListener();
        uiManager?.RemoveListeners();
    }
    
    private void InitializeManagers()
    {
        Instance = this;
        SaveFile = new SaveFile();
        
        toolManager = new ToolManager();
        audioManager = new AudioManager(audioSource);
        uiManager = new UIManager(overwriteIndicator, loopTimelineIndicator ,progressBarTimeline);
        saveManager = new SaveManager(Instance);
        noteManager = new NoteManager(Instance, audioManager, notePrefab, allNotesParents);
        timeLine = new Timeline(Instance);
        cursor = new CustomCursor(cursorImageRenderer);
        overwriteConfirmationPopup = new CustomPopup(popUp, confirmButton, declineButton, Instance);
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

        if (isStopWhatPlayerIsDoing) return;
        
        if (toolManager?.GetSelectedTool() == 1 && Input.GetMouseButton(0))
        {
            noteManager?.PlaceOrRemoveNoteAtMousePosition(mouseWorldPosition, true);
        }
        
        if (toolManager?.GetSelectedTool() == 2 && Input.GetMouseButton(0))
        {
            noteManager?.PlaceOrRemoveNoteAtMousePosition(mouseWorldPosition, false);
        }
    }

    private void SetCurrentSelectedTool(int _toolIndex)
    {
        if (isStopWhatPlayerIsDoing) return;
        Cursor.visible = _toolIndex == 0;
        cursor.ChangeCursorImage(cursorIcons[_toolIndex]);
        toolManager?.SetCurrentSelectedTool(_toolIndex);
    }

    private void SetTimeline(int _timelineIndex)
    {
        if (isStopWhatPlayerIsDoing) return;
        if (timeLine == null) return;
        switch (_timelineIndex)
        {
            case 0: timeLine.StartTimeline(); break;
            case 1: timeLine.PauseTimeline(); break;
            case 2: timeLine.StopTimeline(); break;
            case 3: timeLine.ToggleRepeatTimeline(); uiManager.ToggleLoopIndicator(); break;
            default: Debug.LogWarning("Unknown timeline index: " + _timelineIndex); break;
        }
    }

    private void SaveOrLoad(int _saveIndex)
    {
        if (isStopWhatPlayerIsDoing) return;
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

    public void TogglePlayerStopDoing()
    {
        isStopWhatPlayerIsDoing = !isStopWhatPlayerIsDoing;
    }
    
    private void HandleOverwriteConfirmation(string _fileName)
    {
        TogglePlayerStopDoing();
        overwriteConfirmationPopup.ShowConfirmationPopup(
            () =>
            {
                TogglePlayerStopDoing();
                saveManager.OverwriteSaveFile(_fileName);
            },
            () =>
            {
                TogglePlayerStopDoing();
            }
        );
    }
}