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
    private InputManager inputManager;
    private AudioManager audioManager;
    private Timeline timeLine;
    private CustomPopup overwriteConfirmationPopup;

    public Queue<Action> actionQueue = new Queue<Action>();

    private bool isStopWhatPlayerIsDoing = false;

    [Header("Buttons")] [SerializeField] private List<Button> legacyButtonsTools = new List<Button>();
    [SerializeField] private List<Button> legacyButtonsTimeline = new List<Button>();
    [SerializeField] private List<Button> legacyButtonSaving = new List<Button>();

    [Header("UI")] [SerializeField] private TMP_InputField saveFileInputField;
    [SerializeField] private GameObject overwriteIndicator;
    [SerializeField] private GameObject loopTimelineIndicator;
    [SerializeField] private GameObject popUp;
    [SerializeField] private TMP_InputField bpmInputField;
    [SerializeField] private TMP_Dropdown sampleRateDropdown;
    [SerializeField] private Toggle fullScreenToggle;

    [Header("Cursors")] [SerializeField] private SpriteRenderer cursorImageRenderer;
    [SerializeField] private List<Sprite> cursorIcons = new List<Sprite>();

    [Header("Timeline")] [SerializeField] private Slider timeLineSlider;
    [SerializeField] private GameObject notePrefab;
    [SerializeField] private Transform allNotesParents;

    [Header("Audio")] [SerializeField] private AudioSource[] audioSource;

    private void Awake()
    {
        InitializeManagers();
        EventManager.InvokeEvent(EventType.SelectTool, 0);

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
            new List<Action<int>> { SetTimeline, SaveOrLoad },
            overwriteIndicator, loopTimelineIndicator, timeLineSlider, bpmInputField, sampleRateDropdown,
            fullScreenToggle
        );
        audioManager = new AudioManager(audioSource);
        noteManager = new NoteManager(Instance, audioManager, notePrefab, allNotesParents);
        saveManager = new SaveManager(Instance, noteManager);
        toolManager = new ToolManager(cursorImageRenderer, cursorIcons);
        overwriteConfirmationPopup = new CustomPopup(popUp, Instance);
        inputManager  = new InputManager(noteManager, toolManager);
    }

    private void Update()
    {
        // execute task queue
        lock (actionQueue)
        {
            foreach (Action a in actionQueue)
            {
                a.Invoke();
            }
        }
        actionQueue.Clear();
        
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        toolManager?.UpdateCursor(mouseWorldPosition);
        inputManager?.Update(mouseWorldPosition);
    }

    public int GetTimelineBpm()
    {
        return timeLine.GetBpm();
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
            case 4:
                saveManager.ExportToFile();
                break;
            default:
                Debug.LogWarning("Unknown save index: " + _saveIndex);
                break;
        }
    }

    public void TogglePlayerStopDoing()
    {
        isStopWhatPlayerIsDoing = !isStopWhatPlayerIsDoing;
        toolManager?.SetIsStoPWhatPlayerDoing();
    }

    public void HandleOverwriteConfirmation(string _fileName)
    {
        saveFileInputField.interactable = false;
        EventManager.InvokeEvent(EventType.SelectTool, 0);
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