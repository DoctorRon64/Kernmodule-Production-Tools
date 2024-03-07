using System.Timers;

public class Timeline : ISaveable
{
    public int currentTimePos;
    public int TimelineLength;
    private Timer timer;

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

    public void StartTimeline()
    {
        currentTimePos = 0;
        timer = new Timer(TimelineLength);
        timer.Elapsed += TimerElapsed;
        timer.Start();

        if (currentTimePos == TimelineLength) {
            timer.Stop();
        }
    }

    private void TimerElapsed(object sender, ElapsedEventArgs e)
    {
        currentTimePos++;
    }
}
