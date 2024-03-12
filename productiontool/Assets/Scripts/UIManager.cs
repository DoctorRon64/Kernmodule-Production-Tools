using System;
using System.Collections.Generic;
using UnityEngine.UI;

public class UIManager
{
    private readonly List<CustomButton> toolButtons = new List<CustomButton>();
    private readonly List<CustomButton> timelineButtons = new List<CustomButton>();
    private readonly List<CustomButton> savingButtons = new List<CustomButton>();
    
    public UIManager()
    {
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
    }

    private void RemoveListenersFromButtons(List<CustomButton> _buttons)
    {
        foreach (var button in _buttons)
        {
            button.RemoveAllListeners();
        }
    }
}