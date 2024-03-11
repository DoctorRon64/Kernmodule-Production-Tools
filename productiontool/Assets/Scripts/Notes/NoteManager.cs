using System;
using System.Collections.Generic;

public class Note
{
    public string ID;
    public float Pitch;
    public int Pos;
}

public class NoteManager : ISaveable
{
    private Dictionary<string, Note> noteDatabase = new Dictionary<string, Note>();
    private readonly GameManager gameManager;

    public NoteManager(GameManager _gameManager)
    {
        this.gameManager = _gameManager;
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
        this.noteDatabase = gameManager.saveFile.NoteDatabase;
    }

    public void Save()
    {
        gameManager.saveFile.NoteDatabase = this.noteDatabase;
    }
}
