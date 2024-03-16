using System;
using System.Collections.Generic;
using TMPro;
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
    private GameManager gameManager;
    
    public UIManager(GameManager _gameManger,
        List<Button> _legacyButtonsTools, List<Button> _legacyButtonsTimeline, List<Button> _legacyButtonSaving,
        List<Action<int>> _allActions, GameObject _overwriteIndicator, GameObject _loopIndicator, Slider _timeLineSlider,  TMP_InputField _bmpInputField,
        TMP_Dropdown _sampleRate
        )
    {
        timeLineSlider = _timeLineSlider;
        gameManager = _gameManger;
        loopIndicator = _loopIndicator;
        overwriteIndicator = _overwriteIndicator;
        bpmField = _bmpInputField;
        dropdownSampleRate = _sampleRate;
        
        InitializeButtons(_legacyButtonsTools, _allActions[0], toolButtons);
        InitializeButtons(_legacyButtonsTimeline, _allActions[1], timelineButtons);
        InitializeButtons(_legacyButtonSaving, _allActions[2], savingButtons);

        EventManager.AddListener<int>(EventType.TimerElapse, UpdateTimelineSlider);
        dropdownSampleRate.onValueChanged.AddListener(SampleRateChanged);
        bpmField.text = 60.ToString();
        bpmField.onValueChanged.AddListener(BpmChanged);
        overwriteIndicator.SetActive(true);
    }

    public void Load(SettingsFile _load)
    {
        overwriteIndicator.SetActive(_load.DoesPlayerWantOverwritePopUp);
        loopIndicator.SetActive(_load.RepeatTimeline);
    }

    public void Save(SettingsFile _save)
    {
        _save.RepeatTimeline = loopIndicator.activeSelf;
        _save.DoesPlayerWantOverwritePopUp = overwriteIndicator.activeSelf;
    }

    private void BpmChanged(string _value)
    {
        EventManager.InvokeEvent(EventType.Bpm, Int32.Parse(_value));
    }

    private void SampleRateChanged(int _value)
    {
        Debug.Log(_value);
        EventManager.InvokeEvent(EventType.SampleRate, dropdownSampleRate.value);
    }

    public void ToggleOverwriteIndicator()
    {
        overwriteIndicator.SetActive(!overwriteIndicator.activeSelf);
    }

    public void ToggleLoopIndicator()
    {
        loopIndicator.SetActive(!loopIndicator.activeSelf);
    }

    private void UpdateTimelineSlider(int _newTime)
    {
        Debug.Log("update Timer to value: " + _newTime);
        timeLineSlider.value = _newTime;
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