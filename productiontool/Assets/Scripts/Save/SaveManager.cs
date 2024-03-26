using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using SFB;

public class SaveManager : ISaveSettings
{
    private readonly string defaultFileName = "saveFile";
    private string lastNamedFile;
    private readonly List<ISaveable> saveableNotes;
    private readonly List<ISaveSettings> saveablesSettings;
    private SaveFile saveFile;

    private readonly string settingsFileName = "settings";
    private SettingsFile settingsFile;

    private readonly GameManager gameManager;
    private readonly NoteManager noteManager;
    private bool doesPlayerWantOverwritePopUp = true;

    public SaveManager(GameManager _gameManager, NoteManager _noteManager)
    {
        settingsFile = new SettingsFile();
        saveableNotes = new List<ISaveable>();
        saveablesSettings = new List<ISaveSettings>();
        gameManager = _gameManager;
        noteManager = _noteManager;

        EventManager.Parameterless.AddListener(EventType.OverwriteToggle, ToggleIfPlayerWantsOverwite);
    }

    //==================================Settings Saving =========================

    public void ExportToFile()
    {
        WavExporter wavExporter = new WavExporter(
            MusicLib.SampleRateLib[settingsFile.SampleRate],
            CalculateTimelineDuration(gameManager.GetTimelineBpm()),
            gameManager.GetTimelineBpm(),
            noteManager?.GetNoteDictionary(),
            lastNamedFile
        );

        wavExporter.ExportWav();
    }

    private float CalculateTimelineDuration(int _bpm)
    {
        float durationPerBeat = 60f / _bpm;
        float totalDuration = durationPerBeat * 29;
        return totalDuration;
    }

    public void LoadSettings()
    {
        string settingsPath = GetFullPath(settingsFileName);

        if (!File.Exists(settingsPath))
        {
            settingsFile = new SettingsFile();
            SaveSettings();
            return;
        }

        settingsFile = LoadJson<SettingsFile>(settingsPath);

        foreach (ISaveSettings saveable in saveablesSettings)
        {
            saveable.Load(settingsFile);
        }
    }

    public void SaveSettings()
    {
        foreach (ISaveSettings saveable in saveablesSettings)
        {
            saveable.Save(settingsFile);
        }

        string settingsPath = GetFullPath(settingsFileName);
        SaveJson(settingsPath, settingsFile);
    }

    //=============================== Tool Saving =============================
    public void SaveTool(string _saveFileName, bool _isOverwrite)
    {
        SaveSettings();
        saveFile = new SaveFile();
        lastNamedFile = _saveFileName;
        string fullpath = _saveFileName;

        if (_isOverwrite)
        {
            fullpath = GetFullPath(_saveFileName);
            if (File.Exists(fullpath))
            {
                if (!doesPlayerWantOverwritePopUp) return;
                gameManager.HandleOverwriteConfirmation(fullpath);
                return;
            }
        }

        foreach (ISaveable saveable in saveableNotes)
        {
            saveable.Save(saveFile);
        }

        SaveJson(fullpath, saveFile);
    }

    public void LoadTool(string _saveFileName)
    {
        saveFile = new SaveFile();
        lastNamedFile = _saveFileName;
        string fullpath = GetFullPath(_saveFileName);

        if (!File.Exists(fullpath))
        {
            Debug.LogWarning("Save file not found.");
            return;
        }

        saveFile = LoadJson<SaveFile>(fullpath);

        foreach (ISaveable _saveable in saveableNotes)
        {
            _saveable.Load(saveFile);
        }
    }

    //============================ Handy methods =============================

    private void SaveJson<T>(string _path, T _file)
    {
        string jsonData = JsonUtility.ToJson(_file, true);
        StreamWriter writer = new StreamWriter(_path, false);
        writer.WriteLine(jsonData);
        writer.Close();
        writer.Dispose();
    }

    private T LoadJson<T>(string _path)
    {
        StreamReader reader = new StreamReader(_path);
        string jsonData = reader.ReadToEnd();
        reader.Close();
        reader.Dispose();

        return JsonUtility.FromJson<T>(jsonData);
    }

    public void ToggleIfPlayerWantsOverwite()
    {
        doesPlayerWantOverwritePopUp = !doesPlayerWantOverwritePopUp;
    }

    public void AddSaveable(ISaveable _saveable)
    {
        saveableNotes.Add(_saveable);
    }

    public void AddSettings(ISaveSettings _settings)
    {
        saveablesSettings.Add(_settings);
    }

    private string GetFullPath(string _fileName)
    {
        if (string.IsNullOrEmpty(_fileName))
        {
            _fileName = defaultFileName;
        }

        string fullPath = Path.Combine( /*Application.isEditor ?  : Application.persistentDataPath */
            Application.dataPath, _fileName + ".json");
        return fullPath;
    }

    public void Load(SettingsFile _load)
    {
        doesPlayerWantOverwritePopUp = _load.DoesPlayerWantOverwritePopUp;
    }

