using System;
using System.Collections.Generic;

[Serializable]
public class DataManager
{
    static DataManager instance;
    public static DataManager Instance
    {
        get 
        { 
            if (instance == null)
                instance = new DataManager();
            return instance;
        }
        set
        {
            instance = value;
        }
    }

    //testing
    public int ivar = 12;
    public float vogel = 10.23f;
    public string hallo = "jaren";

    //Timeline
    public int currentTimePos = 0;
    public int TimelineLength = 64;

    //notes
    public Dictionary<string, Note> noteDatabase = new Dictionary<string, Note>();
}
