using UnityEngine;

public class CustomCursor
{
    private SpriteRenderer cursorImage;
    
    public CustomCursor(SpriteRenderer _render)
    {
        cursorImage = _render;
    }
    
    public void UpdateCursorPosition(Vector2 _pos)
    {
        //offset
        /*
        _pos.x += .3f;
        _pos.y += .3f;
        */
        cursorImage.transform.position = _pos;
    }

    public void ChangeCursorImage(Sprite _image)
    {
        cursorImage.sprite = _image;
    }
}