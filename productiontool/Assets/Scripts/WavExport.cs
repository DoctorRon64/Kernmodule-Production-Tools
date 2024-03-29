using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using SFB;
using UnityEngine;

public class WavExport
{
    private readonly int sampleRate;
    private readonly Dictionary<Vector2Int, Note> notes;
    private float songLength;
    private readonly float noteLength = 1f;
    private readonly int bpm;
    
    public WavExport(int _sampleRate, float _songLength, int _bpm, Dictionary<Vector2Int, Note> _notes)
    {
        songLength = _songLength;
        sampleRate = _sampleRate;
        bpm = _bpm;
        notes = _notes;

        CalculateDuration();
        Export();
    }

    private void CalculateDuration()
    {
        double baseSongLength = 29.0; // Base song length for BPM of 60
        double baseBPM = 60.0; // Base BPM

        // Adjust the base song length proportionally based on the current BPM
        songLength = (float)(baseSongLength * (baseBPM / bpm));
        Debug.Log(songLength);
    }
    
    private void Export()
    {
        int numChannels = 2; // Stereo
        short bitsPerSample = 16; // 16-bit PCM
        int subChunk1Size = 16;
        short audioFormat = 1; // PCM

        int numSamples = (int)(songLength * sampleRate);
        int subChunk2Size = numSamples * numChannels * bitsPerSample / 8;

        int chunkSize = 4 + (8 + subChunk1Size) + (8 + subChunk2Size);

        string filePath = StandaloneFileBrowser.SaveFilePanel("Save WAV file", "", "mySongName", "wav");
        if (string.IsNullOrEmpty(filePath))
        {
            Debug.LogError("File path is invalid or empty.");
            return;
        }

        double originalDurationInSeconds = songLength;

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            using (var writer = new BinaryWriter(stream))
            {
                writer.Write("RIFF".ToCharArray());
                writer.Write(chunkSize);
                writer.Write("WAVE".ToCharArray());
                writer.Write("fmt ".ToCharArray());
                writer.Write(subChunk1Size);
                writer.Write(audioFormat);
                writer.Write((short)numChannels);
                writer.Write(sampleRate);
                writer.Write(sampleRate * numChannels * (bitsPerSample / 8));
                writer.Write((short)(numChannels * (bitsPerSample / 8)));
                writer.Write(bitsPerSample);
                writer.Write("data".ToCharArray());
                writer.Write(subChunk2Size);

                // Writing audio data for each note
                foreach (var note in notes.Values)
                {
                    double adjustedNoteLength = noteLength * (originalDurationInSeconds / songLength); // Adjusted note length

                    double bpmMultiplier = bpm / 60.0; // Calculate the multiplier for adjusting note duration based on BPM

                    int numSamplesInNote = (int)(adjustedNoteLength * sampleRate / bpmMultiplier); // Calculate the number of samples in the note
    
                    int noteStartSample = (int)(note.Pos.x * sampleRate);
                    int noteEndSample = noteStartSample + numSamplesInNote; // Adjusting note end sample based on adjusted duration

                    for (int i = noteStartSample; i < noteEndSample; i++)
                    {
                        double angle = 2 * Math.PI * note.Frequency * (i - noteStartSample) / sampleRate;
                        short sample = (short)(Math.Sin(angle) * short.MaxValue);

                        // Apply fade-out effect towards the end of the note
                        double fadeOutFactor = 1.0 - ((double)(i - noteStartSample) / numSamplesInNote);
                        sample = (short)(sample * fadeOutFactor);

                        writer.Write(sample);
                        writer.Write(sample);
                    }
                }
            }
        }

        Debug.Log("WAV file exported successfully.");
    }
}