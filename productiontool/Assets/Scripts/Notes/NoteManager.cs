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
    private readonly Stack<ICommand> commandStack = new Stack<ICommand>();
    private readonly Stack<ICommand> redoStack = new Stack<ICommand>();
    
    public static readonly Vector2Int MinBound = new Vector2Int(-18, 0);
    public static readonly Vector2Int MaxBound = new Vector2Int(10, -12);
    private int sampleRate = MusicLib.SampleRateLib[0];

    public NoteManager(GameManager _gameManager, AudioManager _audioManager, GameObject _notePrefab,
        Transform _noteParent)
    {
        this.audioManager = _audioManager;
        this.gameManager = _gameManager;
        this.noteParent = _noteParent;

        noteDatabase = new Dictionary<Vector2Int, Note>();
        noteVisualizer = new NoteVisualizer(_notePrefab, _noteParent);

        EventManager.AddListener<int>(EventType.TimerElapse, PlayNotesAtPosition);
        EventManager.AddListener<int>(EventType.SampleRate, GetSampleRate);
    }

    public void ExecuteCommand(ICommand _command)
    {
        _command.Execute();
        commandStack.Push(_command);
        redoStack.Clear();
    }
    
    public void UndoLastCommand()
    {
        if (commandStack.Count <= 0) return;
        ICommand lastCommand = commandStack.Pop();
        lastCommand.Undo();
        redoStack.Push(lastCommand);
    }
    
    public void RedoLastCommand()
    {
        if (redoStack.Count <= 0) return;
        ICommand lastRedoCommand = redoStack.Pop();
        lastRedoCommand.Execute();
        commandStack.Push(lastRedoCommand);
    }

    public Note GetNoteAtMousePosition(Vector3 _mousePosition)
    {
        Vector2Int gridPosition = new Vector2Int(Mathf.RoundToInt(_mousePosition.x), Mathf.RoundToInt(_mousePosition.y));
        if (noteDatabase.TryGetValue(gridPosition, out Note note))
        {
            return note;
        }
        return null;
    }
    
    public void UpdateValue(int _timelinePosition)
    {
        foreach (var note in noteDatabase.Values)
        {
            int notePosX = note.Pos.x - MinBound.x;
            if (notePosX == _timelinePosition)
            {
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
    public void PlaceOrRemoveNoteAtPosition(Vector3 _mousePos, bool _placeNote)
    {
        Vector2Int gridPosition = new Vector2Int(Mathf.RoundToInt(_mousePos.x), Mathf.RoundToInt(_mousePos.y));

        if (IfMousePosOutBounds(gridPosition)) return;
        if (!IsGridPositionValid(gridPosition)) return;

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
    public Dictionary<Vector2Int, Note> GetNoteDictionary()
    {
        return noteDatabase;
    }
    
    //=============== Private Methods ===================================
    private bool IsGridPositionValid(Vector2Int _position)
    {
        return !(_position.x < MinBound.x || _position.x > MaxBound.x ||
                 _position.y > MinBound.y || _position.y < MaxBound.y);
    }
    private void PlayNotesAtPosition(int _newTime)
    {
        lock (gameManager.actionQueue)
        {
            gameManager.actionQueue.Enqueue(() => UpdateValue(_newTime - 1));
        }
    }
    private bool IfMousePosOutBounds(Vector2Int _pos)
    {
        if (_pos.x < MinBound.x || _pos.x >= MaxBound.x) return false;
        if (_pos.y < MinBound.y || _pos.y >= MaxBound.y) return false;
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
        audioManager.PlayClip(newNote, sampleRate);
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