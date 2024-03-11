using UnityEngine;
using UnityEngine.UI;

public class ToolCursor
{
    private SpriteRenderer cursorImage;

    public ToolCursor(SpriteRenderer _render)
    {
        cursorImage = _render;
    }
    
    public void UpdateCursorPosition(Vector2 _pos)
    {
        cursorImage.transform.position = _pos;
    }

    public void ChangeCursorImage(Sprite _image)
    {
        cursorImage.sprite = _image;
    }
}