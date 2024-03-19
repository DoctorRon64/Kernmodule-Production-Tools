using System;
using UnityEngine;

public class AudioManager
{
    private readonly AudioSource[] audioSources;
    
    public AudioManager(AudioSource[] _sources)
    {
        audioSources = _sources;
    }

    public void PlayClip(Note _note, int _sampleRate)
    {
        const float noteLength = 0.3f;
        
        // Calculate the sample length based on a longer duration
        float fadeOutDuration = 0.05f;
        int sampleLength = Mathf.CeilToInt((_sampleRate * (noteLength + fadeOutDuration)));
        
        float[] samples = new float[sampleLength];
        for (int i = 0; i < sampleLength; i++)
        {
            float time = (float)i / _sampleRate;
            float amplitude = Mathf.Sin(2 * Mathf.PI * _note.Frequency * time) * 0.2f;
            if (time > noteLength)
            {
                float t = (time - noteLength) / fadeOutDuration;
                amplitude *= Mathf.Lerp(1f, 0f, t);
            }
            samples[i] = amplitude;
        }
        
        string randomClipName = GenerateUniqueClipName();
        
        // Generate audio file
        AudioClip clip = AudioClip.Create(randomClipName, sampleLength, 1 , _sampleRate, false);
        if (clip != null)
        {
            clip.SetData(samples, 0);
            AudioSource source = GetAudioSource();
            source.PlayOneShot(clip);
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

    private AudioSource GetAudioSource()
    {
        foreach (AudioSource source in audioSources)
        {
            if (!source.isPlaying)
            {
                return source;
            }
        }

        return null;
    }
}

