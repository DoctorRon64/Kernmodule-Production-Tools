using System;

[Serializable]
public class SettingsFile
{ 
    public bool DoesPlayerWantOverwritePopUp = true;
    public int SampleRate = MusicLib.SampleRateLib[0];
    public bool RepeatTimeline = true;
}