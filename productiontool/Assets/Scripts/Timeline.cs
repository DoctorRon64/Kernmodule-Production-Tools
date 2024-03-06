public class Timeline : ISaveable 
{
    public int currentTimePos;
    public int TimelineLength;

    public void Load()
    {
        this.currentTimePos = DataManager.Instance.currentTimePos;
        this.TimelineLength = DataManager.Instance.TimelineLength;
    }

    public void Save()
    {
        DataManager.Instance.currentTimePos = this.currentTimePos;
        DataManager.Instance.TimelineLength = this.TimelineLength;
    }
}