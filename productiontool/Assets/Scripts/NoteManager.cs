using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

[Serializable]
public class Note
{
    [SerializeField] public float Frequency;
    [SerializeField] public Vector2Int Pos;
}

public class NoteManager : ISaveable, ISaveSettings
{
    private readonly Dictionary<Vector2Int, Note> noteDatabase;
    private readonly NoteVisualizer noteVisualizer;
    private readonly Transform noteParent;
    private readonly GameManager gameManager;
    private readonly AudioManager audioManager;

    private static readonly Vector2Int minBound = new Vector2Int(-18, 0);
    private static readonly Vector2Int maxBound = new Vector2Int(10, -12);
    private int sampleRate = MusicLib.SampleRateLib[0];
    
    public NoteManager(GameManager _gameManager, AudioManager _audioManager , GameObject _notePrefab, Transform _noteParent)
    {
        this.audioManager = _audioManager;
        this.gameManager = _gameManager;
        this.noteParent = _noteParent;
        
        noteDatabase = new Dictionary<Vector2Int, Note>();
        noteVisualizer = new NoteVisualizer(_notePrefab, _noteParent);
        
        EventManager.AddListener<int>(EventType.TimerElapse, PlayNotesAtPosition);
        EventManager.AddListener<int>(EventType.SampleRate, GetSampleRate);
    }
    
    public void PlaceOrRemoveNoteAtMousePosition(Vector3 _mousePos, bool _placeNote)
    {
        Vector2Int gridPosition = new Vector2Int(Mathf.RoundToInt(_mousePos.x), Mathf.RoundToInt(_mousePos.y));
        
        if (IfMousePosOutBounds(gridPosition)) return;
        
        if (gridPosition.x < minBound.x || gridPosition.x > maxBound.x ||
            gridPosition.y > minBound.y || gridPosition.y < maxBound.y)
            return;

        if (_placeNote)
        {
            if (noteDatabase.ContainsKey(gridPosition)) return;
            PlaceNote(gridPosition);
        }
        else
        {
            if (!noteDatabase.ContainsKey(gridPosition)) return;
            RemoveNote(gridPosition);
        }
    }

    private bool IfMousePosOutBounds(Vector2Int _pos){
        if (_pos.x < minBound.x || _pos.x >= maxBound.x) return false;
        if (_pos.y < minBound.y || _pos.y >= maxBound.y) return false;
        return true;
    }
    
    private void PlaceNote(Vector2Int _pos)
    {
        if (!CheckFrequencyWithYPos(-_pos.y)) return;
        Note newNote = new Note()
        {
            Frequency = GetFrequencyWithYPos(-_pos.y),
            Pos = _pos,
        };
        noteDatabase.Add(_pos, newNote);
        noteVisualizer.VisualizeNotePlacement(noteDatabase[_pos]);
    }

    private void RemoveNote(Vector2Int _pos)
    {
        noteDatabase.Remove(_pos);
        noteVisualizer.RemoveNoteVisual(_pos);
    }

    private bool CheckFrequencyWithYPos(int _ypos)
    {
        if (_ypos < 0 || _ypos >= MusicLib.FrequenciesLib.Length) return false;
        return MusicLib.FrequenciesLib[_ypos] != 0;
    }

    private float GetFrequencyWithYPos(int _Ypos)
    {
        return MusicLib.FrequenciesLib[_Ypos];
    }

    private void GetSampleRate(int _value)
    {
        sampleRate = MusicLib.SampleRateLib[_value];
    }

    public void Load(SaveFile _save)
    {
        ClearAllNotes();
    
        foreach (var note in _save.noteDatabase)
        {
            noteDatabase.Add(note.Pos, note);
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

    public void PlayNotesAtPosition(int _timelinePosition)
    {
        foreach (var note in noteDatabase.Values)
        {
            int notePosX = note.Pos.x - minBound.x;
            if (notePosX == _timelinePosition)
            {
                Debug.Log("called the note corrosponding to time frame: " + _timelinePosition + " " + notePosX);
                audioManager.PlayClip(note, sampleRate);
            }
        }
    }
    
    public void Save(SaveFile _save)
    {
        List<Note> newNoteList = new List<Note>();
        foreach (var note in noteDatabase)
        {
            newNoteList.Add(note.Value);
        }
        _save.noteDatabase = newNoteList;
    }

    public void Load(SettingsFile _load)
    {
        sampleRate = MusicLib.SampleRateLib[_load.SampleRate];
    }

    public void Save(SettingsFile _save)
    {
        int indexSampleRate = Array.IndexOf(MusicLib.SampleRateLib, sampleRate);
        if (indexSampleRate != -1)
        {
            _save.SampleRate = indexSampleRate;
        }
        else
        {
            Debug.LogWarning("Sample rate not found in the library. Saving default sample rate index.");
            _save.SampleRate = 0;
        }
    }
}
//========================MUSIC LIBRARY======================================
public static class MusicLib
{
    public static readonly float[] FrequenciesLib = new float[]
    {
        493.88f, 466.16f, 440f, 415.3f, 392f, 369.99f,
        349.23f, 329.63f, 311.13f, 293.66f, 277.18f,
        261.63f,
    };

    public static readonly int[] SampleRateLib = new int[]
    {
        44100, 48000
    };
}

//========================VISUALIZER=========================================
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
            if (child == null) continue;
            var childPosition = child.position;
            if (!Mathf.Approximately(childPosition.x, _pos.x) ||
                !Mathf.Approximately(childPosition.y, _pos.y)) continue;
            Object.Destroy(child.gameObject);
            return;
        }
    }
}
