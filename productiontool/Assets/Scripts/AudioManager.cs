﻿using UnityEngine;

public class AudioManager
{
    private AudioSource audioSource;

    public AudioManager(AudioSource _source)
    {
        audioSource = _source;
    }

    public void PlayCLip(Note _note)
    {
        AudioClip clip = GenerateTone(_note.Frequency, _note.SampleRate);
        audioSource.PlayOneShot(clip);
    }

    private AudioClip GenerateTone(float _frequency, int _sampleRate)
    {
        int lengthSecs = 1; // for now
        int sampleLength = _sampleRate * lengthSecs;
        float[] samples = new float[sampleLength];

        for (int i = 0; i < sampleLength; i++)
        {
            float time = (float)i / _sampleRate;
            samples[i] = Mathf.Sin(2 * Mathf.PI * _frequency * time);
        }

        AudioClip clip = AudioClip.Create("GenerateTone", sampleLength, 1, _sampleRate, false);
        clip.SetData(samples, 0);
        return clip;
    }
}