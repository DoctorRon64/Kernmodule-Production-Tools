using UnityEngine;

public class CustomCursor
{
    private readonly SpriteRenderer cursorImage;
    
    public CustomCursor(SpriteRenderer _render)
    {
        cursorImage = _render;
    }
    
    public void UpdateCursorPosition(Vector2 _pos)
    {
        //offset
        _pos.x += .35f;
        _pos.y += .32f;
    
        cursorImage.transform.position = _pos;
    }

    public void ChangeCursorImage(Sprite _image)
    {
        cursorImage.sprite = _image;
    }
}