    public void Save(SettingsFile _save)
    {
        _save.DoesPlayerWantOverwritePopUp = doesPlayerWantOverwritePopUp;
    }
}

public class WavExporter
{
    private int sampleRate;
    private float songDuration;
    private int bpm;
    private Dictionary<Vector2Int, Note> noteDictionary;
    private string fileName;

    public WavExporter(int _sampleRate, float _songDuration, int _bpm, Dictionary<Vector2Int, Note> _noteDictionary,
        string _fileName)
    {
        this.sampleRate = _sampleRate;
        this.songDuration = _songDuration;
        this.bpm = _bpm;
        this.noteDictionary = _noteDictionary;
        this.fileName = _fileName;
    }

    public void ExportWav()
    {
        var extensionFilter = new[]
        {
            new ExtensionFilter("WAV files", "wav"),
        };

        // Open file save panel
        string path = StandaloneFileBrowser.SaveFilePanel("Save WAV File", "", fileName, extensionFilter);

        if (!string.IsNullOrEmpty(path))
        {
            FileStream fileStream = null;

            try
            {
                // Create file stream
                fileStream = new FileStream(path, FileMode.Create);

                // Write WAV header and audio data
                WriteWavHeader(fileStream);
                WriteAudioData(fileStream);
            }
            catch (Exception e)
            {
                Debug.LogError("Error exporting WAV file: " + e.Message);
            }
            finally
            {
                if (fileStream != null)
                {
                    fileStream.Close();
                    fileStream.Dispose();
                }
            }
        }
    }

    private void WriteWavHeader(FileStream fileStream)
    {
        // Calculate total samples
        int totalSamples = (int)(songDuration * sampleRate);

        // Calculate data size
        int dataSize = totalSamples * 2; // 16-bit audio, so 2 bytes per sample

        // Write WAV header
        byte[] header = new byte[44];
        header[0] = (byte)'R';
        header[1] = (byte)'I';
        header[2] = (byte)'F';
        header[3] = (byte)'F';
        WriteInt32(header, 4, 36 + dataSize);
        header[8] = (byte)'W';
        header[9] = (byte)'A';
        header[10] = (byte)'V';
        header[11] = (byte)'E';
        header[12] = (byte)'f';
        header[13] = (byte)'m';
        header[14] = (byte)'t';
        header[15] = (byte)' ';
        WriteInt32(header, 16, 16);
        WriteInt16(header, 20, 1);
        WriteInt16(header, 22, 1);
        WriteInt32(header, 24, sampleRate);
        WriteInt32(header, 28, sampleRate * 2);
        WriteInt16(header, 32, 2);
        WriteInt16(header, 34, 16);
        header[36] = (byte)'d';
        header[37] = (byte)'a';
        header[38] = (byte)'t';
        header[39] = (byte)'a';
        WriteInt32(header, 40, dataSize);

        fileStream.Write(header, 0, header.Length);
    }

    private void WriteAudioData(FileStream _fileStream)
    {
        // Generate audio data
        int totalSamples = (int)(songDuration * sampleRate);
        float[] audioData = new float[totalSamples];

        foreach (KeyValuePair<Vector2Int, Note> pair in noteDictionary)
        {
            Note note = pair.Value;
            int startPos = Mathf.Clamp((int)(note.Pos.x * sampleRate), 0, totalSamples - 1);
            int endPos = Mathf.Clamp((int)(note.Pos.y * sampleRate), 0, totalSamples - 1);
            for (int i = startPos; i < endPos; i++)
            {
                audioData[i] += Mathf.Sin(2 * Mathf.PI * note.Frequency * i / sampleRate);
            }
        }

        // Normalize audio data
        float max = 0f;
        foreach (float sample in audioData)
        {
            max = Mathf.Max(max, Mathf.Abs(sample));
        }

        for (int i = 0; i < totalSamples; i++)
        {
            audioData[i] /= max;
        }

        // Convert float audio data to 16-bit PCM and write to file
        short[] pcmData = new short[totalSamples];
        for (int i = 0; i < totalSamples; i++)
        {
            pcmData[i] = (short)Mathf.Clamp(audioData[i] * short.MaxValue, short.MinValue, short.MaxValue);
        }

        byte[] byteData = new byte[totalSamples * 2];
        Buffer.BlockCopy(pcmData, 0, byteData, 0, byteData.Length);
        _fileStream.Write(byteData, 0, byteData.Length);
    }

    private void WriteInt32(byte[] _array, int _offset, int _value)
    {
        _array[_offset] = (byte)(_value & 0xff);
        _array[_offset + 1] = (byte)((_value >> 8) & 0xff);
        _array[_offset + 2] = (byte)((_value >> 16) & 0xff);
        _array[_offset + 3] = (byte)((_value >> 24) & 0xff);
    }

    private void WriteInt16(byte[] _array, int _offset, short _value)
    {
        _array[_offset] = (byte)(_value & 0xff);
        _array[_offset + 1] = (byte)((_value >> 8) & 0xff);
    }
}