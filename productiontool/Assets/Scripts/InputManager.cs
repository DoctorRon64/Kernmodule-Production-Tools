using UnityEngine;

public class InputManager
{
    private readonly NoteManager noteManager;
    private readonly ToolManager toolManager;
    private bool isInHoverText;
    
    public InputManager(NoteManager _noteManager, ToolManager _toolManager)
    {
        noteManager = _noteManager;
        toolManager = _toolManager;
    }

    public void ToggleIsInHoverText(bool _toggle)
    {
        isInHoverText = _toggle;
    }
    
    public void Update(Vector3 _mousePos)
    {
        if (isInHoverText) return;
        CheckUndoRedoInput();
        CheckToolInput(_mousePos);
        SelectTool();
    }

    private void CheckUndoRedoInput()
    {
        if (!Input.GetKey(KeyCode.LeftControl)) return;
        if (Input.GetKeyDown(KeyCode.Z))
        {
            noteManager?.UndoLastCommand();
        }

        if (Input.GetKeyDown(KeyCode.Y))
        {
            noteManager?.RedoLastCommand();
        }
    }

    private void SelectTool()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            EventManager.InvokeEvent(EventType.SelectTool, 0);
        }
        
        if (Input.GetKeyDown(KeyCode.B))
        {
            EventManager.InvokeEvent(EventType.SelectTool, 1);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            EventManager.InvokeEvent(EventType.SelectTool, 2);
        }
    }

    private void CheckToolInput(Vector3 _mousePos)
    {
        if (toolManager == null) return;
        int whichToolIsSelected = toolManager.GetSelectedTool();

        if (whichToolIsSelected == 1 && Input.GetMouseButton(0))
        {
            Note existingNote = noteManager?.GetNoteAtMousePosition(_mousePos);
            if (existingNote != null) return;
            ICommand placeNoteCommand = new EditNoteCommand(noteManager, _mousePos, true);
            noteManager?.ExecuteCommand(placeNoteCommand);
        }

        if (whichToolIsSelected == 2 && Input.GetMouseButton(0))
        {
            Note noteToRemove = noteManager?.GetNoteAtMousePosition(_mousePos);
            if (noteToRemove == null) return;
            ICommand removeNoteCommand = new EditNoteCommand(noteManager, _mousePos, false);
            noteManager?.ExecuteCommand(removeNoteCommand);
        }
    }
}