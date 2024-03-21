using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : ISaveSettings, ISaveable
{
    private readonly List<Button> legacyButtonsTimeline;
    private readonly List<Button> legacyButtonsTools;
    private readonly List<Button> legacyButtonsSaving;
    
    private readonly GameObject overwriteIndicator;
    private readonly GameObject loopIndicator;
    private readonly Slider timeLineSlider;
    private readonly TMP_InputField bpmField;
    private readonly TMP_Dropdown dropdownSampleRate;
    private readonly Toggle fullscreenToggle;
    private bool fullscreenOnOrOff;
    private GameManager gameManager;
    
    public UIManager(GameManager _gameManger,
        List<Button> _legacyButtonsTools, List<Button> _legacyButtonsTimeline, List<Button> _legacyButtonSaving,
        List<Action<int>> _allActions, GameObject _overwriteIndicator, GameObject _loopIndicator, Slider _timeLineSlider,  TMP_InputField _bmpInputField,
        TMP_Dropdown _sampleRate, Toggle _fullScreenToggle
        )
    {
        timeLineSlider = _timeLineSlider;
        gameManager = _gameManger;
        loopIndicator = _loopIndicator;
        overwriteIndicator = _overwriteIndicator;
        bpmField = _bmpInputField;
        dropdownSampleRate = _sampleRate;
        fullscreenToggle = _fullScreenToggle;

        legacyButtonsTools = _legacyButtonsTools;
        legacyButtonsTimeline = _legacyButtonsTimeline;
        legacyButtonsSaving = _legacyButtonSaving;
        
        InitializeToolButtons(legacyButtonsTools);
        InitializeButtons(legacyButtonsTimeline, _allActions[0]);
        InitializeButtons(legacyButtonsSaving, _allActions[1]);

        EventManager.AddListener<int>(EventType.TimerElapse, UpdateTimelineSlider);
        EventManager.Parameterless.AddListener(EventType.Repeat, ToggleLoopIndicator);
        EventManager.Parameterless.AddListener(EventType.OverwriteToggle, ToggleOverwriteIndicator);

        fullscreenToggle.onValueChanged.AddListener(FullscreenToggle);
        dropdownSampleRate.onValueChanged.AddListener(SampleRateChanged);
        bpmField.onValueChanged.AddListener(BpmChanged);
        bpmField.text = 60.ToString();
    }

    public void Load(SettingsFile _load)
    {
        overwriteIndicator.SetActive(_load.DoesPlayerWantOverwritePopUp);
        loopIndicator.SetActive(_load.RepeatTimeline);
        dropdownSampleRate.value = _load.SampleRate;
        fullscreenToggle.isOn = _load.FullscreenToggle;
        
        FullscreenToggle(_load.FullscreenToggle);
    }

    public void Save(SettingsFile _save)
    {
        _save.FullscreenToggle = fullscreenOnOrOff;
    }

    private void BpmChanged(string _value)
    {
        int integerBpm;
        if (_value == "") integerBpm = 0;
        integerBpm =  Int32.Parse(_value);
        EventManager.InvokeEvent(EventType.Bpm, integerBpm);
    }

    private void SampleRateChanged(int _value)
    {
        EventManager.InvokeEvent(EventType.SampleRate, dropdownSampleRate.value);
    }

    private void FullscreenToggle(bool _toggle)
    {
        fullscreenOnOrOff = _toggle;
        Screen.fullScreen = fullscreenOnOrOff;
    }

    private void ToggleOverwriteIndicator()
    {
        overwriteIndicator.SetActive(!overwriteIndicator.activeSelf);
    }

    private void ToggleLoopIndicator()
    {
        loopIndicator.SetActive(!loopIndicator.activeSelf);
    }

    private void UpdateTimelineSlider(int _newTime)
    {
        lock (gameManager.actionQueue)
        {
            gameManager.actionQueue.Enqueue(() => UpdateValue(_newTime - 1));
        }
    }

    void UpdateValue(float _value)
    {
        timeLineSlider.value = _value;
    }

    private void InitializeButtons(List<Button> _buttons, Action<int> _onClickCallback)
    {
        for (int i = 0; i < _buttons.Count; i++)
        {
            int buttonIndex = i;
            _buttons[i].onClick.AddListener(() => _onClickCallback(buttonIndex));
        }
    }
    
    private void InitializeToolButtons(List<Button> _buttons)
    {
        for (int i = 0; i < _buttons.Count; i++)
        {
            int buttonIndex = i;
            _buttons[i].onClick.AddListener(() => EventManager.InvokeEvent(EventType.SelectTool, buttonIndex));
        }
    }


    public void RemoveListeners()
    {
        RemoveListenersFromButtons(legacyButtonsTools);
        RemoveListenersFromButtons(legacyButtonsTimeline);
        RemoveListenersFromButtons(legacyButtonsSaving);
        
        bpmField.onValueChanged.RemoveAllListeners();
        dropdownSampleRate.onValueChanged.RemoveAllListeners();
    }
    
    private void RemoveListenersFromButtons(List<Button> _buttons)
    {
        foreach (var button in _buttons)
        {
            button.onClick.RemoveAllListeners();
        }
    }

    public void Load(SaveFile _load)
    {
        bpmField.text = _load.BPM.ToString();
    }

    public void Save(SaveFile _save)
    {
        
    }
}