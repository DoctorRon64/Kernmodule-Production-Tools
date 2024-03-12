using System;
using System.Collections.Generic;

[Serializable]
public class SaveFile
{
    //Timeline
    public int currentTimePos = 0;
    public int timelineLength = 64;
    public bool repeatTimeline = false;
    
    //notes
    public List<Note> noteDatabase;
}
