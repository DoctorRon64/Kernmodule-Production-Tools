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
        timer = new Timer(1000); //<---- 1000ms = 1s
        timer.Elapsed += TimerElapsed;
        timer.Start();

        if (currentTimePos == TimelineLength)
        {
            timer.Stop();
        }
    }

    private void TimerElapsed(object sender, ElapsedEventArgs e)
    {
        currentTimePos++;
        if (currentTimePos >= TimelineLength)
        {
            timer.Stop();
            // Optionally, you might raise an event here to notify the completion of the timeline
        }
    }

    public void Dispose()
    {
        timer?.Dispose();
    }
}
