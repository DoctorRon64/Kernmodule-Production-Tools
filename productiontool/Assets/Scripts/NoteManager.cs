using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class Note
{
    [SerializeField] public float Frequency;
    [SerializeField] public int SampleRate;
    [SerializeField] public Vector2Int Pos;
}

public class NoteManager : ISaveable
{
    private readonly Dictionary<Vector2Int, Note> noteDatabase = new Dictionary<Vector2Int, Note>();
    private readonly SaveFile saveFile;
    private GameObject notePrefab;
    
    public NoteManager(SaveFile _saveFile, GameObject _notePrefab)
    {
        notePrefab = _notePrefab;
        saveFile = _saveFile;
    }
    
    public void PlaceNoteAtMousePosition(Vector3 _mousePos)
    {
        Vector2Int gridPosition = new Vector2Int(Mathf.RoundToInt(_mousePos.x), Mathf.RoundToInt(_mousePos.y));
        PlaceNote(gridPosition);
    }

    public void RemoveNoteAtMousePosition(Vector3 _mousePos)
    {
        Vector2Int gridPosition = new Vector2Int(Mathf.RoundToInt(_mousePos.x), Mathf.RoundToInt(_mousePos.y));
        RemoveNote(gridPosition);
    }

    private void PlaceNote(Vector2Int _pos)
    {
        if (noteDatabase.ContainsKey(_pos)) return;
        var newNote = new Note()
        {
            Frequency = GetFrequencyWithYPos(_pos.y),
            SampleRate = GetSampleRate(),
            Pos = _pos,
        };
        noteDatabase.Add(_pos, newNote);
    }

    private void RemoveNote(Vector2Int _pos)
    {
        if (!noteDatabase.ContainsKey(_pos)) return;
        noteDatabase.Remove(_pos);
    }

    private float GetFrequencyWithYPos(int _Ypos)
    {
        return MusicLib.FrequenciesLib[_Ypos];
    }
    
    private int GetSampleRate()
    {
        return MusicLib.SampleRateLib[0];
    }

    //handleInput
    public void Load()
    {
        foreach (var note in saveFile.noteDatabase)
        {
            noteDatabase.Add(note.Pos, note);
        }
    }
    
    public void Save()
    {
        List<Note> newNoteList = new List<Note>();
        foreach (var note in noteDatabase)
        {
            newNoteList.Add(note.Value);
        }
        saveFile.noteDatabase = newNoteList;
    }
}

public static class MusicLib
{
    public static readonly float[] FrequenciesLib = new float[]
    {
        261.63f, 277.18f, 293.66f, 311.13f, 329.63f,
        349.23f, 369.99f, 392f, 415.3f, 440f, 466.16f, 
        493.88f
    };

    public static readonly int[] SampleRateLib = new int[]
    {
        44100, 48000
    };
}