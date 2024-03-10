using System;
using System.Collections.Generic;

public class UIManager
{
    private readonly GameManager gameManager;
    private readonly List<ToolButton> toolButtons = new List<ToolButton>();
    
    public UIManager(GameManager _gameManager)
    {
        this.gameManager = _gameManager;
    }

    public void InitializeToolButtons(List<ToolButton> _toolbuttons, Action<int> _onClickCallback)
    {
        foreach (var toolButton in _toolbuttons)
        {
            toolButton.AddListener(_onClickCallback);
            toolButtons.Add(toolButton);
        }
    }
    
    public void RemoveListeners()
    {
        foreach (var toolButton in toolButtons)
        {
            toolButton.RemoveAllListeners();
        }
    }
}