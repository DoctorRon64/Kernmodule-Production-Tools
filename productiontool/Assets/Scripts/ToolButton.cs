using System;
using UnityEngine.UI;

public class ToolButton
{
    private readonly Button button;
    private readonly int toolIndex;
    private Action<int> onClickToolButton;

    public ToolButton(Button _button, int _toolIndex)
    {
        this.toolIndex = _toolIndex;
        this.button = _button;
        this.onClickToolButton = null;
        button.onClick.AddListener(OnClick);
    }

    public void RemoveAllListeners()
    {
        onClickToolButton = null;
        button.onClick.RemoveAllListeners();
    }

    public void AddListener(Action<int> _onClickCallback)
    {
        onClickToolButton += _onClickCallback;
    }
    
    private void OnClick()
    {
        onClickToolButton?.Invoke(toolIndex);
    }
}