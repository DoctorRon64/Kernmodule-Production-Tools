using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : ISaveSettings, ISaveable
{
    private readonly List<CustomButton> toolButtons = new List<CustomButton>();
    private readonly List<CustomButton> timelineButtons = new List<CustomButton>();
    private readonly List<CustomButton> savingButtons = new List<CustomButton>();
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
        
        InitializeButtons(_legacyButtonsTools, _allActions[0], toolButtons);
        InitializeButtons(_legacyButtonsTimeline, _allActions[1], timelineButtons);
        InitializeButtons(_legacyButtonSaving, _allActions[2], savingButtons);

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

    private void InitializeButtons(List<Button> _buttons, Action<int> _onClickCallback, List<CustomButton> _buttonList)
    {
        foreach (var button in _buttons)
        {
            var customButton = new CustomButton(button, _buttonList.Count);
            customButton.AddListener(_onClickCallback);
            _buttonList.Add(customButton);
        }
    }

    public void RemoveListeners()
    {
        RemoveListenersFromButtons(toolButtons);
        RemoveListenersFromButtons(timelineButtons);
        RemoveListenersFromButtons(savingButtons);
        bpmField.onValueChanged.RemoveAllListeners();
        dropdownSampleRate.onValueChanged.RemoveAllListeners();
    }

    private void RemoveListenersFromButtons(List<CustomButton> _buttons)
    {
        foreach (var button in _buttons)
        {
            button.RemoveAllListeners();
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