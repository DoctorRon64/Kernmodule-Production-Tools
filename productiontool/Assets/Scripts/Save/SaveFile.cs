using System;
using System.Collections.Generic;

[Serializable]
public class SaveFile
{
    //Timeline
    public int currentTimePos = 0;
    public int TimelineLength = 64;
    public bool repeatTimeline = false;
    
    //notes
    public List<Note> NoteDatabase = new List<Note>();
}
