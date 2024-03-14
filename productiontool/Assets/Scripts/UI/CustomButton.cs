using System;
using UnityEngine.UI;

public class CustomButton
{
    private readonly Button button;
    private readonly int indexButton;
    private Action<int> onClickToolButton;
    
    public CustomButton(Button _button, int _indexButton)
    {
        this.indexButton = _indexButton;
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
        onClickToolButton?.Invoke(indexButton);
    }
}