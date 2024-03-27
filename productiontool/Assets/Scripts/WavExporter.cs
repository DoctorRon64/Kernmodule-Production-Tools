using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SFB;
using UnityEngine;

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

    private byte[] GenerateAudio(int _sampleRate, double _durationInSeconds, int _bpm, float[] _frequencies)
    {
        int numSamples = (int)(_sampleRate * _durationInSeconds);
        byte[] audioData = new byte[numSamples * 2];
        int maxAmplitude = 32767;
        double beatDurationInSeconds = 60.0 / _bpm;
        int remainingSamples = numSamples;
        double[] samples = new double[numSamples];
        
        foreach (float frequency in _frequencies)
        {
            double noteDurationInSeconds = beatDurationInSeconds;
            int noteNumSamples = (int)(_sampleRate * noteDurationInSeconds);

            // Adjust note duration based on the remaining samples
            if (noteNumSamples > remainingSamples)
            {
                noteNumSamples = remainingSamples;
            }

            // Generate samples for the current note
            for (int i = 0; i < noteNumSamples && remainingSamples > 0; i++)
            {
                double sample = Math.Sin(2 * Math.PI * frequency * i / _sampleRate);
                int audioIndex = (numSamples - remainingSamples + i);
                if (audioIndex >= samples.Length) continue;
                samples[audioIndex] += sample;
                remainingSamples--;
            }
        }


        // Normalize samples to ensure they stay within range
        double maxSample = samples.Max();
        double minSample = samples.Min();
        double maxAbsSample = Math.Max(Math.Abs(maxSample), Math.Abs(minSample));
        double scale = maxAmplitude / maxAbsSample;

        // Convert double samples to short samples and apply normalization
        for (int i = 0; i < numSamples; i++)
        {
            short normalizedSample = (short)(samples[i] * scale);
            byte[] sampleBytes = BitConverter.GetBytes(normalizedSample);
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