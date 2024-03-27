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
        EventManager.InvokeEvent(EventType.InfoText, hoverText);
        EventManager.InvokeEvent(EventType.InfoPopUpActive, true);
    }

    public void OnPointerExit(PointerEventData _eventData)
    {
        EventManager.InvokeEvent(EventType.InfoPopUpActive, false);
    }
}