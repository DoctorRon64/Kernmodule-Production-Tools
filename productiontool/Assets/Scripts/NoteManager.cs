﻿using System;
using System.Collections.Generic;

public class Note
{
    public string id;
    public float pitch;
    public int pos;
}

public class NoteManager : ISaveable
{
    Dictionary<string, Note> noteDatabase = new Dictionary<string, Note>();

    public void PlaceNote(float _pitch, int _pos)
    {
        string hashId = GenerateId();
        if (noteDatabase.ContainsKey(hashId)) return;

        Note newNote = new Note()
        {
            id = hashId,
            pitch = _pitch,
            pos = _pos,
        };
        noteDatabase.Add(newNote.id, newNote);
    }

    private string GenerateId()
    {
        return Guid.NewGuid().ToString();
    }

    public void RemoveNote(Note _note)
    {
        if (!noteDatabase.ContainsKey(_note.id)) return;
        noteDatabase.Remove(_note.id);
    }

    //handleInput

    public void Load()
    {
        this.noteDatabase = DataManager.Instance.noteDatabase;
    }

    public void Save()
    {
        DataManager.Instance.noteDatabase = this.noteDatabase;
    }
}
