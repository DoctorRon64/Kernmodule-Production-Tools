using System;
using UnityEngine;
using UnityEngine.UI;

public class CustomPopup
{
    private readonly GameObject popupPanel;
    private readonly Button continueButton;
    private readonly Button stopButton;
    private readonly GameManager gameManager;

    private Action continueAction;
    private Action stopAction;
    
    public CustomPopup(GameObject _object, Button _confirm, Button _stop, GameManager _gameManager)
    {
        this.popupPanel = _object;
        this.continueButton = _confirm;
        this.stopButton = _stop;
        this.gameManager = _gameManager;
        
        continueButton.onClick.AddListener(OnContinueClicked);
        stopButton.onClick.AddListener(OnStopClicked);
        popupPanel.SetActive(false);
    }

    public void ShowConfirmationPopup(Action _onContinue, Action _onStop)
    {
        continueAction = () =>
        {
            gameManager.TogglePlayerStopDoing();
            _onContinue?.Invoke();
            gameManager.TogglePlayerStopDoing();
        };
        stopAction = () =>
        {
            gameManager.TogglePlayerStopDoing();
            _onStop?.Invoke();
            gameManager.TogglePlayerStopDoing();
        };
        popupPanel.SetActive(true);
    }

    private void OnContinueClicked()
    {
        continueAction?.Invoke();
        popupPanel.SetActive(false);
    }

    private void OnStopClicked()
    {
        stopAction?.Invoke();
        popupPanel.SetActive(false);
    }
}