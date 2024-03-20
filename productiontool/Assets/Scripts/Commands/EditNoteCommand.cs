using UnityEngine;

public class EditNoteCommand : ICommand
{
    private readonly NoteManager noteManager;
    private readonly bool placeNote;
    private Note placedNote;
    private Vector3 mousePos;
    
    public EditNoteCommand(NoteManager _noteManager, Vector3 _mousePos, bool _placeNote)
    {
        mousePos = _mousePos;
        noteManager = _noteManager;
        placeNote = _placeNote;
    }
    
    public void Execute()
    {
        if (placeNote)
        {
            noteManager.PlaceOrRemoveNoteAtPosition(mousePos, true);
        }
        else
        {
            placedNote = noteManager.GetNoteAtMousePosition(mousePos);
            noteManager.PlaceOrRemoveNoteAtPosition(mousePos, false);
        }
    }

    public void Undo()
    {
        if (placeNote)
        {
            noteManager.PlaceOrRemoveNoteAtPosition(mousePos, false);
        }
        else
        {
            if (placedNote == null) return;
            Vector3 position = new Vector3(placedNote.Pos.x, placedNote.Pos.y, 0f);
            noteManager.PlaceOrRemoveNoteAtPosition(position, true);
        }
    }
}

public class UndoCommand : ICommand
{
    private readonly NoteManager noteManager;

    public UndoCommand(NoteManager _noteManager)
    {
        noteManager = _noteManager;
    }

    public void Execute()
    {
        noteManager.UndoLastCommand();
    }
}

public class RedoCommand : ICommand
{
    private readonly NoteManager noteManager;

    public RedoCommand(NoteManager _noteManager)
    {
        noteManager = _noteManager;
    }

    public void Execute()
    {
        noteManager.RedoLastCommand();
    }
}
