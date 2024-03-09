using System.Timers;

public class Timeline : ISaveable
{
    private int currentTimePos;
    private int TimelineLength;
    private bool repeatTimeline;
    private Timer timer;
    private DataManager dataManager;
    
    public Timeline()
    {
        dataManager = GameManager.Instance.DataManager;
    }
    
    public void Load()
    {
        this.currentTimePos = dataManager.currentTimePos;
        this.TimelineLength = dataManager.TimelineLength;
        this.repeatTimeline = dataManager.repeatTimeline;
    }

    public void Save()
    {
        dataManager.currentTimePos = this.currentTimePos;
        dataManager.TimelineLength = this.TimelineLength;
        dataManager.repeatTimeline = this.repeatTimeline;
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

    public void PauseTimeline()
    {
        if (timer == null) return;
        timer.Enabled = !timer.Enabled;
    }
    
    public void stopTimeline()
    {
        timer.Stop();
    }

    public void RepeatTimeline(bool _repeat)
    {
        repeatTimeline = _repeat;
    }

    private void TimerElapsed(object sender, ElapsedEventArgs e)
    {
        currentTimePos++;
        if (currentTimePos >= TimelineLength)
        {
            if (repeatTimeline)
            {
                StartTimeline();
            }
            else
            {
                stopTimeline();
            }
            //mischien event raisen
        }
    }

    public void Dispose()
    {
        timer?.Dispose();
    }
}