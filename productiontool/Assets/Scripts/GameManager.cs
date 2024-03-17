using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private static GameManager Instance { get; set; }
    private SaveManager saveManager;
    private ToolManager toolManager;
    private UIManager uiManager;
    private NoteManager noteManager;
    private AudioManager audioManager;
    
    private CustomCursor cursor;
    private Timeline timeLine;
    private CustomPopup overwriteConfirmationPopup;

    private bool isStopWhatPlayerIsDoing = false;

    [Header("Buttons")] 
    [SerializeField] private List<Button> legacyButtonsTools = new List<Button>();
    [SerializeField] private List<Button> legacyButtonsTimeline = new List<Button>();
    [SerializeField] private List<Button> legacyButtonSaving = new List<Button>();
    [SerializeField] private TMP_InputField saveFileInputField;
    [SerializeField] private GameObject overwriteIndicator;
    [SerializeField] private GameObject loopTimelineIndicator;
    [SerializeField] private GameObject popUp;
    [SerializeField] private TMP_InputField bpmInputField;
    [SerializeField] private TMP_Dropdown sampleRateDropdown;
    
    [Header("Cursors")] 
    [SerializeField] private SpriteRenderer cursorImageRenderer;
    [SerializeField] private List<Sprite> cursorIcons = new List<Sprite>();

    [Header("Timeline")] 
    [SerializeField] private Slider timeLineSlider;
    [SerializeField] private GameObject notePrefab;
    [SerializeField] private Transform allNotesParents;

    [Header("Audio")] [SerializeField] private AudioSource audioSource;

    private void Awake()
    {
        InitializeManagers();
        SetCurrentSelectedTool(0);

        foreach (var field in typeof(GameManager).GetFields(
                     System.Reflection.BindingFlags.NonPublic | 
                     System.Reflection.BindingFlags.Instance | 
                     System.Reflection.BindingFlags.DeclaredOnly))
        {
            if (typeof(ISaveable).IsAssignableFrom(field.FieldType))
            {
                ISaveable saveable = (ISaveable)field.GetValue(this);
                saveManager.AddSaveable(saveable);
            }

            if (typeof(ISaveSettings).IsAssignableFrom(field.FieldType))
            {
                ISaveSettings settings = (ISaveSettings)field.GetValue(this);
                saveManager.AddSettings(settings);
            }
        }
        saveManager.LoadSettings();
    }

    private void OnDisable()
    {
        //want anders word valentijn boos
        saveManager?.SaveSettings();
        
        timeLine?.RemoveListener();
        uiManager?.RemoveListeners();
        EventManager.RemoveAllListeners();
    }

    private void InitializeManagers()
    {
        Instance = this;
        timeLine = new Timeline(Instance);

       
        uiManager = new UIManager(Instance,
            legacyButtonsTools, legacyButtonsTimeline, legacyButtonSaving,
            new List<Action<int>> { SetCurrentSelectedTool, SetTimeline, SaveOrLoad },
            overwriteIndicator, loopTimelineIndicator, timeLineSlider, bpmInputField, sampleRateDropdown
        );
        saveManager = new SaveManager(Instance);
        audioManager = new AudioManager(audioSource);
        toolManager = new ToolManager();
        noteManager = new NoteManager(Instance, audioManager, notePrefab, allNotesParents);
        cursor = new CustomCursor(cursorImageRenderer);
        overwriteConfirmationPopup = new CustomPopup(popUp, Instance);
    }
    
    private void Update()
    {
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        cursor.UpdateCursorPosition(mouseWorldPosition);

        if (isStopWhatPlayerIsDoing) return;

        if (toolManager?.GetSelectedTool() == 1 && Input.GetMouseButton(0))
            noteManager?.PlaceOrRemoveNoteAtMousePosition(mouseWorldPosition, true);

        if (toolManager?.GetSelectedTool() == 2 && Input.GetMouseButton(0))
            noteManager?.PlaceOrRemoveNoteAtMousePosition(mouseWorldPosition, false);
    }

    public void SetCurrentSelectedTool(int _toolIndex)
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
            case 0:
                timeLine.StartTimeline();
                break;
            case 1:
                timeLine.PauseTimeline();
                break;
            case 2:
                timeLine.StopTimeline();
                break;
            case 3:
                EventManager.Parameterless.InvokeEvent(EventType.Repeat);
                break;
            default:
                Debug.LogWarning("Unknown timeline index: " + _timelineIndex);
                break;
        }
    }

    private void SaveOrLoad(int _saveIndex)
    {
        if (isStopWhatPlayerIsDoing) return;
        switch (_saveIndex)
        {
            case 0:
                saveManager.SaveTool(saveFileInputField.text, true);
                break;
            case 1:
                saveManager.LoadTool(saveFileInputField.text);
                break;
            case 2:
                EventManager.Parameterless.InvokeEvent(EventType.OverwriteToggle);
                break;
            case 3:
                noteManager.ClearAllNotes();
                saveFileInputField.text = "";
                break;
            default:
                Debug.LogWarning("Unknown save index: " + _saveIndex);
                break;
        }
    }

    public void TogglePlayerStopDoing()
    {
        isStopWhatPlayerIsDoing = !isStopWhatPlayerIsDoing;
    }

    public void HandleOverwriteConfirmation(string _fileName)
    {
        saveFileInputField.interactable = false;
        SetCurrentSelectedTool(0);
        TogglePlayerStopDoing();
        overwriteConfirmationPopup.ShowConfirmationPopup(
            () =>
            {
                TogglePlayerStopDoing();
                saveFileInputField.interactable = true;
                saveManager.SaveTool(_fileName, false);
            },
            () =>
            {
                saveFileInputField.interactable = true;
                TogglePlayerStopDoing();
            }
        );
    }
}