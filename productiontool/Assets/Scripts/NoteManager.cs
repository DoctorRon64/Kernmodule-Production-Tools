using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

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
    private readonly NoteVisualizer noteVisualizer;
    private readonly Transform noteParent;
    
    public NoteManager(SaveFile _saveFile, GameObject _notePrefab, Transform _noteParent)
    {
        saveFile = _saveFile;
        noteParent = _noteParent;
        noteVisualizer = new NoteVisualizer(_notePrefab, _noteParent);
    }
    
    public void PlaceNoteAtMousePosition(Vector3 _mousePos)
    {
        Vector2Int gridPosition = new Vector2Int(Mathf.RoundToInt(_mousePos.x), Mathf.RoundToInt(_mousePos.y));
        PlaceNote(gridPosition);
        noteVisualizer.VisualizeNotePlacement(noteDatabase[gridPosition]);
    }

    public void RemoveNoteAtMousePosition(Vector3 _mousePos)
    {
        Vector2Int gridPosition = new Vector2Int(Mathf.RoundToInt(_mousePos.x), Mathf.RoundToInt(_mousePos.y));
        RemoveNote(gridPosition);
        noteVisualizer.RemoveNoteVisual(gridPosition);
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

    public void Load()
    {
        ClearAllNotes();
    
        Debug.Log("Number of notes in save file: " + saveFile.noteDatabase.Count);

        foreach (var note in saveFile.noteDatabase)
        {
            noteDatabase.Add(note.Pos, note);
            Debug.Log("visualize on load");
            noteVisualizer.VisualizeNotePlacement(note);
        }
    }

    public void ClearAllNotes()
    {
        foreach (Transform child in noteParent)
        {
            Object.Destroy(child.gameObject);
        }
        noteDatabase.Clear();
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

public class NoteVisualizer
{
    private readonly GameObject notePrefab;
    private readonly Transform parentTransform;

    public NoteVisualizer(GameObject _notePrefab, Transform _parentTransform)
    {
        notePrefab = _notePrefab;
        parentTransform = _parentTransform;
    }

    public void VisualizeNotePlacement(Note _note)
    {
        var noteVisual = Object.Instantiate(notePrefab, parentTransform);
        noteVisual.transform.position = new Vector3(_note.Pos.x, _note.Pos.y, 0f);
    }

    public void RemoveNoteVisual(Vector2Int _pos)
    {
        foreach (Transform child in parentTransform)
        {
            var childPosition = child.position;
            if (Mathf.Approximately(childPosition.x, _pos.x) && Mathf.Approximately(childPosition.y, _pos.y))
            {
                Object.Destroy(child.gameObject);
                return;
            }
        }
    }
}