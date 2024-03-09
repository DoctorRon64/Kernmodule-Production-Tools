using UnityEngine;
using UnityEngine.UI;

public class ToolButton : MonoBehaviour
{
    public ToolType toolType;

    void Start()
    {
        
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        GameManager.Instance.UIManager?.SelectTool(toolType);
    }

    private void OnDisable()
    {
        GetComponent<Button>().onClick.RemoveAllListeners();
    }
}
