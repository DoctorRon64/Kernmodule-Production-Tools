using System;
using System.Collections.Generic;

[Serializable]
public class SaveFile
{
    //notes
    public int BPM = 60;
    public List<Note> noteDatabase = new List<Note>();
}