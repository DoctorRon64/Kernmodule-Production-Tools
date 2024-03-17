using System;

[Serializable]
public class SettingsFile
{ 
    public int SampleRate = MusicLib.SampleRateLib[0];
    public bool DoesPlayerWantOverwritePopUp = true;
    public bool RepeatTimeline = true;
}