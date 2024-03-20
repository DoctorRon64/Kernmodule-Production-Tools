using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using SFB;

public class SaveManager : ISaveSettings
{
    private readonly string defaultFileName = "saveFile";
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
        Debug.Log(MusicLib.SampleRateLib[settingsFile.SampleRate] + ":" + 29 + ":" + gameManager.GetTimelineBPM() +
                  ":" + noteManager?.GetNoteDictionary());
        WavExporter wavExporter = new WavExporter(MusicLib.SampleRateLib[settingsFile.SampleRate], 29,
            gameManager.GetTimelineBPM(), noteManager?.GetNoteDictionary());
        Debug.Log(wavExporter);
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
    private double durationInSeconds;
    private int bpm;
    private float[] frequencies;

    public WavExporter(int _sampleRate, double _durationInSeconds, int _bpm, Dictionary<Vector2Int, Note> _notes)
    {
        sampleRate = _sampleRate;
        durationInSeconds = _durationInSeconds;
        bpm = _bpm;

        List<float> freqList = new List<float>();
        foreach (var kvp in _notes)
        {
            freqList.Add(kvp.Value.Frequency);
        }

        frequencies = freqList.ToArray();

        Debug.Log("Total Duration: " + durationInSeconds);
        int numSamples = (int)(_sampleRate * durationInSeconds);
        Debug.Log("Total Samples: " + numSamples);

        byte[] audioData = GenerateAudio(sampleRate, durationInSeconds, bpm, frequencies);

        SaveFileDialog(audioData, sampleRate);
    }

    private byte[] GenerateAudio(int _sampleRate, double _durationInSeconds, int _bpm, float[] frequencies)
    {
        int numSamples = (int)(_sampleRate * _durationInSeconds);
        byte[] audioData = new byte[numSamples * 2];
        int maxAmplitude = 32767;
        double beatDurationInSeconds = 60.0 / _bpm;
        int remainingSamples = numSamples;
        double[] samples = new double[numSamples];

        foreach (float frequency in frequencies)
        {
            double noteDurationInSeconds = beatDurationInSeconds;
            int noteNumSamples = (int)(_sampleRate * noteDurationInSeconds);

            // Adjust note duration based on the remaining samples
            if (noteNumSamples > remainingSamples)
            {
                noteDurationInSeconds = (double)remainingSamples / _sampleRate;
                noteNumSamples = remainingSamples;
            }

            // Generate samples for the current note
            for (int i = 0; i < noteNumSamples && remainingSamples > 0; i++)
            {
                double sample = Math.Sin(2 * Math.PI * frequency * i / _sampleRate);
                int audioIndex = (numSamples - remainingSamples + i);
                samples[audioIndex] += sample;
                remainingSamples--;
            }
        }

        // Normalize samples to ensure they stay within range
        double maxSample = samples.Max();
        double minSample = samples.Min();
        double range = Math.Max(Math.Abs(maxSample), Math.Abs(minSample));
        double scale = maxAmplitude / range;

        // Convert double samples to short samples
        short[] shortSamples = new short[numSamples];
        for (int i = 0; i < numSamples; i++)
        {
            shortSamples[i] = (short)(samples[i] * scale);
        }

        // Convert short samples to bytes
        for (int i = 0; i < numSamples; i++)
        {
            byte[] sampleBytes = BitConverter.GetBytes(shortSamples[i]);
            Buffer.BlockCopy(sampleBytes, 0, audioData, i * 2, 2);
        }

        return audioData;
    }

    private void SaveFileDialog(byte[] _audioData, int _sampleRate)
    {
        string filePath = StandaloneFileBrowser.SaveFilePanel("Save WAV file", "", "mySongName", "wav");
        if (filePath.Length == 0)
        {
            Debug.Log("No file selected.");
            return;
        }

        WriteWavFile(filePath, _audioData, _sampleRate);
    }

    private void WriteWavFile(string _filePath, byte[] _audioData, int _sampleRate)
    {
        using (var stream = new FileStream(_filePath, FileMode.Create))
        using (var writer = new BinaryWriter(stream))
        {
            writer.Write(new char[] { 'R', 'I', 'F', 'F' });
            writer.Write(36 + _audioData.Length); // RIFF chunk size
            writer.Write(new char[] { 'W', 'A', 'V', 'E' });
            writer.Write(new char[] { 'f', 'm', 't', ' ' });
            writer.Write(16); // Size of fmt chunk
            writer.Write((short)1); // Audio format (PCM)
            writer.Write((short)1); // Num channels
            writer.Write(_sampleRate); // Sample rate
            writer.Write(_sampleRate * 2); // Byte rate
            writer.Write((short)2); // Block align
            writer.Write((short)16); // Bits per sample
            writer.Write(new char[] { 'd', 'a', 't', 'a' });
            writer.Write(_audioData.Length); // Data chunk size
            writer.Write(_audioData);
        }
    }
}