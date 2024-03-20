using UnityEngine;

public class EditNoteCommand : ICommand
{
    private readonly NoteManager noteManager;
    private readonly Vector3 mousePosition;
    private readonly bool placeNote;
    private Note placedNote;

    public EditNoteCommand(NoteManager _noteManager, Vector3 _mousePosition, bool _placeNote)
    {
        noteManager = _noteManager;
        mousePosition = _mousePosition;
        placeNote = _placeNote;
    }

    public void Execute()
    {
        if (placeNote)
        {
            noteManager.PlaceOrRemoveNoteAtPosition(mousePosition, true);
        }
        else
        {
            placedNote = noteManager.GetNoteAtMousePosition(mousePosition);
            noteManager.PlaceOrRemoveNoteAtPosition(mousePosition, false);
        }
    }

    public void Undo()
    {
        if (placeNote)
        {
            noteManager.PlaceOrRemoveNoteAtPosition(mousePosition, false);
        }
        else
        {
            if (placedNote == null) return;
            Vector3 position = new Vector3(placedNote.Pos.x, placedNote.Pos.y, 0f);
            noteManager.PlaceOrRemoveNoteAtPosition(position, true);
        }
    }
}