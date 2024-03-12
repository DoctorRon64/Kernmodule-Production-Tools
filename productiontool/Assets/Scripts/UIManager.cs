using System;
using System.Collections.Generic;
using UnityEngine.UI;

public class UIManager
{
    private readonly List<CustomButton> toolButtons = new List<CustomButton>();
    private readonly List<CustomButton> timelineButtons = new List<CustomButton>();
    
    public UIManager()
    {
    }

    public void InitializeToolButtons(List<Button> buttons, Action<int> onClickCallback)
    {
        InitializeButtons(buttons, onClickCallback, toolButtons);
    }
    
    public void InitializeTimelineButtons(List<Button> buttons, Action<int> onClickCallback)
    {
        InitializeButtons(buttons, onClickCallback, timelineButtons);
    }

    private void InitializeButtons(List<Button> buttons, Action<int> onClickCallback, List<CustomButton> buttonList)
    {
        foreach (var button in buttons)
        {
            var customButton = new CustomButton(button, buttonList.Count);
            customButton.AddListener(onClickCallback);
            buttonList.Add(customButton);
        }
    }

    public void RemoveListeners()
    {
        RemoveListenersFromButtons(toolButtons);
        RemoveListenersFromButtons(timelineButtons);
    }

    private void RemoveListenersFromButtons(List<CustomButton> buttons)
    {
        foreach (var button in buttons)
        {
            button.RemoveAllListeners();
        }
    }
}