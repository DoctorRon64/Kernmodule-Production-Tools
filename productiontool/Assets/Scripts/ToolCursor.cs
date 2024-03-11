using UnityEngine;

public class ToolCursor
{
    private readonly SpriteRenderer cursorImage;

    public ToolCursor(SpriteRenderer _render)
    {
        cursorImage = _render;
        if (cursorImage == null) Debug.LogError("No cursor image assigned.");
    }

    public void UpdateCursorPosition(Vector3 _pos)
    {
        if (cursorImage == null) { Debug.LogError("No cursor image assigned."); return; }
        cursorImage.transform.position = _pos;
    }

    public void ChangeCursorImage(Sprite _image)
    {
        if (cursorImage != null)
        {
            cursorImage.sprite = _image;
        }
        else
        {
            Debug.LogError("No cursor image assigned.");
        }
    }
}