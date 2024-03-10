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
    public Dictionary<string, Note> noteDatabase = new Dictionary<string, Note>();
}
