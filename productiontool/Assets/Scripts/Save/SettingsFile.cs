using System;

[Serializable]
public class SettingsFile
{ 
    public int SampleRate = 0;
    public bool DoesPlayerWantOverwritePopUp = true;
    public bool RepeatTimeline = true;
    public bool FullscreenToggle = true;
}