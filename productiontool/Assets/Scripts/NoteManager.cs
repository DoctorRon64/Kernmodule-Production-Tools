using System;
using System.Collections.Generic;

[Serializable]
public class Note
{
    public string ID;
    public float Pitch;
    public int Pos;
}

public class NoteManager : ISaveable
{
    private readonly Dictionary<string, Note> noteDatabase = new Dictionary<string, Note>();
    private readonly SaveFile saveFile;
    
    public NoteManager(SaveFile _saveFile)
    {
        saveFile = _saveFile;
    }

    public void PlaceNote(float _pitch, int _pos)
    {
        string hashId = GenerateId();
        if (noteDatabase.ContainsKey(hashId)) return;

        var newNote = new Note()
        {
            ID = hashId,
            Pitch = _pitch,
            Pos = _pos,
        };
        noteDatabase.Add(newNote.ID, newNote);
    }

    private string GenerateId()
    {
        return Guid.NewGuid().ToString();
    }

    public void RemoveNote(Note _note)
    {
        if (!noteDatabase.ContainsKey(_note.ID)) return;
        noteDatabase.Remove(_note.ID);
    }

    //handleInput

    public void Load()
    {
        foreach (var note in saveFile.NoteDatabase)
        {
            noteDatabase.Add(note.ID, note);
        }
    }

    public void Save()
    {
        List<Note> newNoteList = new List<Note>();
        foreach (var note in noteDatabase)
        {
            newNoteList.Add(note.Value);
        }
        
        newNoteList = saveFile.NoteDatabase;
    }
}