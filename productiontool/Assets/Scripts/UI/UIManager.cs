using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class UIManager
{
    private readonly List<CustomButton> toolButtons = new List<CustomButton>();
    private readonly List<CustomButton> timelineButtons = new List<CustomButton>();
    private readonly List<CustomButton> savingButtons = new List<CustomButton>();
    private readonly GameObject overwriteIndicator;
    private readonly GameObject loopIndicator;
    private readonly Slider timeLineSlider;
   
    private GameManager gameManager;
    
    public UIManager(GameManager _gameManger,GameObject _overwriteIndicator, GameObject _loopIndicator, Slider _timeLineSlider)
    {
        this.timeLineSlider = _timeLineSlider;
        gameManager = _gameManger;
        loopIndicator = _loopIndicator;
        overwriteIndicator = _overwriteIndicator;
        
        Timeline.OnTimeLineElapsed += UpdateTimelineSlider;
        overwriteIndicator.SetActive(true);
    }

    public void ToggleOverwriteIndicator()
    {
        overwriteIndicator.SetActive(!overwriteIndicator.activeSelf);
    }
    
    public void ToggleLoopIndicator()
    {
        loopIndicator.SetActive(!loopIndicator.activeSelf);
    }

    public void InitializeToolButtons(List<Button> _buttons, Action<int> _onClickCallback)
    {
        InitializeButtons(_buttons, _onClickCallback, toolButtons);
    }
    
    public void InitializeTimelineButtons(List<Button> _buttons, Action<int> _onClickCallback)
    {
        InitializeButtons(_buttons, _onClickCallback, timelineButtons);
    }

    public void InitializeSavingButtons(List<Button> _buttons, Action<int> _onClickCallBack)
    {
        InitializeButtons(_buttons, _onClickCallBack, savingButtons);
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
        Timeline.OnTimeLineElapsed -= UpdateTimelineSlider;
    }

    private void RemoveListenersFromButtons(List<CustomButton> _buttons)
    {
        foreach (var button in _buttons)
        {
            button.RemoveAllListeners();
        }
    }
}