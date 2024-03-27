using TMPro;
using UnityEngine;

public class CustomHoverMessage
{
    private bool isHovering = false;
    private readonly TextMeshProUGUI hoverText;

    public CustomHoverMessage(TextMeshProUGUI _hoverText)
    {
        hoverText = _hoverText;
        hoverText.gameObject.SetActive(false);
        
        EventManager.AddListener<bool>(EventType.InfoPopUpActive, ToggleVisibility);
        EventManager.AddListener<string>(EventType.InfoText, SetText);
    }

    private void SetText(string _text)
    {
        hoverText.text = _text;
    }

    private void ToggleVisibility(bool _isVisible)
    {
        isHovering = _isVisible;
        hoverText.gameObject.SetActive(_isVisible);
    }
}