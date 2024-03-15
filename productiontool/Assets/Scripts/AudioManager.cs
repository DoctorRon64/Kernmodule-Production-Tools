using System;
using UnityEngine;

public class AudioManager
{
    private readonly AudioSource audioSource;

    public AudioManager(AudioSource _source)
    {
        audioSource = _source;
    }

    public void PlayClip(Note _note)
    {
        Debug.Log("Called play clip" + _note);
        const float noteLength = 0.3f;

        int sampleLength = Mathf.CeilToInt(_note.SampleRate * noteLength);
        float[] samples = new float[sampleLength];
        for (int i = 0; i < sampleLength; i++)
        {
            float time = (float)i / _note.SampleRate;
            samples[i] = Mathf.Sin(2 * Mathf.PI * _note.Frequency * time);
        }
        string randomClipName = GenerateUniqueClipName();

        AudioClip clip = AudioClip.Create(randomClipName, sampleLength, 1, _note.SampleRate, false);
        if (clip != null)
        {
            clip.SetData(samples, 0);
            audioSource.PlayOneShot(clip);
            UnityEngine.Object.Destroy(clip);
        }
        else
        {
            Debug.LogError("Failed to create AudioClip.");
        }
    }

    private string GenerateUniqueClipName()
    {
        Guid newUuid = Guid.NewGuid();
        return newUuid.ToString();
    }
}