using UnityEngine;
using UnityEngine.EventSystems;

public class CustomHoverDetector : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private string hoverText;
    
    public void InitText(string _hoverText)
    {
        hoverText = _hoverText;
    }
    
    public void OnPointerEnter(PointerEventData _eventData)
    {
        EventManager.InvokeEvent(EventType.ButtonHoverText, hoverText);
        EventManager.InvokeEvent(EventType.OnMouseHover, true);
    }

    public void OnPointerExit(PointerEventData _eventData)
    {
        EventManager.InvokeEvent(EventType.OnMouseHover, false);
    }
}